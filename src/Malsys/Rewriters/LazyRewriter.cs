using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Malsys.Expressions;
using Microsoft.FSharp.Collections;
using FunMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.FunctionDefinition>;
using Symbol = Malsys.Symbol<Malsys.Expressions.IValue>;
using SymbolPatern = Malsys.Symbol<string>;
using SymbolPaternsList = Malsys.SymbolsList<string>;
using VarMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.Expressions.IValue>;

namespace Malsys.Rewriters {
	public class LazyRewriter : IRewriter {

		private Dictionary<string, RewriteRule[]> rewriteRules;
		private VarMap variables;
		private FunMap functions;
		private Random rndGenerator;

		private IEnumerator<Symbol> source;
		private IndexableQueue<Symbol> inputBuffer;
		/// <summary>
		/// Cyclic history array. Length should not be 0.
		/// </summary>
		private Symbol[] outputHistory;
		/// <summary>
		/// Length of output history array. Optimal value should be determined in initialization picking length
		/// of the logngest context of all left contexts
		/// </summary>
		private int outputHistoryLength;
		/// <summary>
		/// Pointing after last added symbol in output history array.
		/// </summary>
		private int outputHistoryIndex;
		private int returnedSymbolsCount;


		/// <summary>
		/// Ensures that desired number of symbols is loaded in input buffer or tries to load them from source.
		/// </summary>
		/// <param name="n">Desired number of loaded symbols in input buffer.</param>
		/// <returns>Returns true if desired number of symbols was loaded into bufffer, otherwise false.</returns>
		private bool ensureLoadedSymbols(int n) {
			while (inputBuffer.Count < n) {
				if (source.MoveNext()) {
					inputBuffer.Enqueue(source.Current);
				}
				else {
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Adds given symbol to optput history. Also raises total returned symbols counter.
		/// </summary>
		/// <param name="s"></param>
		private void addSymbolToHistory(Symbol s) {
			outputHistory[outputHistoryIndex] = s;
			outputHistoryIndex = (outputHistoryIndex + 1) % outputHistory.Length;

			returnedSymbolsCount++;
		}

		private bool tryRewrite(Symbol symbol, out RewriteRule rruleResult, out VarMap vars) {

			vars = variables;
			RewriteRule[] rrules;

			if (rewriteRules.TryGetValue(symbol.Name, out rrules)) {

				var possibleRrules = new List<Tuple<float, RewriteRule, VarMap>>();

				foreach (var rr in rrules) {

					if (rr.SymbolPattern.Name != symbol.Name) {
						continue;
					}
					if (rr.LeftContext.Length > 0 && !checkLeftContext(rr.LeftContext)) {
						continue;
					}
					if (rr.RightContext.Length > 0 && !checkRightContext(rr.RightContext)) {
						continue;
					}

					// matching values on patterns

					// pattern
					vars = matchPattern(rr.SymbolPattern, symbol, vars);

					// left context
					int lCctxtLen = rr.LeftContext.Length;
					for (int i = 0; i < lCctxtLen; i++) {
						int historyIndex = (outputHistoryIndex - lCctxtLen + i + outputHistoryLength) % outputHistoryLength;
						vars = matchPattern(rr.LeftContext[i], outputHistory[historyIndex], vars);
					}

					// right context
					int rCtxtLen = rr.RightContext.Length;
					for (int i = 0; i < rCtxtLen; i++) {
						vars = matchPattern(rr.RightContext[i], inputBuffer[i], vars);
					}

					// condition
					foreach (var varDef in rr.Condition.VariableDefinitions) {
						vars = VariableDefinitionEvaluator.EvaluateAndAdd(varDef, vars, functions);
					}

					var condValue = ExpressionEvaluator.Evaluate(rr.Condition.Expression, vars, functions);
					if (!condValue.IsConstant) {
						continue;
					}

					var condConst = (Constant)condValue;
					if (condConst.IsNaN || condConst.IsZero()) {
						continue;
					}

					// probability weight

					foreach (var varDef in rr.ProbabilityWeight.VariableDefinitions) {
						vars = VariableDefinitionEvaluator.EvaluateAndAdd(varDef, vars, functions);
					}

					var probabValue = ExpressionEvaluator.Evaluate(rr.Condition.Expression, vars, functions);
					if (!probabValue.IsConstant) {
						continue;
					}

					var probabConst = (Constant)probabValue;
					if (probabConst.IsNaN || probabConst.Value < 0) {
						continue;
					}

					possibleRrules.Add(new Tuple<float, RewriteRule, VarMap>((float)probabConst.Value, rr, vars));
				}

				if (possibleRrules.Count > 0) {

					float sumWeights = possibleRrules.Sum(tuple => tuple.Item1);
					float rand = (float)(rndGenerator.NextDouble() * sumWeights);
					float acc = 0f;

					foreach (var tuple in possibleRrules) {
						acc += tuple.Item1;
						if (rand < acc) {
							rruleResult = tuple.Item2;
							vars = tuple.Item3;
							return true;
						}
					}

					// this should be unreachable, but float arithmetic may be inaccurate, so if program reaches here,
					// acc should be very close to rand
					Debug.Assert(Math.Abs(rand - acc) < 1e-6, "Random number {0} should be close to {1}.".Fmt(rand, acc));

					// return last item as result
					rruleResult = possibleRrules[possibleRrules.Count - 1].Item2;
					vars = possibleRrules[possibleRrules.Count - 1].Item3;
					return true;
				}

			}

			rruleResult = null;
			return false;
		}

		private bool checkLeftContext(SymbolPaternsList ctxt) {
			// left context check relies on sufficient size of symbols history
			int ctxtLen = ctxt.Length;

			if (returnedSymbolsCount < ctxtLen) {
				return false;  // not enough symbols in history
			}

			for (int i = 0; i < ctxtLen; i++) {
				int historyIndex = (outputHistoryIndex - ctxtLen + i + outputHistoryLength) % outputHistoryLength;
				if (ctxt[i].Name != outputHistory[historyIndex].Name) {
					return false;
				}
			}

			return true;
		}

		private bool checkRightContext(SymbolPaternsList ctxt) {

			int ctxtLen = ctxt.Length;

			if (!ensureLoadedSymbols(ctxtLen)) {
				return false;  // not enough symbols in input
			}

			for (int i = 0; i < ctxtLen; i++) {
				if (ctxt[i].Name != inputBuffer[i].Name) {
					return false;
				}
			}

			return true;
		}

		private VarMap matchPattern(SymbolPatern p, Symbol s, VarMap vars) {
			int argsLen = s.Arguments.Length;
			int patternLen = p.Arguments.Length;

			for (int i = 0; i < patternLen; i++) {
				var value = i < argsLen ? s.Arguments[i] : Constant.NaN;
				vars = MapModule.Add(p.Arguments[i], value, vars);
			}

			return vars;
		}

		/// <summary>
		/// Returns optimal history length based on max length of left context on rewrite rule.
		/// </summary>
		private int getOptimalHistoryLength(Dictionary<string, RewriteRule[]> rrules) {
			return rrules
				.Select(d => d.Value)
				.Max(arr => arr.Max(rr => rr.LeftContext.Length));
		}


		#region IRewriter Members

		public void Initialize(Dictionary<string, RewriteRule[]> rrules, VarMap vars, FunMap funs, Random rnd) {
			rewriteRules = rrules;
			variables = vars;
			functions = funs;
			rndGenerator = rnd;
			inputBuffer = new IndexableQueue<Symbol>();

			// optimal history length + 1 to be safe and to NOT have history of 0 length
			outputHistoryLength = getOptimalHistoryLength(rewriteRules) + 1;
			outputHistory = new Symbol[outputHistoryLength];
			outputHistoryIndex = 0;

			returnedSymbolsCount = 0;
		}

		public IEnumerable<Symbol> Rewrite(IEnumerable<Symbol> src) {
			source = src.GetEnumerator();

			while (ensureLoadedSymbols(1)) {
				var symbol = inputBuffer.Dequeue();

				VarMap vars;
				RewriteRule rrule;

				if (tryRewrite(symbol, out rrule, out vars)) {
					// define replacement variables
					foreach (var varDef in rrule.ReplacementVars) {
						vars = varDef.EvaluateAndAdd(vars, functions);
					}

					// return evaluated replacement
					foreach (var replacSymbol in rrule.Replacement) {
						var result = replacSymbol.Evaluate(vars, functions);
						addSymbolToHistory(result);
						yield return result;
					}
				}
				else {
					// no rewrite rule found for symbol, identity (symbol rewrotes to itself)
					addSymbolToHistory(symbol);
					yield return symbol;
				}
			}

			source = null;
		}

		#endregion
	}
}

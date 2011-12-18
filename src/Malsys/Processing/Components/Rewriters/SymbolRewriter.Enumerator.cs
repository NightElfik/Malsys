using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using Malsys.Evaluators;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Evaluated;
using ConstsMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Evaluated.IValue>;
using FunsMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Compiled.FunctionEvaledParams>;
using Symbol = Malsys.SemanticModel.Symbol<Malsys.SemanticModel.Evaluated.IValue>;
using SymbolPatern = Malsys.SemanticModel.Symbol<string>;
using SymbolPaternsList = Malsys.SemanticModel.SymbolsList<string>;

namespace Malsys.Processing.Components.Rewriters {
	public partial class SymbolRewriter {

		private class SymbolRewriterEnumerator : IEnumerator<Symbol> {

			private readonly SymbolRewriter parent;
			private readonly IExpressionEvaluator exprEvaluator;
			private readonly ConstsMap constants;
			private readonly FunsMap functions;
			private readonly Dictionary<string, RewriteRule[]> rewriteRules;
			private readonly HashSet<string> contextIgnoredSymbolNames;
			private readonly IRandomGenerator rndGenerator;

			private readonly IEnumerator<Symbol> symbolsSource;

			private readonly IndexableQueue<Symbol> inputBuffer;

			private readonly IndexableQueue<Symbol> outputBuffer;

			/// <summary>
			/// Maximal length of left context from all rewrite rules.
			/// </summary>
			private readonly int leftCtxtMaxLen;

			/// <summary>
			/// In left context queue are kept only valid symbols for context checking.
			/// </summary>
			private readonly IndexableQueue<Symbol> leftContext;

			/// <summary>
			/// Maximal length of right context from all rewrite rules.
			/// </summary>
			private readonly int rightCtxtMaxLen;

			/// <summary>
			/// In right context queue are kept only valid symbols for context checking.
			/// If no symbols are ignored, this queue is same as input buffer.
			/// </summary>
			private readonly IndexableQueue<Symbol> rightContext;


			public SymbolRewriterEnumerator(SymbolRewriter parentSr) {

				parent = parentSr;

				symbolsSource = parent.SymbolProvider.GetEnumerator();

				exprEvaluator = parent.exprEvaluator;

				rewriteRules = parent.rewriteRules;
				constants = parent.lsystem.Constants;
				functions = parent.lsystem.Functions;

				contextIgnoredSymbolNames = parent.contextIgnoredSymbolNames;
				if (parent.RandomGeneratorProvider == null) {
					rndGenerator = new PseudoRandomGenerator();
				}
				else {
					rndGenerator = parent.RandomGeneratorProvider.GetRandomGenerator();
				}
				leftCtxtMaxLen = parent.leftCtxtMaxLen;
				rightCtxtMaxLen = parent.rightCtxtMaxLen;

				inputBuffer = new IndexableQueue<Symbol>(rightCtxtMaxLen + 4);
				// optimal output buffer length is max length of any replacement
				outputBuffer = new IndexableQueue<Symbol>(parent.rrReplacementMaxLen + 1);

				// optimal history length + 1 to be able add before delete
				leftContext = new IndexableQueue<Symbol>(leftCtxtMaxLen + 1);
				rightContext = new IndexableQueue<Symbol>(rightCtxtMaxLen + 1);
			}


			#region IEnumerator<Symbol<IValue>> Members

			public bool MoveNext() {

				if (outputBuffer.Count > 0) {
					Current = outputBuffer.Dequeue();
					return true;
				}

				Symbol currSymbol;
				while (tryGetInputSymbol(out currSymbol)) {

					rewrite(currSymbol);

					// add processed symbol to left context
					if (!isIgnoredInContext(currSymbol)) {
						leftContext.Enqueue(currSymbol);
						if (leftContext.Count > leftCtxtMaxLen) {
							leftContext.Dequeue();  // throw away unnecessary symbol from left context
						}
					}

					if (outputBuffer.Count > 0) {
						Current = outputBuffer.Dequeue();
						return true;
					}
				}

				Current = null;
				return false;  // no symbols to process
			}

			public Symbol Current { get; private set; }

			object System.Collections.IEnumerator.Current {
				get { return Current; }
			}

			public void Reset() {
				throw new NotImplementedException();
			}

			public void Dispose() {

			}

			#endregion


			private bool tryGetInputSymbol(out Symbol symbol) {

				if (!ensureInputSymbolsCount(1)) {
					symbol = null;
					return false;
				}

				symbol = inputBuffer.Dequeue();
				if (!isIgnoredInContext(symbol)) {
					var rCtxt = rightContext.Dequeue();
					Debug.Assert(rCtxt == symbol, "First symbol in right context do not match first symbol in input buffer.");
				}

				return true;
			}

			private bool ensureInputSymbolsCount(int count) {

				Contract.Ensures(Contract.Result<bool>() ? inputBuffer.Count >= count : true);

				int symbolsToLoad = count - inputBuffer.Count;

				if (symbolsToLoad <= 0) {
					return true;  // required amount of symbols already loaded
				}

				int loaded = loadSymbols(symbolsToLoad);
				return loaded >= symbolsToLoad; // return false if less than required is loaded
			}

			private int loadSymbols(int count) {

				int loadedSymbolsCount = 0;

				while (count-- > 0 && symbolsSource.MoveNext()) {
					var symbol = symbolsSource.Current;
					inputBuffer.Enqueue(symbol);
					if (!isIgnoredInContext(symbol)) {
						rightContext.Enqueue(symbol);
					}
					loadedSymbolsCount++;
				}

				return loadedSymbolsCount;

			}

			/// <summary>
			/// Rewrites given symbol to output buffer.
			/// </summary>
			private void rewrite(Symbol symbol) {

				ConstsMap consts;
				RewriteRule rrule;

				if (tryFindRewriteRule(symbol, out rrule, out consts)) {

					var replac = chooseReplacement(rrule, consts, functions);

					foreach (var replacSymbol in replac.Replacement) {
						var evaledSymbol = new Symbol<IValue>(replacSymbol.Name, exprEvaluator.EvaluateList(replacSymbol.Arguments, consts, functions));
						outputBuffer.Enqueue(evaledSymbol);
					}
				}
				else {
					// implicit identity
					outputBuffer.Enqueue(symbol);
				}
			}

			private bool tryFindRewriteRule(Symbol symbol, out RewriteRule rruleResult, out ConstsMap constsResult) {

				RewriteRule[] rrules;
				if (rewriteRules.TryGetValue(symbol.Name, out rrules)) {
					foreach (var rr in rrules) {

						Debug.Assert(rr.SymbolPattern.Name == symbol.Name, "Bad rewrite rule Dictionary.");

						// get work copy of variables
						var actualConsts = constants;

						// left context check first, it is simpler (do not invoke input reading as right context check)
						if (rr.LeftContext.Length > 0 && !checkContext(leftContext, rr.LeftContext, ref actualConsts)) {
							continue;
						}

						if (rr.RightContext.Length > 0 && !checkRightContext(rr.RightContext, ref actualConsts)) {
							continue;
						}

						// map pattern
						mapPatternConsts(rr.SymbolPattern, symbol, ref actualConsts);

						// map local constant definitions
						foreach (var cd in rr.LocalConstantDefs) {
							actualConsts = actualConsts.Add(cd.Name, exprEvaluator.Evaluate(cd.Value, actualConsts, functions));
						}

						// check condition
						var condValue = exprEvaluator.Evaluate(rr.Condition, actualConsts, functions);
						if (!condValue.IsConstant) {
							continue;
						}

						var condConst = (Constant)condValue;
						if (condConst.IsNaN || condConst.IsZero) {
							continue;
						}

						rruleResult = rr;
						constsResult = actualConsts;


						return true;
					}

				}

				rruleResult = null;
				constsResult = null;
				return false;
			}




			private bool checkContext(IndexableQueue<Symbol> contextSymbols, SymbolPaternsList ctxtPattern, ref ConstsMap consts) {

				if (contextSymbols.Count < ctxtPattern.Length) {
					return false;
				}

				for (int i = 0; i < ctxtPattern.Length; i++) {
					if (ctxtPattern[i].Name == contextSymbols[i].Name) {
						mapPatternConsts(ctxtPattern[i], contextSymbols[i], ref consts);
					}
					else {
						return false;
					}
				}

				return true;
			}

			private bool checkRightContext(SymbolPaternsList ctxt, ref ConstsMap consts) {

				while (rightContext.Count < ctxt.Length) {
					// load as much symbols as needed to match context if non of newly loaded will be ignored
					// at least 1 symbol is loaded because rightContext.Count < ctxt.Length
					int toLoad = ctxt.Length - rightContext.Count;
					int loaded = loadSymbols(toLoad);
					if (loaded < toLoad) {
						return false; // not enough symbols to match context
					}
				}

				return checkContext(rightContext, ctxt, ref consts);
			}

			private void mapPatternConsts(SymbolPatern pattern, Symbol symbol, ref ConstsMap consts) {

				int paramsLen = symbol.Arguments.Length;
				int patternLen = pattern.Arguments.Length;

				for (int i = 0; i < patternLen; i++) {
					// set value to NaN if symbol has not enough actual parameters to match pattern
					var value = i < paramsLen ? symbol.Arguments[i] : Constant.NaN;
					consts = consts.Add(pattern.Arguments[i], value);
				}

			}

			private RewriteRuleReplacement chooseReplacement(RewriteRule rr, ConstsMap consts, FunsMap funs) {

				int rrrLen = rr.Replacements.Length;

				if (rrrLen == 0) {
					return RewriteRuleReplacement.Empty;
				}
				else if (rrrLen == 1) {
					return rr.Replacements[0];
				}

				var weights = rr.Replacements.Select(replac => (double)exprEvaluator.EvaluateAsConst(replac.Weight, consts, funs)).ToArray();
				double sumWeights = weights.Sum();
				double rand = rndGenerator.NextDouble() * sumWeights;
				double acc = 0d;

				for (int i = 0; i < rrrLen; i++) {
					acc += weights[i];
					if (rand < acc) {
						return rr.Replacements[i];
					}
				}

				// this should be unreachable but float arithmetic may be inaccurate so if program reaches here,
				// acc should be very close to rand
				// check this invariant while debugging
				Debug.Assert(Math.Abs(rand - acc) < 1e-6, "Random number {0} should be close to {1}.".Fmt(rand, acc));

				// return last item as result
				return rr.Replacements[rrrLen - 1];
			}

			private bool isIgnoredInContext(Symbol symbol) {
				return contextIgnoredSymbolNames.Contains(symbol.Name);
			}

		}

	}
}

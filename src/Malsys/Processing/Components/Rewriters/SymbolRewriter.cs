using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using Malsys.Expressions;
using Microsoft.FSharp.Collections;
using FunMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.FunctionDefinition>;
using Symbol = Malsys.Symbol<Malsys.Expressions.IValue>;
using SymbolPatern = Malsys.Symbol<string>;
using SymbolPaternsList = Malsys.SymbolsList<string>;
using VarMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.Expressions.IValue>;

namespace Malsys.Processing.Components.Rewriters {
	public class SymbolRewriter : IRewriter {

		private ISymbolProcessor outputProcessor;

		private Dictionary<string, RewriteRule[]> rewriteRules;

		private VarMap variables;
		private FunMap functions;

		private Random rndGenerator;

		private int seed;

		private int leftCtxtMaxLen;
		private IndexableQueue<Symbol> leftContext;

		private int rightCtxtMaxLen;
		private IndexableQueue<Symbol> rightContext;




		public SymbolRewriter() {

			outputProcessor = EmptySymbolProcessor.Instance;

			variables = MapModule.Empty<string, IValue>();
			functions = MapModule.Empty<string, FunctionDefinition>();

			rndGenerator = new Random();

			rewriteRules = new Dictionary<string, RewriteRule[]>();
			initContextCaches();
		}

		[ContractInvariantMethod]
		private void objectInvariant() {

			Contract.Invariant(outputProcessor != null);
			Contract.Invariant(rewriteRules != null);

			Contract.Invariant(variables != null);
			Contract.Invariant(functions != null);

			Contract.Invariant(rndGenerator != null);

			Contract.Invariant(leftContext != null);
			Contract.Invariant(rightContext != null);
		}


		#region IRewriter Members


		public ISymbolProcessor OutputProcessor {
			set { outputProcessor = value; }
		}

		public IEnumerable<RewriteRule> RewriteRules {
			set {
				rewriteRules = RewriterUtils.CreateRrulesMap(value);

				initContextCaches();
			}
		}

		public VarMap Variables {
			set { variables = value; }
		}

		public FunMap Functions {
			set { functions = value; }
		}

		public IValue RandomSeed {
			set {
				if (value.IsConstant && !((Constant)value).IsNaN) {
					seed = ((Constant)value).GetIntValueRounded();
				}
				else {
					throw new ArgumentException("Random seed value is invalid.");
				}
			}
		}

		#endregion

		#region ISymbolProcessor Members

		public void BeginProcessing() {
			rndGenerator = new Random(seed);

			outputProcessor.BeginProcessing();
		}

		public void ProcessSymbol(Symbol symbol) {
			rightContext.Enqueue(symbol);

			if (rightContext.Count < rightCtxtMaxLen + 1) {
				return;  // too few symbols in right context, wait for more
			}

			var currSym = rightContext.Dequeue();
			rewrite(currSym);
		}

		public void EndProcessing() {
			while (rightContext.Count > 0) {

				var currSym = rightContext.Dequeue();
				rewrite(currSym);
			}

			leftContext.Clear();
			rightContext.Clear();

			outputProcessor.EndProcessing();
		}

		#endregion



		private void rewrite(Symbol symbol) {

			VarMap vars;
			RewriteRule rrule;

			if (tryFindRewriteRule(symbol, out rrule, out vars)) {
				var replac = chooseReplacement(rrule, vars, functions);
				foreach (var s in replac.Replacement) {
					outputProcessor.ProcessSymbol(s.Evaluate(vars, functions));
				}
			}
			else {
				outputProcessor.ProcessSymbol(symbol);
			}

			leftContext.Enqueue(symbol);
			if (leftContext.Count > leftCtxtMaxLen) {
				leftContext.Dequeue();  // throw away unnecessary symbols from left context
			}
		}


		private bool tryFindRewriteRule(Symbol symbol, out RewriteRule rruleResult, out VarMap varsResult) {

			RewriteRule[] rrules;
			if (rewriteRules.TryGetValue(symbol.Name, out rrules)) {
				foreach (var rr in rrules) {
					// get work copy of variables
					var vars = variables;

					Debug.Assert(rr.SymbolPattern.Name == symbol.Name, "Bad rewrite rule Dictionary.");

					if (rr.LeftContext.Length > 0 && !checkContext(false, rr.LeftContext, ref vars)) {
						continue;
					}
					if (rr.RightContext.Length > 0 && !checkContext(true, rr.RightContext, ref vars)) {
						continue;
					}

					// map pattern
					vars = mapPatternVars(rr.SymbolPattern, symbol, vars);
					// map local variables
					foreach (var varDef in rr.LocalVariables) {
						vars = VariableDefinitionEvaluator.EvaluateAndAdd(varDef, vars, functions);
					}

					// check condition
					var condValue = ExpressionEvaluator.Evaluate(rr.Condition, vars, functions);
					if (!condValue.IsConstant) {
						continue;
					}

					var condConst = (Constant)condValue;
					if (condConst.IsNaN || condConst.IsZero()) {
						continue;
					}

					rruleResult = rr;
					varsResult = vars;
					return true;
				}

			}

			rruleResult = null;
			varsResult = null;
			return false;
		}

		private bool checkContext(bool right, SymbolPaternsList ctxt, ref VarMap vars) {

			var context = right ? rightContext : leftContext;

			if (context.Count < ctxt.Length) {
				return false;
			}

			for (int i = 0; i < ctxt.Length; i++) {
				if (ctxt[i].Name == context[i].Name) {
					vars = mapPatternVars(ctxt[i], context[i], vars);
				}
				else {
					return false;
				}
			}

			return true;
		}

		private VarMap mapPatternVars(SymbolPatern pattern, Symbol symbol, VarMap vars) {
			int argsLen = symbol.Arguments.Length;
			int patternLen = pattern.Arguments.Length;

			for (int i = 0; i < patternLen; i++) {
				var value = i < argsLen ? symbol.Arguments[i] : Constant.NaN;
				vars = MapModule.Add(pattern.Arguments[i], value, vars);
			}

			return vars;
		}

		private RewriteRuleReplacement chooseReplacement(RewriteRule rr, VarMap vars, FunMap funs) {

			int rrrLen = rr.Replacements.Length;

			if (rrrLen == 0) {
				return RewriteRuleReplacement.Empty;
			}
			else if (rrrLen == 1) {
				return rr.Replacements[0];
			}

			var weights = rr.Replacements.Select(replac => (double)ExpressionEvaluator.EvaluateAsConst(replac.Weight, vars, funs)).ToArray();
			double sumWeights = weights.Sum();
			double rand = rndGenerator.NextDouble() * sumWeights;
			double acc = 0d;

			for (int i = 0; i < rrrLen; i++) {
				acc += weights[i];
				if (rand < acc) {
					return rr.Replacements[i];
				}
			}

			// this should be unreachable, but float arithmetic may be inaccurate, so if program reaches here,
			// acc should be very close to rand
			Debug.Assert(Math.Abs(rand - acc) < 1e-6, "Random number {0} should be close to {1}.".Fmt(rand, acc));

			// return last item as result
			return rr.Replacements[rrrLen - 1];
		}


		private void initContextCaches() {

			var rRules = rewriteRules.Select(d => d.Value);

			leftCtxtMaxLen = rRules.Any()
				? rRules.Max(arr => arr.Max(rr => rr.LeftContext.Length))
				: 0;

			rightCtxtMaxLen = rRules.Any()
				? rRules.Max(arr => arr.Max(rr => rr.RightContext.Length))
				: 0;

			// optimal history length + 1 to be able add before delete and to NOT have history of length 0
			leftContext = new IndexableQueue<Symbol>(leftCtxtMaxLen + 1);
			rightContext = new IndexableQueue<Symbol>(rightCtxtMaxLen + 1);
		}
	}
}

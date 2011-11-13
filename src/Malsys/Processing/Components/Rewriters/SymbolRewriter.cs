using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using Malsys.Evaluators;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Evaluated;
using Microsoft.FSharp.Collections;
using FunMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Evaluated.FunctionEvaledParams>;
using Symbol = Malsys.SemanticModel.Symbol<Malsys.SemanticModel.Evaluated.IValue>;
using SymbolPatern = Malsys.SemanticModel.Symbol<string>;
using SymbolPaternsList = Malsys.SemanticModel.SymbolsList<string>;
using VarMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Evaluated.IValue>;

namespace Malsys.Processing.Components.Rewriters {
	public class SymbolRewriter : IRewriter {

		private ISymbolProcessor outputProcessor;
		private ExpressionEvaluator exprEvaluator;
		private BindingsEvaluator bindEvaluator;

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
			functions = MapModule.Empty<string, FunctionEvaledParams>();

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

		public ISymbolProcessor OutputProcessor { set { outputProcessor = value; } }

		public ProcessContext Context {
			set {
				exprEvaluator = value.ExpressionEvaluator;
				bindEvaluator = new BindingsEvaluator(exprEvaluator);
				variables = value.Lsystem.Variables;
				functions = value.Lsystem.Functions;
				rewriteRules = createRrulesMap(value.Lsystem.RewriteRules);

				initContextCaches();
			}
		}

		public IValue RandomSeed {
			set {
				if (value.IsConstant && !((Constant)value).IsNaN) {
					seed = ((Constant)value).RoundedIntValue;
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

			BindingMaps data;
			RewriteRule rrule;

			if (tryFindRewriteRule(symbol, out rrule, out data)) {
				var replac = chooseReplacement(rrule, data.Variables, data.Functions);
				foreach (var s in replac.Replacement) {
					outputProcessor.ProcessSymbol(exprEvaluator.EvaluateSymbol(s, data.Variables, data.Functions));
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


		private bool tryFindRewriteRule(Symbol symbol, out RewriteRule rruleResult, out BindingMaps mapsResult) {

			RewriteRule[] rrules;
			if (rewriteRules.TryGetValue(symbol.Name, out rrules)) {
				foreach (var rr in rrules) {
					// get work copy of variables
					BindingMaps data = new BindingMaps() {
						Variables = variables,
						Functions = functions
					};

					Debug.Assert(rr.SymbolPattern.Name == symbol.Name, "Bad rewrite rule Dictionary.");

					if (rr.LeftContext.Length > 0 && !checkContext(false, rr.LeftContext, data)) {
						continue;
					}
					if (rr.RightContext.Length > 0 && !checkContext(true, rr.RightContext, data)) {
						continue;
					}

					// map pattern
					data.Variables = mapPatternVars(rr.SymbolPattern, symbol, data.Variables);
					// map local bindings
					bindEvaluator.EvaluateList(rr.LocalBindings, data);

					// check condition
					var condValue = exprEvaluator.Evaluate(rr.Condition, data.Variables, data.Functions);
					if (!condValue.IsConstant) {
						continue;
					}

					var condConst = (Constant)condValue;
					if (condConst.IsNaN || condConst.IsZero()) {
						continue;
					}

					rruleResult = rr;
					mapsResult = data;


					return true;
				}

			}

			rruleResult = null;
			mapsResult = null;
			return false;
		}

		private bool checkContext(bool right, SymbolPaternsList ctxt, BindingMaps maps) {

			var context = right ? rightContext : leftContext;

			if (context.Count < ctxt.Length) {
				return false;
			}

			for (int i = 0; i < ctxt.Length; i++) {
				if (ctxt[i].Name == context[i].Name) {
					maps.Variables = mapPatternVars(ctxt[i], context[i], maps.Variables);
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
				vars = vars.Add(pattern.Arguments[i], value);
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

			var weights = rr.Replacements.Select(replac => (double)exprEvaluator.EvaluateAsConst(replac.Weight, vars, funs)).ToArray();
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

		private Dictionary<string, RewriteRule[]> createRrulesMap(IEnumerable<RewriteRule> rrules) {

			Contract.Requires<ArgumentNullException>(rrules != null);
			Contract.Ensures(Contract.Result<Dictionary<string, RewriteRule[]>>() != null);

			return rrules
				.GroupBy(rr => rr.SymbolPattern.Name)
				.ToDictionary(g => g.Key, g => g.ToArray());
		}
	}
}

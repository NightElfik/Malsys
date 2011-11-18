using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using Malsys.Evaluators;
using Malsys.Processing.Components.Common;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Evaluated;
using Microsoft.FSharp.Collections;
using ConstsMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Evaluated.IValue>;
using FunsMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Compiled.FunctionEvaledParams>;
using Symbol = Malsys.SemanticModel.Symbol<Malsys.SemanticModel.Evaluated.IValue>;
using SymbolPatern = Malsys.SemanticModel.Symbol<string>;
using SymbolPaternsList = Malsys.SemanticModel.SymbolsList<string>;

namespace Malsys.Processing.Components.Rewriters {
	public class SymbolRewriter : IRewriter {

		private ISymbolProcessor outputProcessor;
		private ExpressionEvaluator exprEvaluator;

		private Dictionary<string, RewriteRule[]> rewriteRules;

		private ConstsMap constants;
		private FunsMap functions;

		private Random rndGenerator;

		private int seed;

		private int leftCtxtMaxLen;
		private IndexableQueue<Symbol> leftContext;

		private int rightCtxtMaxLen;
		private IndexableQueue<Symbol> rightContext;


		private bool rewriteRuleApplied;
		private long rewrittenSymbols;




		public SymbolRewriter() {

			outputProcessor = EmptySymbolProcessor.Instance;

			constants = MapModule.Empty<string, IValue>();
			functions = MapModule.Empty<string, FunctionEvaledParams>();

			rewriteRules = new Dictionary<string, RewriteRule[]>();
			initContextCaches();
		}

		[ContractInvariantMethod]
		private void objectInvariant() {

			Contract.Invariant(outputProcessor != null);

			Contract.Invariant(rewriteRules != null);

			Contract.Invariant(constants != null);
			Contract.Invariant(functions != null);

			Contract.Invariant(leftContext != null);
			Contract.Invariant(rightContext != null);
		}



		#region IRewriter Members

		public ISymbolProcessor OutputProcessor {
			set { outputProcessor = value; }
		}

		[UserSettable]
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

		public void ProcessSymbol(Symbol symbol) {

			rightContext.Enqueue(symbol);

			if (rightContext.Count < rightCtxtMaxLen + 1) {
				return;  // too few symbols in right context, wait for more
			}

			var currSym = rightContext.Dequeue();
			rewrite(currSym);
		}

		#endregion

		#region IComponent Members

		public ProcessContext Context {
			set {
				exprEvaluator = value.ExpressionEvaluator;
				constants = value.Lsystem.Constants;
				functions = value.Lsystem.Functions;
				rewriteRules = createRrulesMap(value.Lsystem.RewriteRules);

				initContextCaches();
			}
		}

		public void BeginProcessing(bool measuring) {

			rndGenerator = new Random(seed);
			leftContext.Clear();
			rightContext.Clear();
			rewriteRuleApplied = false;
			rewrittenSymbols = 0;

			outputProcessor.BeginProcessing(measuring);
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

		#region User readable properties

		public IValue RewrittenSymbols { get { return rewrittenSymbols.ToConst(); } }

		#endregion



		private void rewrite(Symbol symbol) {

			ConstsMap consts;
			RewriteRule rrule;

			if (tryFindRewriteRule(symbol, out rrule, out consts)) {
				rewriteRuleApplied = true;
				var replac = chooseReplacement(rrule, consts, functions);
				foreach (var s in replac.Replacement) {
					outputProcessor.ProcessSymbol(exprEvaluator.EvaluateSymbol(s, consts, functions));
				}
			}
			else {
				outputProcessor.ProcessSymbol(symbol);
			}

			leftContext.Enqueue(symbol);
			if (leftContext.Count > leftCtxtMaxLen) {
				leftContext.Dequeue();  // throw away unnecessary symbols from left context
			}

			rewrittenSymbols++;
		}


		private bool tryFindRewriteRule(Symbol symbol, out RewriteRule rruleResult, out ConstsMap constsResult) {

			RewriteRule[] rrules;
			if (rewriteRules.TryGetValue(symbol.Name, out rrules)) {
				foreach (var rr in rrules) {
					// get work copy of variables
					var actualConsts = constants;

					Debug.Assert(rr.SymbolPattern.Name == symbol.Name, "Bad rewrite rule Dictionary.");

					if (rr.LeftContext.Length > 0 && !checkContext(false, rr.LeftContext, ref actualConsts)) {
						continue;
					}
					if (rr.RightContext.Length > 0 && !checkContext(true, rr.RightContext, ref actualConsts)) {
						continue;
					}

					// map pattern
					actualConsts = mapPatternConsts(rr.SymbolPattern, symbol, actualConsts);
					// map local bindings
					foreach (var cd in rr.LocalConstantDefs) {
						actualConsts = actualConsts.Add(cd.Name, exprEvaluator.Evaluate(cd.Value));
					}

					// check condition
					var condValue = exprEvaluator.Evaluate(rr.Condition, actualConsts, functions);
					if (!condValue.IsConstant) {
						continue;
					}

					var condConst = (Constant)condValue;
					if (condConst.IsNaN || condConst.IsZero()) {
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

		private bool checkContext(bool right, SymbolPaternsList ctxt, ref ConstsMap consts) {

			var context = right ? rightContext : leftContext;

			if (context.Count < ctxt.Length) {
				return false;
			}

			for (int i = 0; i < ctxt.Length; i++) {
				if (ctxt[i].Name == context[i].Name) {
					consts = mapPatternConsts(ctxt[i], context[i], consts);
				}
				else {
					return false;
				}
			}

			return true;
		}

		private ConstsMap mapPatternConsts(SymbolPatern pattern, Symbol symbol, ConstsMap consts) {
			int argsLen = symbol.Arguments.Length;
			int patternLen = pattern.Arguments.Length;

			for (int i = 0; i < patternLen; i++) {
				var value = i < argsLen ? symbol.Arguments[i] : Constant.NaN;
				consts = consts.Add(pattern.Arguments[i], value);
			}

			return consts;
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

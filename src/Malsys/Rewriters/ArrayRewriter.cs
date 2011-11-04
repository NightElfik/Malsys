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
	public class ArrayRewriter : IRewriter {

		private Dictionary<string, RewriteRule[]> rewriteRules;
		private VarMap variables;
		private FunMap functions;
		private Random rndGenerator;
		private List<Symbol> input;
		private List<Symbol> output;

		private int inputIndex;

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

			int inputStartIndex = right ? inputIndex + 1 : inputIndex - ctxt.Length;

			if (inputStartIndex < 0 || inputStartIndex + ctxt.Length >= input.Count) {
				return false;
			}

			for (int i = 0; i < ctxt.Length; i++) {
				if (ctxt[i].Name == input[inputStartIndex + i].Name) {
					vars = mapPatternVars(ctxt[i], input[inputStartIndex + i], vars);
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

		#region IRewriter Members

		public void Initialize(Dictionary<string, RewriteRule[]> rrules, VarMap vars, FunMap funs, Random rnd) {
			rewriteRules = rrules;
			variables = vars;
			functions = funs;
			rndGenerator = rnd;
			output = new List<Symbol>();
		}

		public IEnumerable<Symbol> Rewrite(IEnumerable<Symbol> src) {
			input = src.ToList();

			for (inputIndex = 0; inputIndex < input.Count; inputIndex++) {
				VarMap vars;
				RewriteRule rrule;
				if (tryFindRewriteRule(input[inputIndex], out rrule, out vars)) {
					var replac = chooseReplacement(rrule, vars, functions);
					foreach (var s in replac.Replacement) {
						output.Add(s.Evaluate(vars, functions));
					}
				}
				else {
					output.Add(input[inputIndex]);
				}
			}

			return output;
		}

		#endregion
	}
}

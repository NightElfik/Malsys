using System.Collections.Generic;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Evaluated;
using FunMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Evaluated.FunctionEvaledParams>;
using VarMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Evaluated.IValue>;

namespace Malsys.Evaluators {
	public class LsystemEvaluator {

		private BindingsEvaluator bindEvaluator;


		public LsystemEvaluator(BindingsEvaluator bindEval) {
			bindEvaluator = bindEval;
		}

		public LsystemEvaled Evaluate(LsystemEvaledParams lsystem, ImmutableList<IValue> arguments, VarMap vars, FunMap funs) {

			if (lsystem.Parameters.Length < arguments.Length) {
				throw new EvalException("Failed to evaluate L-system `{0}`. It takes only {1} parameters but {2} arguments were given.".Fmt(
					lsystem.BindedName, lsystem.Parameters.Length, arguments.Length));
			}

			// add arguments as new vars
			for (int i = 0; i < lsystem.Parameters.Length; i++) {
				IValue value;
				if (lsystem.Parameters[i].IsOptional) {
					value = i < arguments.Length ? value = arguments[i] : lsystem.Parameters[i].DefaultValue;
				}
				else {
					if (i < arguments.Length) {
						value = arguments[i];
					}
					else {
						throw new EvalException("Failed to evaluate L-system `{0}`. {1}. parameter is not optional and only {2} values given.".Fmt(
							lsystem.BindedName, i + 1, arguments.Length));
					}
				}

				vars = vars.Add(lsystem.Parameters[i].Name, value);
			}

			BindingMaps bindMap = new BindingMaps(BindingType.SymbolList) {
				Variables = vars,
				Functions = funs
			};

			var rewriteRules = evaluateStatements(lsystem.Statements, bindMap);

			return new LsystemEvaled(bindMap.Variables, bindMap.Functions, rewriteRules, lsystem.AstNode);
		}


		private ImmutableList<RewriteRule> evaluateStatements(IEnumerable<ILsystemStatement> statements, BindingMaps bindMap) {

			var rrs = new List<RewriteRule>();

			foreach (var stat in statements) {
				switch (stat.StatementType) {

					case LsystemStatementType.Binding:
						bindEvaluator.Evaluate((Binding)stat, bindMap);
						break;

					case LsystemStatementType.RewriteRule:
						rrs.Add((RewriteRule)stat);
						break;

					default:
						throw new EvalException("Unknown lsystem statement.");
				}
			}

			return new ImmutableList<RewriteRule>(rrs);
		}

	}
}

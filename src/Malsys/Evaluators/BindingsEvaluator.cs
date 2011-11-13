using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Evaluators {
	public class BindingsEvaluator {

		private ExpressionEvaluator exprEvaluator;


		public BindingsEvaluator(ExpressionEvaluator exprEval) {
			exprEvaluator = exprEval;
		}

		public void EvaluateList(ImmutableList<Binding> bindings, BindingMaps data) {
			foreach (var bind in bindings) {
				Evaluate(bind, data);
			}
		}

		public void Evaluate(Binding bind, BindingMaps data) {
			switch (bind.BindingType) {
				case BindingType.Expression:
					if (data.Variables == null || data.Functions == null) {
						throw new EvalException("Unexcpected expression binding.");
					}

					var value = exprEvaluator.Evaluate((IExpression)bind.Value, data.Variables, data.Functions);
					data.Variables = data.Variables.Add(bind.Name, value);
					break;

				case BindingType.Function:
					if (data.Variables == null || data.Functions == null) {
						throw new EvalException("Unexcpected function binding.");
					}

					var bindFun = (Function)bind.Value;
					var funPrms = exprEvaluator.EvaluateOptParams(bindFun.Parameters, data.Variables, data.Functions);
					var fun = new FunctionEvaledParams(funPrms, bindFun.Bindings, bindFun.ReturnExpression, bindFun.AstNode);
					data.Functions = data.Functions.Add(bind.Name, fun);
					break;

				case BindingType.SymbolList:
					if (data.Variables == null || data.Functions == null || data.Symbols == null) {
						throw new EvalException("Unexcpected symbols binding.");
					}

					var symList = exprEvaluator.EvaluateSymbols((SymbolsListExpr)bind.Value, data.Variables, data.Functions);
					data.Symbols = data.Symbols.Add(bind.Name, symList);
					break;

				case BindingType.Lsystem:
					if (data.Variables == null || data.Functions == null || data.Lsystems == null) {
						throw new EvalException("Unexcpected lsystem binding.");
					}

					var bindLsys = (Lsystem)bind.Value;
					var lsysPrms = exprEvaluator.EvaluateOptParams(bindLsys.Parameters, data.Variables, data.Functions);
					var lsys = new LsystemEvaledParams(lsysPrms, bindLsys.Statements, bindLsys.AstNode);
					data.Lsystems = data.Lsystems.Add(bind.Name, lsys);
					break;

				default:
					throw new EvalException("Unknown binding");
			}

		}
	}
}

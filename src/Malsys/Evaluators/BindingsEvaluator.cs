using Malsys.SemanticModel.Compiled;
using FunMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Evaluated.FunctionEvaled>;
using SymMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Evaluated.SymbolsListVal>;
using VarMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Evaluated.IValue>;

namespace Malsys.Evaluators {
	public class BindingsEvaluator {

		private ExpressionEvaluator exprEvaluator;


		public BindingsEvaluator(ExpressionEvaluator exprEval) {
			exprEvaluator = exprEval;
		}


		public void EvaluateList(ImmutableList<Binding> bindings, ref VarMap vars, ref FunMap funs) {

			foreach (var bind in bindings) {
				switch (bind.BindingType) {
					case BindingType.Expression:
						var value = exprEvaluator.Evaluate((IExpression)bind.Value, vars, funs);
						vars = vars.Add(bind.Name, value);
						break;
					case BindingType.Function:
						var fun = exprEvaluator.EvaluateFunction((Function)bind.Value, vars, funs);
						funs = funs.Add(bind.Name, fun);
						break;
					case BindingType.SymbolList:
						throw new EvalException("Unexcpected symbols binding");
					default:
						throw new EvalException("Unknown binding");
				}
			}

		}

		public void EvaluateList(ImmutableList<Binding> bindings, ref VarMap vars, ref FunMap funs, ref SymMap syms) {

			foreach (var bind in bindings) {
				switch (bind.BindingType) {
					case BindingType.Expression:
						var value = exprEvaluator.Evaluate((IExpression)bind.Value, vars, funs);
						vars = vars.Add(bind.Name, value);
						break;
					case BindingType.Function:
						var fun = exprEvaluator.EvaluateFunction((Function)bind.Value, vars, funs);
						funs = funs.Add(bind.Name, fun);
						break;
					case BindingType.SymbolList:
						var symList = exprEvaluator.EvaluateSymbols((SymbolsListExpr)bind.Value, vars, funs);
						syms = syms.Add(bind.Name, symList);
						break;
					default:
						throw new EvalException("Unknown binding");
				}
			}

		}
	}
}

using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Evaluated;
using Microsoft.FSharp.Collections;

namespace Malsys.Evaluators {
	internal class InputEvaluator : IInputEvaluator {

		private ExpressionEvaluator exprEvaluator;
		private IParametersEvaluator paramsEvaluator;


		public InputEvaluator(ExpressionEvaluator exprEval, IParametersEvaluator parametersEvaluator) {

			exprEvaluator = exprEval;
			paramsEvaluator = parametersEvaluator;
		}


		public SemanticModel.Evaluated.InputBlock Evaluate(SemanticModel.Compiled.InputBlock input) {

			var consts = MapModule.Empty<string, IValue>();
			var funs = MapModule.Empty<string, FunctionEvaledParams>();
			var lsys = MapModule.Empty<string, LsystemEvaledParams>();

			foreach (var stat in input.Statements) {
				switch (stat.StatementType) {
					case InputStatementType.Constant:
						var cst = (ConstantDefinition)stat;
						consts = consts.Add(cst.Name, exprEvaluator.Evaluate(cst.Value));
						break;

					case InputStatementType.Function:
						var fun = (Function)stat;
						var funPrms = paramsEvaluator.Evaluate(fun.Parameters, consts, funs);
						funs = funs.Add(fun.Name, new FunctionEvaledParams(fun.Name, funPrms, fun.Statements, fun.AstNode));
						break;

					case InputStatementType.Lsystem:
						var ls = (Lsystem)stat;
						var lsPrms = paramsEvaluator.Evaluate(ls.Parameters, consts, funs);
						lsys = lsys.Add(ls.Name, new LsystemEvaledParams(ls.Name, lsPrms, ls.Statements, ls.AstNode));
						break;

					default:
						throw new EvalException("Unknown input statement type `{0}`.".Fmt(stat.StatementType));
				}
			}

			return new SemanticModel.Evaluated.InputBlock(consts, funs, lsys);
		}

	}
}

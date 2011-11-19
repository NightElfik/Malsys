using System.Collections.Generic;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Evaluated;
using Microsoft.FSharp.Collections;

namespace Malsys.Evaluators {
	public class InputEvaluator {

		private ExpressionEvaluator exprEvaluator;


		public InputEvaluator(ExpressionEvaluator exprEval) {
			exprEvaluator = exprEval;
		}


		public InputBlock Evaluate(IEnumerable<IInputStatement> inStatements) {

			var consts = MapModule.Empty<string, IValue>();
			var funs = MapModule.Empty<string, FunctionEvaledParams>();
			var lsys = MapModule.Empty<string, LsystemEvaledParams>();

			foreach (var stat in inStatements) {
				switch (stat.StatementType) {
					case InputStatementType.Constant:
						var cst = (ConstantDefinition)stat;
						consts = consts.Add(cst.Name, exprEvaluator.Evaluate(cst.Value));
						break;

					case InputStatementType.Function:
						var fun = (Function)stat;
						var funPrms = exprEvaluator.EvaluateOptParams(fun.Parameters, consts, funs);
						funs = funs.Add(fun.Name, new FunctionEvaledParams(fun.Name, funPrms, fun.Statements, fun.AstNode));
						break;

					case InputStatementType.Lsystem:
						var ls = (Lsystem)stat;
						var lsPrms = exprEvaluator.EvaluateOptParams(ls.Parameters, consts, funs);
						lsys = lsys.Add(ls.Name, new LsystemEvaledParams(ls.Name, lsPrms, ls.Statements, ls.AstNode));
						break;

					default:
						throw new EvalException("Unknown input statement type `{0}`.".Fmt(stat.StatementType));
				}
			}

			return new InputBlock(consts, funs, lsys);
		}

	}
}

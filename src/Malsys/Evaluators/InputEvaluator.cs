using Malsys.SemanticModel.Compiled;
using Microsoft.FSharp.Collections;
using System.Collections.Generic;

namespace Malsys.Evaluators {
	internal class InputEvaluator : IInputEvaluator {

		private IExpressionEvaluator exprEvaluator;
		private IParametersEvaluator paramsEvaluator;



		public InputEvaluator(IExpressionEvaluator exprEval, IParametersEvaluator parametersEvaluator) {

			exprEvaluator = exprEval;
			paramsEvaluator = parametersEvaluator;
		}


		public SemanticModel.Evaluated.InputBlock Evaluate(InputBlock input) {

			var consts = MapModule.Empty<string, SemanticModel.Evaluated.IValue>();
			var constsAst = MapModule.Empty<string, Ast.ConstantDefinition>();
			var funs = MapModule.Empty<string, FunctionEvaledParams>();
			var lsys = MapModule.Empty<string, LsystemEvaledParams>();
			var procConfs = MapModule.Empty<string, ProcessConfiguration>();
			var procStats = new List<ProcessStatement>();

			foreach (var stat in input.Statements) {
				switch (stat.StatementType) {
					case InputStatementType.Constant:
						var cst = (ConstantDefinition)stat;
						var constValue = exprEvaluator.Evaluate(cst.Value, consts, funs);
						consts = consts.Add(cst.Name, constValue);
						constsAst = constsAst.Add(cst.Name, cst.AstNode);
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

					case InputStatementType.ProcessConfiguration:
						var config = (ProcessConfiguration)stat;
						procConfs = procConfs.Add(config.Name, config);
						break;

					case InputStatementType.ProcessStatement:
						procStats.Add((ProcessStatement)stat);
						break;

					default:
						throw new EvalException("Unknown input statement type `{0}`.".Fmt(stat.StatementType));
				}
			}

			return new SemanticModel.Evaluated.InputBlock(consts, constsAst, funs, lsys, procConfs, procStats.ToImmutableList(), input.SourceName);
		}

	}
}

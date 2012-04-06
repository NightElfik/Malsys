using System.Collections.Generic;
using System.Diagnostics;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Evaluated;
using Microsoft.FSharp.Collections;

namespace Malsys.Evaluators {
	/// <remarks>
	/// All public members are thread safe if supplied evaluators are thread safe.
	/// </remarks>
	internal class InputEvaluator : IInputEvaluator {

		private readonly IParametersEvaluator paramsEvaluator;


		public InputEvaluator(IParametersEvaluator parametersEvaluator) {
			paramsEvaluator = parametersEvaluator;
		}


		public InputBlockEvaled Evaluate(InputBlock input, IExpressionEvaluatorContext exprEvalCtxt) {

			var lsys = MapModule.Empty<string, LsystemEvaledParams>();
			var procConfs = MapModule.Empty<string, ProcessConfigurationStatement>();
			var procStats = new List<ProcessStatementEvaled>();

			foreach (var stat in input.Statements) {
				switch (stat.StatementType) {
					case InputStatementType.Constant:
						var cst = (ConstantDefinition)stat;
						exprEvalCtxt = exprEvalCtxt.AddVariable(cst.Name, cst.Value, cst.AstNode);
						break;

					case InputStatementType.Function:
						var fun = (Function)stat;
						var funPrms = paramsEvaluator.Evaluate(fun.Parameters, exprEvalCtxt);
						var funData = new FunctionData(fun.Name, funPrms, fun.Statements);
						exprEvalCtxt = exprEvalCtxt.AddFunction(funData);
						break;

					case InputStatementType.Lsystem:
						var ls = (Lsystem)stat;
						var lsPrms = paramsEvaluator.Evaluate(ls.Parameters, exprEvalCtxt);
						lsys = lsys.Add(ls.Name, new LsystemEvaledParams(ls.Name, ls.IsAbstract, lsPrms, ls.BaseLsystems, ls.Statements, ls.AstNode));
						break;

					case InputStatementType.ProcessConfiguration:
						var config = (ProcessConfigurationStatement)stat;
						procConfs = procConfs.Add(config.Name, config);
						break;

					case InputStatementType.ProcessStatement:
						var procStat = (ProcessStatement)stat;
						procStats.Add(new ProcessStatementEvaled(procStat.TargetLsystemName, exprEvalCtxt.EvaluateList(procStat.Arguments),
							procStat.ProcessConfiName, procStat.ComponentAssignments, procStat.AstNode));
						break;

					default:
						Debug.Fail("Unknown input statement type value `{1}`.".Fmt(stat.StatementType.ToString()));
						break;
				}
			}

			return new InputBlockEvaled(exprEvalCtxt, lsys, procConfs, procStats.ToImmutableList(), input.SourceName);
		}

	}
}

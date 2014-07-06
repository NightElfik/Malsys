// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System.Collections.Generic;
using System.Diagnostics;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Evaluated;
using Microsoft.FSharp.Collections;

namespace Malsys.Evaluators {
	/// <remarks>
	/// All public members are thread safe if supplied evaluators are thread safe.
	/// </remarks>
	public class InputEvaluator : IInputEvaluator {

		protected readonly IParametersEvaluator paramsEvaluator;


		public InputEvaluator(IParametersEvaluator parametersEvaluator) {
			paramsEvaluator = parametersEvaluator;
		}


		/// <remarks>
		/// All errors are logged to given logger (do not throws EvalException).
		/// </remarks>
		public InputBlockEvaled Evaluate(InputBlock input, IExpressionEvaluatorContext exprEvalCtxt, IMessageLogger logger) {

			var inBlockEvaled = new InputBlockEvaled() {
				SourceName = input.SourceName
			};

			var lsys = MapModule.Empty<string, LsystemEvaledParams>();
			var procConfs = MapModule.Empty<string, ProcessConfigurationStatement>();

			foreach (var stat in input.Statements) {
				switch (stat.StatementType) {
					case InputStatementType.Constant:
						var cst = (ConstantDefinition)stat;
						try {
							exprEvalCtxt = exprEvalCtxt.AddVariable(cst.Name, cst.Value, cst.AstNode);
						}
						catch (EvalException ex) {
							logger.LogMessage(Message.ConstDefEvalFailed, cst.AstNode.TryGetPosition(), cst.Name, ex.GetFullMessage());
						}
						break;

					case InputStatementType.Function:
						var fun = (Function)stat;
						try {
							var funData = new FunctionData() {
								Name = fun.Name,
								Parameters = paramsEvaluator.Evaluate(fun.Parameters, exprEvalCtxt),
								Statements = fun.Statements
							};
							exprEvalCtxt = exprEvalCtxt.AddFunction(funData);
						}
						catch (EvalException ex) {
							logger.LogMessage(Message.FunDefEvalFailed, fun.AstNode.TryGetPosition(), fun.Name, ex.GetFullMessage());
						}
						break;

					case InputStatementType.Lsystem:
						var ls = (Lsystem)stat;
						try {
							lsys = lsys.Add(ls.Name, new LsystemEvaledParams(ls.AstNode) {
								Name = ls.Name,
								IsAbstract = ls.IsAbstract,
								Parameters = paramsEvaluator.Evaluate(ls.Parameters, exprEvalCtxt),
								BaseLsystems = ls.BaseLsystems,
								Statements = ls.Statements,
							});
						}
						catch (EvalException ex) {
							logger.LogMessage(Message.LsysParamsEvalFailed, ls.AstNode.TryGetPosition(), ls.Name, ex.GetFullMessage());
						}
						break;

					case InputStatementType.ProcessConfiguration:
						var config = (ProcessConfigurationStatement)stat;
						procConfs = procConfs.Add(config.Name, config);
						break;

					case InputStatementType.ProcessStatement:
						var procStat = (ProcessStatement)stat;
						inBlockEvaled.ProcessStatements.Add(new ProcessStatementEvaled(procStat.AstNode) {
							TargetLsystemName = procStat.TargetLsystemName,
							Arguments = exprEvalCtxt.EvaluateList(procStat.Arguments),
							ProcessConfigName = procStat.ProcessConfiName,
							ComponentAssignments = procStat.ComponentAssignments,
							AdditionalLsystemStatements = procStat.AdditionalLsystemStatements,
						});
						break;

					default:
						Debug.Fail("Unknown input statement type value `{1}`.".Fmt(stat.StatementType.ToString()));
						break;
				}
			}

			inBlockEvaled.ExpressionEvaluatorContext = exprEvalCtxt;
			inBlockEvaled.Lsystems = lsys;
			inBlockEvaled.ProcessConfigurations = procConfs;
			return inBlockEvaled;
		}

		public enum Message {

			[Message(MessageType.Error, "Failed to evaluate constant definition `{0}`. {1}")]
			ConstDefEvalFailed,
			[Message(MessageType.Error, "Failed to evaluate default value of parameter of function definition `{0}`. {1}")]
			FunDefEvalFailed,
			[Message(MessageType.Error, "Failed to evaluate default value of parameter of L-system definition `{0}`. {1}")]
			LsysParamsEvalFailed,

		}
	}
}

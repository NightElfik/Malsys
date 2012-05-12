﻿/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
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

			var lsys = MapModule.Empty<string, LsystemEvaledParams>();
			var procConfs = MapModule.Empty<string, ProcessConfigurationStatement>();
			var procStats = new List<ProcessStatementEvaled>();

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
							var funPrms = paramsEvaluator.Evaluate(fun.Parameters, exprEvalCtxt);
							var funData = new FunctionData(fun.Name, funPrms, fun.Statements);
							exprEvalCtxt = exprEvalCtxt.AddFunction(funData);
						}
						catch (EvalException ex) {
							logger.LogMessage(Message.FunDefEvalFailed, fun.AstNode.TryGetPosition(), fun.Name, ex.GetFullMessage());
						}
						break;

					case InputStatementType.Lsystem:
						var ls = (Lsystem)stat;
						try {
							var lsPrms = paramsEvaluator.Evaluate(ls.Parameters, exprEvalCtxt);
							lsys = lsys.Add(ls.Name, new LsystemEvaledParams(ls.Name, ls.IsAbstract, lsPrms, ls.BaseLsystems, ls.Statements, ls.AstNode));
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
						procStats.Add(new ProcessStatementEvaled(procStat.TargetLsystemName, exprEvalCtxt.EvaluateList(procStat.Arguments),
							procStat.ProcessConfiName, procStat.ComponentAssignments, procStat.AdditionalLsystemStatements, procStat.AstNode));
						break;

					default:
						Debug.Fail("Unknown input statement type value `{1}`.".Fmt(stat.StatementType.ToString()));
						break;
				}
			}

			return new InputBlockEvaled(exprEvalCtxt, lsys, procConfs, procStats.ToImmutableList(), input.SourceName);
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

// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Evaluated;
using Microsoft.FSharp.Collections;

namespace Malsys.Evaluators {
	/// <remarks>
	/// All public members are thread safe if supplied evaluators are thread safe.
	/// </remarks>
	public class LsystemEvaluator : ILsystemEvaluator {

		protected readonly IParametersEvaluator paramsEvaluator;
		protected readonly ISymbolEvaluator symbolEvaluator;


		public LsystemEvaluator(IParametersEvaluator parametersEvaluator, ISymbolEvaluator iSymbolEvaluator) {
			paramsEvaluator = parametersEvaluator;
			symbolEvaluator = iSymbolEvaluator;
		}


		/// <remarks>
		/// All errors are logged to given logger (do not throws EvalException).
		/// </remarks>
		public LsystemEvaled Evaluate(LsystemEvaledParams lsystem, IList<IValue> arguments, IExpressionEvaluatorContext exprEvalCtxt,
				IBaseLsystemResolver baseResolver, IMessageLogger logger) {

			return evaluate(lsystem, arguments, exprEvalCtxt, baseResolver, new List<LsystemEvaledParams>(), logger);

		}

		/// <remarks>
		/// All errors are logged to given logger (do not throws EvalException).
		/// </remarks>
		public LsystemEvaled EvaluateAdditionalStatements(LsystemEvaled lsystem, IEnumerable<ILsystemStatement> additionalStatements, IMessageLogger logger) {

			var exprEvalCtxt = lsystem.ExpressionEvaluatorContext;
			var valAssigns = lsystem.ComponentValuesAssigns;
			var symAssigns = lsystem.ComponentSymbolsAssigns;
			var symsInt = lsystem.SymbolsInterpretation;
			var rRules = new List<RewriteRule>();

			foreach (var stat in additionalStatements) {
				evaluateStatement(stat, ref exprEvalCtxt, ref valAssigns, ref symAssigns, ref symsInt, rRules, lsystem.Name, logger);
			}

			rRules.AddRange(lsystem.RewriteRules);

			return new LsystemEvaled(lsystem.Name, lsystem.IsAbstract, lsystem.BaseLsystems, exprEvalCtxt,
				valAssigns, symAssigns, symsInt, rRules.ToImmutableList(), lsystem.AstNode);
		}

		/// <remarks>
		/// All errors are logged to given logger (do not throws EvalException).
		/// </remarks>
		private LsystemEvaled evaluate(LsystemEvaledParams lsystem, IList<IValue> arguments, IExpressionEvaluatorContext exprEvalCtxt,
				IBaseLsystemResolver baseResolver, List<LsystemEvaledParams> derivedLsystems, IMessageLogger logger) {


			if (lsystem.Parameters.Length < arguments.Count) {
				logger.LogMessage(Message.TooManyArgs, lsystem.Name, lsystem.Parameters.Length, arguments.Count);
			}

			var originalExprEvalCtxt = exprEvalCtxt;  // save original context for evaluation of all base L-systems
			for (int i = 0; i < lsystem.Parameters.Length; i++) {
				IValue value;
				if (lsystem.Parameters[i].IsOptional) {
					value = i < arguments.Count ? value = arguments[i] : lsystem.Parameters[i].DefaultValue;
				}
				else {
					if (i < arguments.Count) {
						value = arguments[i];
					}
					else {
						logger.LogMessage(Message.NotEnoughArgs, lsystem.Name, i + 1, arguments.Count);
						return null;
					}
				}

				exprEvalCtxt = exprEvalCtxt.AddVariable(lsystem.Parameters[i].Name, value, lsystem.Parameters[i].AstNode);
			}

			var valAssigns = MapModule.Empty<string, IValue>();
			var symAssigns = MapModule.Empty<string, ImmutableList<Symbol<IValue>>>();
			var symsInt = MapModule.Empty<string, SymbolInterpretationEvaled>();
			var derivedRrRules = new List<RewriteRule>();

			// evaluate base L-systems
			var baseLsystems = new LsystemEvaled[lsystem.BaseLsystems.Length];
			derivedLsystems.Add(lsystem);
			var argumentsExprEvalCtxt = exprEvalCtxt;  // save arguments context for evaluation of all base L-systems

			for (int i = 0; i < baseLsystems.Length; i++) {
				ImmutableList<IValue> args;
				try {
					args = argumentsExprEvalCtxt.EvaluateList(lsystem.BaseLsystems[i].Arguments);
				}
				catch (EvalException ex) {
					logger.LogMessage(Message.BaseArgsEvalFailed, baseLsystems[i].AstNode.TryGetPosition(), lsystem.BaseLsystems[i].Name, lsystem.Name, ex.GetFullMessage());
					return null;
				}
				var baseLsystemCompiled = baseResolver.Resolve(lsystem.BaseLsystems[i].Name, logger);
				if (baseLsystemCompiled == null) {
					logger.LogMessage(Message.BaseLsestemNotDefinded, lsystem.BaseLsystems[i].Name, lsystem.Name);
					return null;
				}
				if (derivedLsystems.Contains(baseLsystemCompiled)) {
					logger.LogMessage(Message.InheritanceCycle, lsystem.Name);
					return null;
				}
				var baseLsystem = evaluate(baseLsystemCompiled, args, originalExprEvalCtxt, baseResolver, derivedLsystems, logger);
				if (baseLsystem == null) {
					return null;
				}
				baseLsystems[i] = baseLsystem;

				// merge current L-system with base L-system
				valAssigns = valAssigns.AddRange(baseLsystem.ComponentValuesAssigns);
				symAssigns = symAssigns.AddRange(baseLsystem.ComponentSymbolsAssigns);
				symsInt = symsInt.AddRange(baseLsystem.SymbolsInterpretation);
				exprEvalCtxt = exprEvalCtxt.MergeWith(baseLsystem.ExpressionEvaluatorContext);
				// add rewrite rules in reverse order
				var newRrList = baseLsystem.RewriteRules.ToList();
				newRrList.AddRange(derivedRrRules);
				derivedRrRules = newRrList;
			}

			var baseLsystemsImm = new ImmutableList<LsystemEvaled>(baseLsystems, true);
			var rRules = new List<RewriteRule>();

			// statements evaluation
			foreach (var stat in lsystem.Statements) {
				evaluateStatement(stat, ref exprEvalCtxt, ref valAssigns, ref symAssigns, ref symsInt, rRules, lsystem.Name, logger);
			}

			rRules.AddRange(derivedRrRules);

			return new LsystemEvaled(lsystem.Name, lsystem.IsAbstract, baseLsystemsImm, exprEvalCtxt,
				valAssigns, symAssigns, symsInt, rRules.ToImmutableList(), lsystem.AstNode);
		}


		/// <remarks>
		/// All errors are logged to given logger (do not throws EvalException).
		/// </remarks>
		private void evaluateStatement(ILsystemStatement statement, ref IExpressionEvaluatorContext exprEvalCtxt, ref FSharpMap<string, IValue> valAssigns,
				ref FSharpMap<string, ImmutableList<Symbol<IValue>>> symAssigns, ref FSharpMap<string, SymbolInterpretationEvaled> symsInt, List<RewriteRule> rRules,
				string lsystemName, IMessageLogger logger) {

			switch (statement.StatementType) {

				case LsystemStatementType.Constant:
					var cst = (ConstantDefinition)statement;
					try {
						if (cst.IsComponentAssign) {
							valAssigns = valAssigns.Add(cst.Name, exprEvalCtxt.Evaluate(cst.Value));
						}
						else {
							exprEvalCtxt = exprEvalCtxt.AddVariable(cst.Name, cst.Value, cst.AstNode);
						}
					}
					catch (EvalException ex) {
						logger.LogMessage(Message.ConstDefEvalFailed, cst.AstNode.TryGetPosition(), cst.Name, lsystemName, ex.GetFullMessage());
					}
					break;

				case LsystemStatementType.Function:
					var fun = (Function)statement;
					try {
						var funPrms = paramsEvaluator.Evaluate(fun.Parameters, exprEvalCtxt);
						var funData = new FunctionData(fun.Name, funPrms, fun.Statements);
						exprEvalCtxt = exprEvalCtxt.AddFunction(funData);
					}
					catch (EvalException ex) {
						logger.LogMessage(Message.FunDefEvalFailed, fun.AstNode.TryGetPosition(), fun.Name, lsystemName, ex.GetFullMessage());
					}
					break;

				case LsystemStatementType.SymbolsConstant:
					var symDef = (SymbolsConstDefinition)statement;
					try {
						symAssigns = symAssigns.Add(symDef.Name, symbolEvaluator.EvaluateList(symDef.Symbols, exprEvalCtxt));
					}
					catch (EvalException ex) {
						logger.LogMessage(Message.SymbolsDefEvalFailed, symDef.AstNode.TryGetPosition(), symDef.Name, lsystemName, ex.GetFullMessage());
					}
					break;

				case LsystemStatementType.RewriteRule:
					rRules.Add((RewriteRule)statement);
					break;

				case LsystemStatementType.SymbolsInterpretation:
					var symInt = (SymbolsInterpretation)statement;
					try {
						var symIntPrms = paramsEvaluator.Evaluate(symInt.Parameters, exprEvalCtxt);
						foreach (var sym in symInt.Symbols) {
							symsInt = symsInt.Add(sym.Name, new SymbolInterpretationEvaled(sym.Name, symIntPrms, symInt.InstructionName,
								symInt.InstructionParameters, symInt.InstructionIsLsystemName, symInt.LsystemConfigName, symInt.AstNode));
						}
					}
					catch (EvalException ex) {
						logger.LogMessage(Message.SymIntDefParamsEvalFailed, symInt.AstNode.TryGetPosition(), lsystemName, ex.GetFullMessage());
					}
					break;

				default:
					Debug.Fail("Unknown L-system statement type value `{1}`.".Fmt(statement.StatementType.ToString()));
					break;

			}
		}

		public enum Message {

			[Message(MessageType.Error, "Not enough arguments supplied to evaluation of L-system `{0}`."
				+ " {1}. parameter is not optional and only {2} values given.")]
			NotEnoughArgs,
			[Message(MessageType.Error, "Cycle in inheritance found while evaluating L-system `{0}`.")]
			InheritanceCycle,
			[Message(MessageType.Error, "Base L-system `{0}` of L-system `{1}` is not defined.")]
			BaseLsestemNotDefinded,

			[Message(MessageType.Error, "Failed to evaluate constant definition `{0}` in L-system `{1}`. {2}")]
			ConstDefEvalFailed,
			[Message(MessageType.Error, "Failed to evaluate default value of parameter of function definition `{0}` in L-system `{1}`. {2}")]
			FunDefEvalFailed,
			[Message(MessageType.Error, "Failed to evaluate symbols definition `{0}` in L-system `{1}`. {2}")]
			SymbolsDefEvalFailed,
			[Message(MessageType.Error, "Failed to evaluate default value of parameter of symbols interpretation definition in L-system `{0}`. {1}")]
			SymIntDefParamsEvalFailed,
			[Message(MessageType.Error, "Failed to evaluate arguments of base L-system `{0}` of L-system `{1}`. {2}")]
			BaseArgsEvalFailed,

			[Message(MessageType.Warning, "L-system `{0}` takes only {1} parameters but {2} arguments were given.")]
			TooManyArgs,

		}

	}
}

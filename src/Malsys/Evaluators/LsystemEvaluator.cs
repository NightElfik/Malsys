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

		/// <summary>
		/// Evaluates additional statements placing them after current symbols.
		/// New rewrite rules are placed at the beginning of the list (to be able to override current).
		/// </summary>
		/// <remarks>
		/// All errors are logged to given logger (do not throws EvalException).
		/// </remarks>
		public void EvaluateAdditionalStatements(LsystemEvaled lsystem,
				IEnumerable<ILsystemStatement> additionalStatements, IMessageLogger logger) {

			List<RewriteRule> rewriteRules = new List<RewriteRule>();

			foreach (var stat in additionalStatements) {
				evaluateStatement(stat,
					ref lsystem.ExpressionEvaluatorContext,
					ref lsystem.ComponentValuesAssigns,
					ref lsystem.ComponentSymbolsAssigns,
					ref lsystem.SymbolsInterpretation,
					rewriteRules,
					lsystem.Name,
					logger);
			}

			rewriteRules.AddRange(lsystem.RewriteRules);
			lsystem.RewriteRules = rewriteRules;
		}

		/// <remarks>
		/// All errors are logged to given logger (do not throws EvalException).
		/// </remarks>
		private LsystemEvaled evaluate(LsystemEvaledParams lsystem, IList<IValue> arguments, IExpressionEvaluatorContext exprEvalCtxt,
				IBaseLsystemResolver baseResolver, List<LsystemEvaledParams> derivedLsystems, IMessageLogger logger) {

			var resultLsystem = new LsystemEvaled(lsystem.AstNode) {
				Name = lsystem.Name,
				IsAbstract = lsystem.IsAbstract,
				BaseLsystems = new List<LsystemEvaled>(),
				ExpressionEvaluatorContext = exprEvalCtxt,
				ComponentValuesAssigns = MapModule.Empty<string, IValue>(),
				ComponentSymbolsAssigns = MapModule.Empty<string, List<Symbol<IValue>>>(),
				SymbolsInterpretation = MapModule.Empty<string, SymbolInterpretationEvaled>(),
				RewriteRules = new List<RewriteRule>(),
			};


			if (lsystem.Parameters.Count < arguments.Count) {
				logger.LogMessage(Message.TooManyArgs, lsystem.Name, lsystem.Parameters.Count, arguments.Count);
			}

			for (int i = 0; i < lsystem.Parameters.Count; i++) {
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

				resultLsystem.ExpressionEvaluatorContext = resultLsystem.ExpressionEvaluatorContext.AddVariable(
					lsystem.Parameters[i].Name, value, lsystem.Parameters[i].AstNode);
			}

			// List of derived L-systems to detect potential loop in inheritance hierarchy.
			var derivedRrRules = new List<RewriteRule>();
			derivedLsystems.Add(lsystem);

			// Evaluate base L-systems.
			var argumentsExprEvalCtxt = resultLsystem.ExpressionEvaluatorContext;  // Save arguments context for evaluation of all base L-systems.

			for (int i = 0; i < lsystem.BaseLsystems.Count; i++) {
				List<IValue> args;
				try {
					args = argumentsExprEvalCtxt.EvaluateList(lsystem.BaseLsystems[i].Arguments);
				}
				catch (EvalException ex) {
					logger.LogMessage(Message.BaseArgsEvalFailed, lsystem.BaseLsystems[i].AstNode.TryGetPosition(),
						lsystem.BaseLsystems[i].Name, lsystem.Name, ex.GetFullMessage());
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

				var baseLsystem = evaluate(baseLsystemCompiled, args, exprEvalCtxt,
					baseResolver, derivedLsystems, logger);
				if (baseLsystem == null) {
					return null;
				}

				resultLsystem.BaseLsystems.Add(baseLsystem);

				// Merge current L-system with base L-system.
				resultLsystem.ComponentValuesAssigns = resultLsystem.ComponentValuesAssigns.AddRange(baseLsystem.ComponentValuesAssigns);
				resultLsystem.ComponentSymbolsAssigns = resultLsystem.ComponentSymbolsAssigns.AddRange(baseLsystem.ComponentSymbolsAssigns);
				resultLsystem.SymbolsInterpretation = resultLsystem.SymbolsInterpretation.AddRange(baseLsystem.SymbolsInterpretation);
				resultLsystem.ExpressionEvaluatorContext = resultLsystem.ExpressionEvaluatorContext.MergeWith(baseLsystem.ExpressionEvaluatorContext);

				// Save rewrite rules in reverse order to add them after main rules.
				var newRrList = baseLsystem.RewriteRules.ToList();
				newRrList.AddRange(derivedRrRules);
				derivedRrRules = newRrList;
			}


			// Statements evaluation.
			foreach (var stat in lsystem.Statements) {
				evaluateStatement(stat,
					ref resultLsystem.ExpressionEvaluatorContext,
					ref resultLsystem.ComponentValuesAssigns,
					ref resultLsystem.ComponentSymbolsAssigns,
					ref resultLsystem.SymbolsInterpretation,
					resultLsystem.RewriteRules,
					resultLsystem.Name,
					logger);
			}

			resultLsystem.RewriteRules.AddRange(derivedRrRules);

			return resultLsystem;
		}


		/// <remarks>
		/// All errors are logged to given logger (do not throws EvalException).
		/// </remarks>
		private void evaluateStatement(ILsystemStatement statement, ref IExpressionEvaluatorContext exprEvalCtxt,
				ref FSharpMap<string, IValue> valAssigns, ref FSharpMap<string, List<Symbol<IValue>>> symAssigns,
				ref FSharpMap<string, SymbolInterpretationEvaled> symsInt, List<RewriteRule> rRules,
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
						var funData = new FunctionData() {
							Name = fun.Name,
							Parameters = funPrms,
							Statements = fun.Statements,
						};
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
						foreach (var sym in symInt.Symbols) {
							symsInt = symsInt.Add(sym.Name, new SymbolInterpretationEvaled(symInt.AstNode) {
								Symbol = sym.Name,
								Parameters = paramsEvaluator.Evaluate(symInt.Parameters, exprEvalCtxt),
								InstructionName = symInt.InstructionName,
								InstructionParameters = symInt.InstructionParameters,
								InstructionIsLsystemName = symInt.InstructionIsLsystemName,
								LsystemConfigName = symInt.LsystemConfigName,
							});
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

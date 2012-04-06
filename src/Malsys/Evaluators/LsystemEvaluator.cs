using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Evaluated;
using Microsoft.FSharp.Collections;

namespace Malsys.Evaluators {
	/// <remarks>
	/// All public members are thread safe if supplied evaluators are thread safe.
	/// </remarks>
	internal class LsystemEvaluator : ILsystemEvaluator {

		private readonly IParametersEvaluator paramsEvaluator;
		private readonly ISymbolEvaluator symbolEvaluator;


		public LsystemEvaluator(IParametersEvaluator parametersEvaluator, ISymbolEvaluator iSymbolEvaluator) {
			paramsEvaluator = parametersEvaluator;
			symbolEvaluator = iSymbolEvaluator;
		}


		/// <remarks>
		/// May throw EvalException on evaluation error while evaluating some expression.
		/// </remarks>
		public LsystemEvaled Evaluate(LsystemEvaledParams lsystem, IList<IValue> arguments, IExpressionEvaluatorContext exprEvalCtxt,
				IBaseLsystemResolver baseResolver, IMessageLogger logger) {

			return evaluate(lsystem, arguments, exprEvalCtxt, baseResolver, new List<LsystemEvaledParams>(), logger);

		}

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
						logger.LogMessage(Message.TooManyArgs, lsystem.AstNode.NameId.Name, i + 1, arguments.Count);
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
				var args = argumentsExprEvalCtxt.EvaluateList(lsystem.BaseLsystems[i].Arguments);
				var baseLsystemCompiled = baseResolver.Resolve(lsystem.BaseLsystems[i].Name, logger);
				if (baseLsystemCompiled == null) {
					return null;
				}
				if (derivedLsystems.Contains(baseLsystemCompiled)) {
					logger.LogMessage(Message.InherenceCycle, lsystem.Name);
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
				switch (stat.StatementType) {

					case LsystemStatementType.Constant:
						var cst = (ConstantDefinition)stat;
						if (cst.IsComponentAssign) {
							valAssigns = valAssigns.Add(cst.Name, exprEvalCtxt.Evaluate(cst.Value));
						}
						else {
							exprEvalCtxt = exprEvalCtxt.AddVariable(cst.Name, cst.Value, cst.AstNode);
						}
						break;

					case LsystemStatementType.Function:
						var fun = (Function)stat;
						var funPrms = paramsEvaluator.Evaluate(fun.Parameters, exprEvalCtxt);
						var funData = new FunctionData(fun.Name, funPrms, fun.Statements);
						exprEvalCtxt = exprEvalCtxt.AddFunction(funData);
						break;

					case LsystemStatementType.SymbolsConstant:
						var symDef = (SymbolsConstDefinition)stat;
						symAssigns = symAssigns.Add(symDef.Name, symbolEvaluator.EvaluateList(symDef.Symbols, exprEvalCtxt));
						break;

					case LsystemStatementType.RewriteRule:
						rRules.Add((RewriteRule)stat);
						break;

					case LsystemStatementType.SymbolsInterpretation:
						var symInt = (SymbolsInterpretation)stat;
						var symIntPrms = paramsEvaluator.Evaluate(symInt.Parameters, exprEvalCtxt);
						foreach (var sym in symInt.Symbols) {
							symsInt = symsInt.Add(sym.Name, new SymbolInterpretationEvaled(sym.Name, symIntPrms, symInt.InstructionName,
								symInt.InstructionParameters, symInt.InstructionIsLsystemName, symInt.LsystemConfigName, symInt.AstNode));
						}
						break;

					default:
						Debug.Fail("Unknown L-system statement type value `{1}`.".Fmt(stat.StatementType.ToString()));
						break;

				}
			}

			rRules.AddRange(derivedRrRules);

			return new LsystemEvaled(lsystem.Name, lsystem.IsAbstract, baseLsystemsImm, exprEvalCtxt,
				valAssigns, symAssigns, symsInt, rRules.ToImmutableList(), lsystem.AstNode);
		}


		public enum Message {

			[Message(MessageType.Error, "Not enough arguments supplied to evaluation of L-system `{0}`."
				+ " {1}. parameter is not optional and only {2} values given.")]
			NotEnoughArgs,
			[Message(MessageType.Error, "Cycle in inherence found while evaluating L-system `{0}`.")]
			InherenceCycle,

			[Message(MessageType.Warning, "L-system `{0}` takes only {1} parameters but {2} arguments were given.")]
			TooManyArgs,

		}


	}
}

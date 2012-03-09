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


		public LsystemEvaled Evaluate(LsystemEvaledParams lsystem, IList<IValue> arguments, IExpressionEvaluatorContext exprEvalCtxt) {

			if (lsystem.Parameters.Length < arguments.Count) {
				throw new EvalException("Failed to evaluate L-system `{0}`. It takes only {1} parameters but {2} arguments were given."
					.Fmt(lsystem.AstNode.NameId.Name, lsystem.Parameters.Length, arguments.Count));
			}

			// add arguments as new constants
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
						throw new EvalException("Failed to evaluate L-system `{0}`. {1}. parameter is not optional and only {2} values given."
							.Fmt(lsystem.AstNode.NameId.Name, i + 1, arguments.Count));
					}
				}

				exprEvalCtxt = exprEvalCtxt.AddVariable(lsystem.Parameters[i].Name, value, lsystem.Parameters[i].AstNode);
			}

			var valAssigns = MapModule.Empty<string, IValue>();
			var symAssigns = MapModule.Empty<string, ImmutableList<Symbol<IValue>>>();
			var symsInt = MapModule.Empty<string, SymbolInterpretationEvaled>();
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
							if (symsInt.ContainsKey(sym.Name)) {
								throw new EvalException("More than one interpretation method defined for symbol `{0}` (`{1}` and `{2}`)."
									.Fmt(sym.Name, symInt.InstructionName, symsInt[sym.Name].InstructionName));
							}
							symsInt = symsInt.Add(sym.Name, new SymbolInterpretationEvaled(sym.Name, symIntPrms,
								symInt.InstructionName, symInt.InstructionParameters, symInt.AstNode));
						}
						break;

					default:
						Debug.Fail("Unknown L-system statement type value `{1}`.".Fmt(stat.StatementType.ToString()));
						break;

				}
			}

			return new LsystemEvaled(lsystem.Name, exprEvalCtxt, valAssigns, symAssigns, symsInt, rRules.ToImmutableList(), lsystem.AstNode);
		}

	}
}

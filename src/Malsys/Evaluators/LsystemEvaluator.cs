using System.Collections.Generic;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Evaluated;
using Microsoft.FSharp.Collections;
using ConstsMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Evaluated.IValue>;
using FunsMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Compiled.FunctionEvaledParams>;
using SymIntMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Symbol<Malsys.SemanticModel.Evaluated.IValue>>;
using SymListMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.ImmutableList<Malsys.SemanticModel.Symbol<Malsys.SemanticModel.Evaluated.IValue>>>;

namespace Malsys.Evaluators {
	internal class LsystemEvaluator : ILsystemEvaluator {

		private IExpressionEvaluator exprEvaluator;
		private IParametersEvaluator paramsEvaluator;
		private ISymbolEvaluator symbolEvaluator;


		public LsystemEvaluator(IExpressionEvaluator exprEval, IParametersEvaluator parametersEvaluator, ISymbolEvaluator iSymbolEvaluator) {

			exprEvaluator = exprEval;
			paramsEvaluator = parametersEvaluator;
			symbolEvaluator = iSymbolEvaluator;
		}


		public LsystemEvaled Evaluate(LsystemEvaledParams lsystem, IList<IValue> arguments, ConstsMap consts, FunsMap funs) {

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

				consts = consts.Add(lsystem.Parameters[i].Name, value);
			}

			var symDefs = MapModule.Empty<string, ImmutableList<Symbol<IValue>>>();
			var symsInt = MapModule.Empty<string, Symbol<IValue>>();

			// statements evaluation
			var rRules = new List<RewriteRule>();
			var processStats = new List<ProcessStatement>();

			foreach (var stat in lsystem.Statements) {
				switch (stat.StatementType) {
					case LsystemStatementType.Constant:
						var cst = (ConstantDefinition)stat;
						consts = consts.Add(cst.Name, exprEvaluator.Evaluate(cst.Value));
						break;

					case LsystemStatementType.Function:
						var fun = (Function)stat;
						var funPrms = paramsEvaluator.Evaluate(fun.Parameters, consts, funs);
						funs = funs.Add(fun.Name, new FunctionEvaledParams(fun.Name, funPrms, fun.Statements, fun.AstNode));
						break;

					case LsystemStatementType.SymbolsConstant:
						var symDef = (SymbolsConstDefinition)stat;
						symDefs = symDefs.Add(symDef.Name, symbolEvaluator.EvaluateList(symDef.Symbols, consts, funs));
						break;

					case LsystemStatementType.RewriteRule:
						rRules.Add((RewriteRule)stat);
						break;

					case LsystemStatementType.SymbolsInterpretation:
						var symInt = (SymbolsInterpretation)stat;
						var prms = exprEvaluator.EvaluateList(symInt.DefaultParameters);
						foreach (var sym in symInt.Symbols) {
							symsInt = symsInt.Add(sym.Name, new Symbol<IValue>(symInt.InstructionName, prms));
						}
						break;

					case LsystemStatementType.ProcessStatement:
						processStats.Add((ProcessStatement)stat);
						break;

					default:
						break;
				}
			}

			return new LsystemEvaled(lsystem.Name, consts, funs, symDefs, symsInt,
				rRules.ToImmutableList(), processStats.ToImmutableList(), lsystem.AstNode);
		}

	}
}

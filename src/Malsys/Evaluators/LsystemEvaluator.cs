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
	public class LsystemEvaluator {

		private ExpressionEvaluator exprEvaluator;


		public LsystemEvaluator(ExpressionEvaluator exprEval) {
			exprEvaluator = exprEval;
		}

		public LsystemEvaled Evaluate(LsystemEvaledParams lsystem, ImmutableList<IValue> arguments,
				ConstsMap consts, FunsMap funs) {

			if (lsystem.Parameters.Length < arguments.Length) {
				throw new EvalException("Failed to evaluate L-system `{0}`. It takes only {1} parameters but {2} arguments were given."
					.Fmt(lsystem.AstNode.NameId.Name, lsystem.Parameters.Length, arguments.Length));
			}

			// add arguments as new consts
			for (int i = 0; i < lsystem.Parameters.Length; i++) {
				IValue value;
				if (lsystem.Parameters[i].IsOptional) {
					value = i < arguments.Length ? value = arguments[i] : lsystem.Parameters[i].DefaultValue;
				}
				else {
					if (i < arguments.Length) {
						value = arguments[i];
					}
					else {
						throw new EvalException("Failed to evaluate L-system `{0}`. {1}. parameter is not optional and only {2} values given."
							.Fmt(lsystem.AstNode.NameId.Name, i + 1, arguments.Length));
					}
				}

				consts = consts.Add(lsystem.Parameters[i].Name, value);
			}

			var symDefs = MapModule.Empty<string, ImmutableList<Symbol<IValue>>>();
			var symsInt = MapModule.Empty<string, Symbol<IValue>>();

			var rewriteRules = evaluateStatements(lsystem.Statements, ref consts, ref funs, ref symDefs, ref symsInt);

			return new LsystemEvaled(lsystem.Name, consts, funs, symDefs, symsInt, rewriteRules, lsystem.AstNode);
		}


		private ImmutableList<RewriteRule> evaluateStatements(IEnumerable<ILsystemStatement> statements,
				ref ConstsMap consts, ref FunsMap funs, ref SymListMap symDefs, ref SymIntMap symsInt) {

			var rrs = new List<RewriteRule>();

			foreach (var stat in statements) {
				switch (stat.StatementType) {
					case LsystemStatementType.Constant:
						var cst = (ConstantDefinition)stat;
						consts = consts.Add(cst.Name, exprEvaluator.Evaluate(cst.Value));
						break;

					case LsystemStatementType.Function:
						var fun = (Function)stat;
						var funPrms = exprEvaluator.EvaluateOptParams(fun.Parameters, consts, funs);
						funs = funs.Add(fun.Name, new FunctionEvaledParams(fun.Name, funPrms, fun.Statements, fun.AstNode));
						break;

					case LsystemStatementType.SymbolsConstant:
						var symDef = (SymbolsConstDefinition)stat;
						symDefs = symDefs.Add(symDef.Name, exprEvaluator.EvaluateSymbols(symDef.Symbols, consts, funs));
						break;

					case LsystemStatementType.RewriteRule:
						rrs.Add((RewriteRule)stat);
						break;

					case LsystemStatementType.SymbolsInterpretation:
						var symInt = (SymbolsInterpretation)stat;
						var prms = exprEvaluator.Evaluate(symInt.DefaultParameters);
						foreach (var sym in symInt.Symbols) {
							symsInt = symsInt.Add(sym.Name, new Symbol<IValue>(symInt.InstructionName, prms));
						}
						break;

					default:
						break;
				}
			}

			return new ImmutableList<RewriteRule>(rrs);
		}

	}
}

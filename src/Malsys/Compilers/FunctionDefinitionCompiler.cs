using System;
using System.Diagnostics;
using Malsys.Expressions;

namespace Malsys.Compilers {
	public static class FunctionDefinitionCompiler {

		/// <summary>
		/// Thread safe.
		/// </summary>
		public static FunctionDefinition CompileFailSafe(this Ast.FunctionDefinition funDef, MessagesCollection msgs) {

			var prms = funDef.Parameters.CompileParametersFailSafe(msgs);
			var varDefs = funDef.LocalVarDefs.CompileFailSafe(msgs);
			var retExpr = funDef.ReturnExpression.CompileFailSafe(msgs);

			return new FunctionDefinition(funDef.NameId.Name, prms, varDefs, retExpr);
		}

		/// <summary>
		/// Thread safe.
		/// </summary>
		public static ImmutableList<OptionalParameter> CompileParametersFailSafe(this ImmutableList<Ast.OptionalParameter> parameters, MessagesCollection msgs) {

			int parametersCount = parameters.Count;
			bool wasOptional = false;
			var result = new OptionalParameter[parametersCount];

			for (int i = 0; i < parametersCount; i++) {

				var prm = parameters[i];
				var compiledParam = prm.CompileParameterFailSafe(msgs);
				result[i] = compiledParam;

				if (compiledParam.IsOptional) {
					wasOptional = true;
				}
				else {
					if (wasOptional) {
						msgs.AddError("Mandatory parameters have to be before all optional parameters, but mandatory parameter `{0}` is after optional.".Fmt(prm.NameId.Name),
							prm.Position);
					}
				}
			}

			// check wether parameters names are unique
			foreach (var indices in result.GetEqualValuesIndices((l, r) => { return l.Name.Equals(r.Name); })) {
				msgs.AddError("{0}. and {1}. parameter have same name `{2}`.".Fmt(indices.Item1 + 1, indices.Item2 + 1, result[indices.Item1].Name),
					parameters[indices.Item1].Position, parameters[indices.Item2].Position);
			}

			return new ImmutableList<OptionalParameter>(result, true);
		}

		public static OptionalParameter CompileParameterFailSafe(this Ast.OptionalParameter parameter, MessagesCollection msgs) {

			if (parameter.IsOptional) {
				var value = parameter.OptionalValue.CompileFailSafe(msgs);
				IValue evalValue;

				try {
					evalValue = ExpressionEvaluator.Evaluate(value);
				}
				catch (EvalException ex) {
					msgs.AddError("Faled to evaluate default value of parameter `{0}`. {1}".Fmt(parameter.NameId.Name, ex.GetWholeMessage()),
						parameter.OptionalValue.Position);
					evalValue = Constant.NaN;
				}

				return new OptionalParameter(parameter.NameId.Name, evalValue);
			}

			else {
				return new OptionalParameter(parameter.NameId.Name);
			}
		}
	}
}

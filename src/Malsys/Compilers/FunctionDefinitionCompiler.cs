using System;
using System.Diagnostics;
using Malsys.Expressions;

namespace Malsys.Compilers {
	public static class FunctionDefinitionCompiler {

		/// <summary>
		/// Thread safe.
		/// </summary>
		public static FunctionDefinition CompileFailSafe(Ast.FunctionDefinition funDef, MessagesCollection msgs) {

			var paramsTuple = CompileParametersFailSafe(funDef.Parameters, msgs);

			var body = ExpressionCompiler.CompileRichFailSafe(funDef, msgs);

			return new FunctionDefinition(funDef.NameId.Name, paramsTuple.Item1, paramsTuple.Item2, body.VariableDefinitions, body.Expression);
		}

		/// <summary>
		/// Thread safe.
		/// </summary>
		public static Tuple<ImmutableList<string>, ImmutableList<IValue>> CompileParametersFailSafe(ImmutableList<Ast.OptionalParameter> parameters, MessagesCollection msgs) {

			int parametersCount = parameters.Count;
			bool wasOptional = false;
			var names = new string[parametersCount];
			IExpression[] optParamsExprs = null;

			for (int i = 0; i < parametersCount; i++) {
				var oParam = parameters[i];
				names[i] = oParam.NameId.Name;

				if (oParam.OptionalValue == null) {
					if (wasOptional) {
						msgs.AddError("Mandatory parameters have to be before all optional parameters, but mandatory parameter `{0}` is after optional.".Fmt(oParam.NameId.Name),
							oParam.Position);

						int optIndex = i - (parametersCount - optParamsExprs.Length);
						optParamsExprs[optIndex] = ExpressionCompiler.ErrorResult;
					}
				}
				else {
					if (!wasOptional) {
						optParamsExprs = new IExpression[parametersCount - i];
						wasOptional = true;
					}

					int optIndex = i - (parametersCount - optParamsExprs.Length);
					optParamsExprs[optIndex] = ExpressionCompiler.CompileFailSafe(oParam.OptionalValue, msgs);
				}
			}

			if (!wasOptional) {
				optParamsExprs = new IExpression[0];
			}

			// check wether parameters names are unique
			int nonUniq1, nonUniq2;
			if (optParamsExprs.TryGetEqualValuesIndices(out nonUniq1, out nonUniq2)) {
				msgs.AddError("{0}. and {1}. parameter have same name `{2}`.".Fmt(nonUniq1 + 1, nonUniq2 + 1, optParamsExprs[nonUniq1]),
					parameters[nonUniq2].Position);
			}

			// evaluating optional's paramters default values
			IValue[] optParamsEval = new IValue[optParamsExprs.Length];
			int index = 0;

			try {
				for (; index < optParamsExprs.Length; index++) {
					optParamsEval[index] = ExpressionEvaluator.Evaluate(optParamsExprs[index]);
				}
			}
			catch (EvalException ex) {
				int absIndex = index + (parametersCount - optParamsExprs.Length);
				msgs.AddError("Faled to evaluate default value of {0}. parameter. {1}".Fmt(absIndex + 1, ex.GetWholeMessage()),
					parameters[absIndex].Position);
				optParamsEval[index] = double.NaN.ToConst();
			}

			return new Tuple<ImmutableList<string>, ImmutableList<IValue>>(new ImmutableList<string>(names, true), new ImmutableList<IValue>(optParamsEval, true));
		}



	}
}

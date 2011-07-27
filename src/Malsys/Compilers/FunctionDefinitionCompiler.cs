using System;
using System.Diagnostics;
using Malsys.Expressions;

namespace Malsys.Compilers {
	public static class FunctionDefinitionCompiler {

		/// <summary>
		/// Thread safe.
		/// </summary>
		public static bool TryCompile(Ast.FunctionDefinition funDef, CompilerParameters prms, out FunctionDefinition result) {

			if (tryCompile(funDef, prms, out result)) {
				prms.Messages.AddMessage("Failed to compile function definition `{0}`.".Fmt(funDef.NameId.Name),
					CompilerMessageType.Error, funDef.Position);
				result = null;
				return false;
			}

			return true;
		}

		/// <summary>
		/// Thread safe.
		/// </summary>
		public static bool TryCompileParameters(ImmutableList<Ast.OptionalParameter> parameters, CompilerParameters prms,
				out ImmutableList<string> paramsNames, out ImmutableList<IValue> optParamsValues) {

			int parametersCount = parameters.Count;
			bool wasOptional = false;
			var names = new string[parametersCount];
			IExpression[] optParamsExprs = null;

			for (int i = 0; i < parametersCount; i++) {
				var oParam = parameters[i];
				names[i] = prms.CaseSensitiveVarsNames ? oParam.NameId.Name : oParam.NameId.Name.ToLowerInvariant();

				if (oParam.OptionalValue == null) {
					if (wasOptional) {
						prms.Messages.AddMessage("Mandatory parameters have to be before all optional parameters, but parameter `{0}` is after optional.".Fmt(oParam.NameId.Name),
							CompilerMessageType.Error, oParam.Position);
						paramsNames = null; optParamsValues = null;
						return false;
					}
				}
				else {
					if (!wasOptional) {
						optParamsExprs = new IExpression[parametersCount - i];
						wasOptional = true;
					}

					int optIndex = i - (parametersCount - optParamsExprs.Length);
					if (!ExpressionCompiler.TryCompile(oParam.OptionalValue, prms, out optParamsExprs[optIndex])) {
						prms.Messages.AddMessage("Failed to compile default value of {0}. parameter `{1}`".Fmt(i + 1, oParam.NameId.Name),
							CompilerMessageType.Error, oParam.Position);
						paramsNames = null; optParamsValues = null;
						return false;
					}
				}
			}

			if (!wasOptional) {
				optParamsExprs = new IExpression[0];
			}

			// check wether parameters names are unique
			int nonUniq1, nonUniq2;
			if (optParamsExprs.TryGetEqualValuesIndices(out nonUniq1, out nonUniq2)) {
				prms.Messages.AddMessage("{0}. and {1}. parameter have same name `{2}`.".Fmt(nonUniq1 + 1, nonUniq2 + 1, optParamsExprs[nonUniq1]),
					CompilerMessageType.Error, parameters[nonUniq2].Position);
				paramsNames = null; optParamsValues = null;
				return false;
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
				prms.Messages.AddMessage("Faled to evaluate default value of {0}. parameter. {1}".Fmt(absIndex + 1, ex.GetWholeMessage()),
					CompilerMessageType.Error, parameters[absIndex].Position);
				paramsNames = null; optParamsValues = null;
				return false;
			}
			catch (Exception ex) {
				int absIndex = index + (parametersCount - optParamsExprs.Length);

				Debug.Fail("{0} exception while evaluating default value of {1}. param.".Fmt(
					ex.GetType().ToString(), absIndex + 1));

				prms.Messages.AddMessage("Failed to evaluate default value of {0}. parameter.".Fmt(absIndex + 1),
					CompilerMessageType.Error, parameters[absIndex].Position);
				paramsNames = null; optParamsValues = null;
				return false;
			}

			paramsNames = new ImmutableList<string>(names, true);
			optParamsValues = new ImmutableList<IValue>(optParamsEval, true);
			return true;
		}


		private static bool tryCompile(Ast.FunctionDefinition funDef, CompilerParameters prms, out FunctionDefinition result) {

			ImmutableList<string> paramsNames;
			ImmutableList<IValue> optParamsValues;
			if (!TryCompileParameters(funDef.Parameters, prms, out paramsNames, out optParamsValues)) {
				result = null;
				return false;
			}

			RichExpression richExpr;
			if (!ExpressionCompiler.TryCompileRich(funDef, prms, out richExpr)) {
				result = null;
				return false;
			}

			string name = prms.CaseSensitiveFunsNames ? funDef.NameId.Name : funDef.NameId.Name.ToLowerInvariant();

			result = new FunctionDefinition(name, paramsNames, optParamsValues, richExpr.VariableDefinitions, richExpr.Expression);
			return true;
		}

	}
}

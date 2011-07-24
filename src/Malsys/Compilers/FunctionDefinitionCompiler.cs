using Malsys.Expressions;

namespace Malsys.Compilers {
	/// <summary>
	/// Thread safe.
	/// </summary>
	public static class FunctionDefinitionCompiler {

		public static bool TryCompile(Ast.FunctionDefinition funDef, CompilerParameters prms, out FunctionDefinition result) {

			bool wasOptional = false;
			var paramNames = new string[funDef.ParametersCount];
			IExpression[] optParamsValues = null;

			for (int i = 0; i < funDef.ParametersCount; i++) {
				var oParam = funDef.GetOptionalParameter(i);
				paramNames[i] = prms.CaseSensitiveVarsNames ? oParam.NameId.Name : oParam.NameId.Name.ToLowerInvariant();

				if (oParam.OptionalValue == null) {
					if (wasOptional) {
						prms.Messages.AddMessage("Mandatory parameters have to be before all optional parameters, but parameter `{0}` is after optional.",
							CompilerMessageType.Error, oParam.Position);
						result = null;
						return false;
					}
				}
				else {
					if (!wasOptional) {
						optParamsValues = new IExpression[funDef.ParametersCount - i];
						wasOptional = true;
					}

					if (!ExpressionCompiler.TryCompile(oParam.OptionalValue, prms, out optParamsValues[i - (funDef.ParametersCount - optParamsValues.Length)])) {
						prms.Messages.AddMessage("Failed to compile default value of {0}. parameter `{1}`".Fmt(i, oParam.NameId.Name),
							CompilerMessageType.Error, oParam.Position);
					}
				}
			}

			if (!wasOptional) {
				optParamsValues = new IExpression[0];
			}

			VariableDefinition[] varDefs;
			if (!VariableDefinitionCompiler.TryCompile(funDef, prms, out varDefs)) {
				result = null;
				return false;
			}

			IExpression expr;
			if (!ExpressionCompiler.TryCompile(funDef.Expression, prms, out expr)) {
				result = null;
				return false;
			}

			string name = prms.CaseSensitiveFunsNames ? funDef.NameId.Name : funDef.NameId.Name.ToLowerInvariant();
			result = new FunctionDefinition(name, paramNames, optParamsValues, varDefs, expr);
			return true;
		}
	}
}

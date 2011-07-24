using Malsys.Expressions;

namespace Malsys.Compilers {
	/// <summary>
	/// Thread safe.
	/// </summary>
	public static class VariableDefinitionCompiler {

		public static bool TryCompile(Ast.VariableDefinition varDef, CompilerParameters prms, out VariableDefinition result) {

			IExpression expr;
			if (ExpressionCompiler.TryCompile(varDef.Expression, prms, out expr)) {
				var name = prms.CaseSensitiveVarsNames ? varDef.NameId.Name : varDef.NameId.Name.ToLowerInvariant();
				result = new VariableDefinition(name, expr);
				return true;
			}
			else {
				result = null;
				return false;
			}
		}

		public static bool TryCompile(Ast.ImmutableList<Ast.VariableDefinition> varDefs, CompilerParameters prms, out VariableDefinition[] result) {
			result = new VariableDefinition[varDefs.Length];

			for (int i = 0; i < varDefs.Length; i++) {
				if (!TryCompile(varDefs[i], prms, out result[i])) {
					result = null;
					return false;
				}
			}

			return true;
		}
	}
}

using Malsys.Expressions;

namespace Malsys.Compilers {
	public static class VariableDefinitionCompiler {
		/// <summary>
		/// Thread safe.
		/// </summary>
		public static VariableDefinition CompileFailSafe(this Ast.VariableDefinition varDef, MessagesCollection msgs) {
			return new VariableDefinition(varDef.NameId.Name, varDef.Expression.CompileFailSafe(msgs));
		}

		/// <summary>
		/// Thread safe.
		/// </summary>
		public static ImmutableList<VariableDefinition> CompileFailSafe(this ImmutableList<Ast.VariableDefinition> varDefs, MessagesCollection msgs) {

			var rsltList = new VariableDefinition[varDefs.Length];

			for (int i = 0; i < varDefs.Length; i++) {
				rsltList[i] = varDefs[i].CompileFailSafe(msgs);
			}

			return new ImmutableList<VariableDefinition>(rsltList, true);
		}
	}
}

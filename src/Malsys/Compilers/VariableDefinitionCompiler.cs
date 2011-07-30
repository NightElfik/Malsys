using Malsys.Expressions;

namespace Malsys.Compilers {
	public static class VariableDefinitionCompiler {
		/// <summary>
		/// Thread safe.
		/// </summary>
		public static VariableDefinition CompileFailSafe(Ast.VariableDefinition varDef, MessagesCollection msgs) {

			var expr = ExpressionCompiler.CompileFailSafe(varDef.Expression, msgs);
			return new VariableDefinition(varDef.NameId.Name, expr);
		}

		/// <summary>
		/// Thread safe.
		/// </summary>
		public static ImmutableList<VariableDefinition> CompileFailSafe(ImmutableList<Ast.VariableDefinition> varDefs, MessagesCollection msgs) {

			var rsltList = new VariableDefinition[varDefs.Length];

			for (int i = 0; i < varDefs.Length; i++) {
				rsltList[i] = CompileFailSafe(varDefs[i], msgs);
			}

			return new ImmutableList<VariableDefinition>(rsltList, true);
		}
	}
}

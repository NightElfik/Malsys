using Malsys.Expressions;

namespace Malsys.Compilers {
	public static class VariableDefinitionCompiler {
		/// <summary>
		/// Thread safe.
		/// </summary>
		public static VariableDefinition<IExpression> CompileFailSafe(this Ast.VariableDefinition varDef, MessagesCollection msgs) {
			return new VariableDefinition<IExpression>(varDef.NameId.Name, varDef.Expression.CompileFailSafe(msgs));
		}

		public static VariableDefinition<SymbolsList<IExpression>> CompileFailSafe(this Ast.SymbolsDefinition symDef, MessagesCollection msgs) {
			return new VariableDefinition<SymbolsList<IExpression>>(symDef.NameId.Name, symDef.Symbols.CompileListFailSafe(msgs));
		}

		/// <summary>
		/// Thread safe.
		/// </summary>
		public static ImmutableList<VariableDefinition<IExpression>> CompileFailSafe(this ImmutableList<Ast.VariableDefinition> varDefs, MessagesCollection msgs) {

			var rsltList = new VariableDefinition<IExpression>[varDefs.Length];

			for (int i = 0; i < varDefs.Length; i++) {
				rsltList[i] = varDefs[i].CompileFailSafe(msgs);
			}

			return new ImmutableList<VariableDefinition<IExpression>>(rsltList, true);
		}
	}
}


namespace Malsys.SemanticModel.Compiled {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class SymbolsConstDefinition : ILsystemStatement {

		public readonly string Name;
		public readonly ImmutableList<Symbol<IExpression>> Symbols;

		public readonly Ast.SymbolsConstDefinition AstNode;


		public SymbolsConstDefinition(string name, ImmutableList<Symbol<IExpression>> symbols, Ast.SymbolsConstDefinition astNode = null) {

			Name = name;
			Symbols = symbols;

			AstNode = astNode;

		}


		public LsystemStatementType StatementType {
			get { return LsystemStatementType.SymbolsConstant; }
		}

	}
}

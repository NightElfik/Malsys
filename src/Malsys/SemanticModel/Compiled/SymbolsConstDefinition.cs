
namespace Malsys.SemanticModel.Compiled {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class SymbolsConstDefinition : ILsystemStatement {

		public readonly string Name;
		public readonly ImmutableList<Symbol<IExpression>> Symbols;


		public SymbolsConstDefinition(string name, ImmutableList<Symbol<IExpression>> symbols) {

			Name = name;
			Symbols = symbols;
		}


		public LsystemStatementType StatementType {
			get { return LsystemStatementType.SymbolsConstant; }
		}

	}
}

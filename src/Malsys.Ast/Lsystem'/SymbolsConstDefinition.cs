
namespace Malsys.Ast {
	public class SymbolsConstDefinition : ILsystemStatement {

		public Identifier NameId;
		public ListPos<LsystemSymbol> SymbolsList;

		public PositionRange Position { get; private set; }


		public SymbolsConstDefinition(Identifier name, ListPos<LsystemSymbol> symbols, PositionRange pos) {
			NameId = name;
			SymbolsList = symbols;
			Position = pos;
		}


		public LsystemStatementType StatementType {
			get { return LsystemStatementType.SymbolsConstDefinition; }
		}

	}
}

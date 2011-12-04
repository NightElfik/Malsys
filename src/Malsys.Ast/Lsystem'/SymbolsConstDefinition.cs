
namespace Malsys.Ast {
	public class SymbolsConstDefinition : ILsystemStatement {

		public readonly Identificator NameId;
		public readonly ImmutableListPos<LsystemSymbol> SymbolsList;


		public SymbolsConstDefinition(Identificator name, ImmutableListPos<LsystemSymbol> symbols, Position pos) {
			NameId = name;
			SymbolsList = symbols;
			Position = pos;
		}


		public Position Position { get; private set; }


		public void Accept(ILsystemVisitor visitor) {
			visitor.Visit(this);
		}

	}
}

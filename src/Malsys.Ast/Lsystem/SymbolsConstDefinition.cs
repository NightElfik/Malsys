
namespace Malsys.Ast {
	public class SymbolsConstDefinition : ILsystemStatement {

		public readonly Identificator NameId;
		public readonly ImmutableListPos<LsystemSymbol> SymbolsList;


		public SymbolsConstDefinition(Identificator name, ImmutableListPos<LsystemSymbol> symbols, Position pos) {
			NameId = name;
			SymbolsList = symbols;
			Position = pos;
		}


		#region IToken Members

		public Position Position { get; private set; }

		#endregion

		#region ILsystemVisitable Members

		public void Accept(ILsystemVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}

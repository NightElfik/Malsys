
namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class SymbolsDefinition : IToken, ILsystemStatement {

		public readonly Keyword Keyword;
		public readonly Identificator NameId;
		public readonly SymbolsListPos<Expression> Symbols;


		public SymbolsDefinition(Keyword keyword, Identificator name, SymbolsListPos<Expression> symbols, Position pos) {
			Keyword = keyword;
			NameId = name;
			Symbols = symbols;
			Position = pos;
		}


		#region IToken Members

		public Position Position { get; private set; }

		#endregion

		#region IAstVisitable Members

		public void Accept(IAstVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}

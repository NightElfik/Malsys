
namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class SymbolsDefinition : IToken, ILsystemStatement {

		public readonly Keyword Keyword;
		public readonly Identificator NameId;
		public readonly ImmutableList<SymbolExprArgs> Symbols;


		public SymbolsDefinition(Keyword keyword, Identificator name, ImmutableList<SymbolExprArgs> symbols, Position pos) {
			Keyword = keyword;
			NameId = name;
			Symbols = symbols;
			Position = pos;
		}


		#region IToken Members

		public Position Position { get; private set; }

		#endregion
	}
}

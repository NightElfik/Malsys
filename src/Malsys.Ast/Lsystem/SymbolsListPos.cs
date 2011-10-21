using System.Collections.Generic;

namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class SymbolsListPos<T> : ImmutableListPos<Symbol<T>> where T : IToken {

		public SymbolsListPos(Position pos)
			: base(pos) { }

		public SymbolsListPos(ImmutableListPos<Symbol<T>> symbols, Position pos)
			: base(symbols, pos) { }

		public SymbolsListPos(IEnumerable<Symbol<T>> symbols, Position pos)
			: base(symbols, pos) { }

		public SymbolsListPos(Position beginSep, Position endSep, Position pos)
			: base(beginSep, endSep, pos) { }

		public SymbolsListPos(IEnumerable<Symbol<T>> vals, Position beginSep, Position endSep, Position pos)
			: base(vals, beginSep, endSep, pos) { }

	}
}

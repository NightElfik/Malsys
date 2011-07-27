using System.Collections.Generic;

namespace Malsys {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class SymbolsList<T> : ImmutableList<Symbol<T>> {

		public SymbolsList()
			: base(ImmutableList<Symbol<T>>.Empty) { }

		public SymbolsList(IEnumerable<Symbol<T>> symbols)
			: base(symbols) { }

		public SymbolsList(ImmutableList<Symbol<T>> symbols)
			: base(symbols) { }

	}
}

using System.Collections.Generic;

namespace Malsys {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class SymbolsList<T> : ImmutableList<Symbol<T>> {

		public static readonly SymbolsList<T> Empty = new SymbolsList<T>(ImmutableList<Symbol<T>>.Empty);


		public SymbolsList(IEnumerable<Symbol<T>> symbols)
			: base(symbols) { }

		public SymbolsList(ImmutableList<Symbol<T>> symbols)
			: base(symbols) { }

	}
}

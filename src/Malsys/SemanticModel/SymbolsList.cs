/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System.Collections.Generic;

namespace Malsys.SemanticModel {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class SymbolsList<T> : ImmutableList<Symbol<T>> {

		new public static readonly SymbolsList<T> Empty = new SymbolsList<T>(ImmutableList<Symbol<T>>.Empty);


		public SymbolsList(IEnumerable<Symbol<T>> symbols)
			: base(symbols) { }

		public SymbolsList(ImmutableList<Symbol<T>> symbols)
			: base(symbols) { }

	}
}

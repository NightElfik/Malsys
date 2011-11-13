using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Malsys.SemanticModel.Evaluated {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class SymbolsListVal : ImmutableList<Symbol<IValue>> {

		new public static readonly SymbolsListVal Empty = new SymbolsListVal(ImmutableList<Symbol<IValue>>.Empty);


		public SymbolsListVal(IEnumerable<Symbol<IValue>> symbols)
			: base(symbols) { }

		public SymbolsListVal(ImmutableList<Symbol<IValue>> symbols)
			: base(symbols) { }

	}
}

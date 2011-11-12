using System.Collections.Generic;

namespace Malsys.Ast {
	public class LsystemSymbolList : ImmutableListPos<LsystemSymbol>, IBindable {


		public LsystemSymbolList(ImmutableListPos<LsystemSymbol> symbols)
			: base(symbols) {
		}

	}
}

using System.Collections.Generic;
using Symbol = Malsys.SemanticModel.Symbol<Malsys.SemanticModel.Evaluated.IValue>;

namespace Malsys.Processing.Components.Interpreters {
	public class SymbolsBuffer : ISymbolProcessor {

		List<Symbol> buffer;


		#region ISymbolProcessor Members

		public void BeginProcessing() {
			buffer = new List<Symbol>();
		}

		public void ProcessSymbol(Symbol symbol) {
			buffer.Add(symbol);
		}

		public void EndProcessing() {
		}

		#endregion


		public List<Symbol> GetAndClear() {
			var buff = buffer ?? new List<Symbol>();
			buffer = null;
			return buff;
		}

	}
}

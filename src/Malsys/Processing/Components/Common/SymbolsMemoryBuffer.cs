using System.Collections.Generic;
using Symbol = Malsys.SemanticModel.Symbol<Malsys.SemanticModel.Evaluated.IValue>;
using Malsys.IO;

namespace Malsys.Processing.Components.Common {
	public class SymbolsMemoryBuffer : ISymbolProcessor {

		List<Symbol> buffer;



		#region ISymbolProcessor Members

		public void ProcessSymbol(Symbol symbol) {
			if (buffer != null) {
				buffer.Add(symbol);
			}
		}

		#endregion

		#region IComponent Members

		public ProcessContext Context { get; set; }

		public void BeginProcessing(bool measuring) {
			buffer = measuring ? null : new List<Symbol>();
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

using System.Collections.Generic;
using Symbol = Malsys.SemanticModel.Symbol<Malsys.SemanticModel.Evaluated.IValue>;

namespace Malsys.Processing.Components.Common {
	public class SymbolsMemoryBuffer : ISymbolProcessor {

		private List<Symbol> buffer;


		#region ISymbolProcessor Members

		public bool RequiresMeasure { get { return false; } }

		public void Initialize(ProcessContext ctxt) { }

		public void Cleanup() { }

		public void BeginProcessing(bool measuring) {
			buffer = measuring ? null : new List<Symbol>();
		}

		public void EndProcessing() { }


		public void ProcessSymbol(Symbol symbol) {
			if (buffer != null) {
				buffer.Add(symbol);
			}
		}

		#endregion


		public List<Symbol> GetAndClear() {
			var buff = buffer ?? new List<Symbol>();
			buffer = null;
			return buff;
		}
	}
}

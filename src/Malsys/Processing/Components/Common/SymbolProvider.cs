using System.Collections.Generic;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Processing.Components.Common {
	public class SymbolProvider : ISymbolProvider {

		public readonly IEnumerable<Symbol<IValue>> Symbols;


		public SymbolProvider(IEnumerable<Symbol<IValue>> symbols) {
			Symbols = symbols;
		}


		public IEnumerator<Symbol<IValue>> GetEnumerator() {
			return Symbols.GetEnumerator();
		}


		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return Symbols.GetEnumerator();
		}


		public bool RequiresMeasure {
			get { return false; }
		}

		public void Initialize(ProcessContext ctxt) { }

		public void Cleanup() { }

		public void BeginProcessing(bool measuring) { }

		public void EndProcessing() { }

	}
}

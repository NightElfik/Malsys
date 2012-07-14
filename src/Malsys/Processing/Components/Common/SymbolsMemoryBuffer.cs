/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System.Collections.Generic;
using Symbol = Malsys.SemanticModel.Symbol<Malsys.SemanticModel.Evaluated.IValue>;

namespace Malsys.Processing.Components.Common {
	/// <summary>
	/// Saves all processed symbols to the memory buffer.
	/// This component is used especially in the unit tests where printing of processed symbols is not desired.
	/// If the user do not have access to the instance of this component there is no way how to get saved symbols.
	/// </summary>
	/// <name>Symbols memory buffer</name>
	/// <group>Special</group>
	public class SymbolsMemoryBuffer : ISymbolProcessor {

		private List<Symbol> buffer;


		public IMessageLogger Logger { get; set; }


		public void Reset() { }

		public void Initialize(ProcessContext ctxt) { }

		public void Cleanup() { }

		public void Dispose() { }



		public bool RequiresMeasure { get { return false; } }


		public void BeginProcessing(bool measuring) {
			buffer = measuring ? null : new List<Symbol>();
		}

		public void EndProcessing() { }


		public void ProcessSymbol(Symbol symbol) {
			if (buffer != null) {
				buffer.Add(symbol);
			}
		}



		public List<Symbol> GetAndClear() {
			var buff = buffer ?? new List<Symbol>();
			buffer = null;
			return buff;
		}
	}
}

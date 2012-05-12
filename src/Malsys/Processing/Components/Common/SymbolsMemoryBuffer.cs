﻿/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System.Collections.Generic;
using Symbol = Malsys.SemanticModel.Symbol<Malsys.SemanticModel.Evaluated.IValue>;

namespace Malsys.Processing.Components.Common {
	/// <summary>
	/// Saves processed symbols to memory buffer.
	/// This component is used especially in unit tests where printing
	/// of symbols is not desired.
	/// </summary>
	/// <name>Symbols memory buffer</name>
	/// <group>Special</group>
	public class SymbolsMemoryBuffer : ISymbolProcessor {

		private List<Symbol> buffer;


		public IMessageLogger Logger { get; set; }


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



		public List<Symbol> GetAndClear() {
			var buff = buffer ?? new List<Symbol>();
			buffer = null;
			return buff;
		}
	}
}

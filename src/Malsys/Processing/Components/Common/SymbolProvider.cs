﻿using System.Collections.Generic;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Processing.Components.Common {
	/// <summary>
	///	Standard implementation of ISymbolProvider interface.
	/// </summary>
	/// <name>Symbol provider component</name>
	/// <group>Common</group>
	public class SymbolProvider : ISymbolProvider {

		/// <summary>
		/// Symbol string which is provided.
		/// </summary>
		[UserSettableSybols]
		public ImmutableList<Symbol<IValue>> Symbols { get; set; }


		public SymbolProvider() {
			Symbols = ImmutableList<Symbol<IValue>>.Empty;
		}

		public SymbolProvider(IEnumerable<Symbol<IValue>> symbols) {
			Symbols = symbols.ToImmutableList();
		}

		public SymbolProvider(ImmutableList<Symbol<IValue>> symbols) {
			Symbols = symbols;
		}


		public IEnumerator<Symbol<IValue>> GetEnumerator() {
			return Symbols.GetEnumerator();
		}


		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return Symbols.GetEnumerator();
		}


		public void Initialize(ProcessContext ctxt) { }

		public void Cleanup() { }


		#region IProcessComponent Members

		public bool RequiresMeasure {
			get { return false; }
		}

		public void BeginProcessing(bool measuring) { }

		public void EndProcessing() { }

		#endregion
	}
}

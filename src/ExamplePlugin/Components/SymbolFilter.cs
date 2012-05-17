/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System;
using System.Collections.Generic;
using Malsys;
using Malsys.Processing;
using Malsys.Processing.Components;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;

namespace ExamplePlugin.Components {
	/// <summary>
	/// Filters symbol stream.
	/// </summary>
	/// <name>Symbol fileter</name>
	/// <group>Plugin</group>
	public class SymbolFilter : ISymbolProcessor {

		private HashSet<String> ignoredSymbols = new HashSet<string>();


		/// <summary>
		/// List of ignored symbols
		/// </summary>
		[AccessName("ignore")]
		[UserSettableSybols]
		public ImmutableList<Symbol<IValue>> Ignore {
			set {
				ignoredSymbols.Clear();
				foreach (var sym in value) {
					ignoredSymbols.Add(sym.Name);
				}
			}
		}


		/// <summary>
		/// Components to which filtered symbols are sent.
		/// </summary>
		[UserConnectable]
		public ISymbolProcessor Output { get; set; }



		public IMessageLogger Logger { get; set; }
		public bool RequiresMeasure { get { return false; } }

		public void Initialize(ProcessContext context) { }

		public void Cleanup() {
			ignoredSymbols.Clear();
		}


		public void ProcessSymbol(Symbol<IValue> symbol) {
			if (!ignoredSymbols.Contains(symbol.Name)) {
				Output.ProcessSymbol(symbol);
			}
		}

		public void BeginProcessing(bool measuring) {
			Output.BeginProcessing(measuring);
		}

		public void EndProcessing() {
			Output.EndProcessing();
		}

	}
}

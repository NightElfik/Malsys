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
	/// <remarks>
	/// Design and step-by-step implementation of this component is described in the bachelor thesis.
	/// </remarks>
	/// <name>Symbol fileter</name>
	/// <group>Plugin</group>
	public class SymbolFilter : ISymbolProcessor {

		private HashSet<string> ignoredSymbols = new HashSet<string>();


		/// <summary>
		/// List of ignored symbols.
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
			// the first part of tutorial how to implement component
			//if (char.IsLower(symbol.Name[0])) {
			//    Output.ProcessSymbol(symbol);
			//}
			if (ignoredSymbols.Contains(symbol.Name)) {
				return;
			}

			foreach (var arg in symbol.Arguments) {
				if (!arg.IsConstant) {
					continue; //  ignore non-constant arguments
				}
				var c = (Constant)arg;
				if (c.IsNaN) {
					Logger.LogMessage("InvalidSymbolParameter", MessageType.Error,
						symbol.AstNode.TryGetPosition(),
						string.Format("Symbol `{0}` have invalid parameter value `{1}`.", symbol.Name, c));
				}
				else if (c.IsInfinity) {
					Logger.LogMessage("StrangeSymbolParameter", MessageType.Warning,
						symbol.AstNode.TryGetPosition(),
						string.Format("Symbol `{0}` have strange parameter value `{1}`.", symbol.Name, c));
				}
			}

			Output.ProcessSymbol(symbol);

		}

		public void BeginProcessing(bool measuring) {
			Output.BeginProcessing(measuring);
		}

		public void EndProcessing() {
			Output.EndProcessing();
		}

	}
}

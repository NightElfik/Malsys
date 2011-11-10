using System;
using Malsys.Expressions;

namespace Malsys.Processing.Components.Rewriters {
	class EmptySymbolProcessor : ISymbolProcessor {

		public static readonly EmptySymbolProcessor Instance = new EmptySymbolProcessor();


		#region ISymbolProcessor Members

		public void BeginProcessing() {
			throw new InvalidOperationException("Empty symbol processor.");
		}

		public void ProcessSymbol(Symbol<IValue> symbol) {
			throw new InvalidOperationException("Empty symbol processor.");
		}

		public void EndProcessing() {
			throw new InvalidOperationException("Empty symbol processor.");
		}

		#endregion

	}
}

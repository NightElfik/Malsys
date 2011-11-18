using System;
using Malsys.Expressions;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Processing.Components.Common {
	class EmptySymbolProcessor : ISymbolProcessor {

		public static readonly EmptySymbolProcessor Instance = new EmptySymbolProcessor();



		#region ISymbolProcessor Members

		public void ProcessSymbol(Symbol<IValue> symbol) {
			throw new InvalidOperationException("Empty symbol processor.");
		}

		#endregion

		#region IComponent Members

		public ProcessContext Context {
			set { throw new InvalidOperationException("Empty symbol processor."); }
		}

		public void BeginProcessing(bool measuring) {
			throw new InvalidOperationException("Empty symbol processor.");
		}

		public void EndProcessing() {
			throw new InvalidOperationException("Empty symbol processor.");
		}

		#endregion
	}
}

using System;
using Malsys.Expressions;
using Malsys.SemanticModel.Evaluated;
using Malsys.SemanticModel;

namespace Malsys.Processing.Components.Rewriters {
	class EmptyRewriter : IRewriter {

		public static readonly EmptyRewriter Instance = new EmptyRewriter();


		#region IRewriter Members

		public ISymbolProcessor OutputProcessor {
			set { throw new InvalidOperationException("Empty rewriter."); }
		}

		public ProcessContext Context {
			set { throw new InvalidOperationException("Empty rewriter."); }
		}

		public IValue RandomSeed {
			set { throw new InvalidOperationException("Empty rewriter."); }
		}

		#endregion

		#region ISymbolProcessor Members

		public void BeginProcessing() {
			throw new InvalidOperationException("Empty rewriter.");
		}

		public void ProcessSymbol(Symbol<IValue> symbol) {
			throw new InvalidOperationException("Empty rewriter.");
		}

		public void EndProcessing() {
			throw new InvalidOperationException("Empty rewriter.");
		}

		#endregion

	}
}

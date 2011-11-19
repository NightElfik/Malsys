using System;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Processing.Components.Rewriters {
	class EmptyRewriter : IRewriter {

		public static readonly EmptyRewriter Instance = new EmptyRewriter();




		#region IRewriter Members

		public ISymbolProcessor OutputProcessor {
			set { throw new InvalidOperationException("Empty rewriter."); }
		}

		[UserSettable]
		public IValue RandomSeed {
			set { throw new InvalidOperationException("Empty rewriter."); }
		}

		#endregion

		#region ISymbolProcessor Members

		public void ProcessSymbol(Symbol<IValue> symbol) {
			throw new InvalidOperationException("Empty rewriter.");
		}

		#endregion

		#region IComponent Members

		public ProcessContext Context {
			set { throw new InvalidOperationException("Empty rewriter."); }
		}

		public void BeginProcessing(bool measuring) {
			throw new InvalidOperationException("Empty rewriter.");
		}

		public void EndProcessing() {
			throw new InvalidOperationException("Empty rewriter.");
		}

		#endregion
	}
}

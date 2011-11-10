using System;
using System.Collections.Generic;
using Malsys.Expressions;
using Microsoft.FSharp.Collections;

namespace Malsys.Processing.Components.Rewriters {
	class EmptyRewriter : IRewriter {

		public static readonly EmptyRewriter Instance = new EmptyRewriter();


		#region IRewriter Members

		public ISymbolProcessor OutputProcessor {
			set { throw new InvalidOperationException("Empty rewriter."); }
		}

		public IEnumerable<RewriteRule> RewriteRules {
			set { throw new InvalidOperationException("Empty rewriter."); }
		}

		public FSharpMap<string, IValue> Variables {
			set { throw new InvalidOperationException("Empty rewriter."); }
		}

		public FSharpMap<string, FunctionDefinition> Functions {
			set { throw new InvalidOperationException("Empty rewriter."); }
		}

		public Expressions.IValue RandomSeed {
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

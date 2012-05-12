/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System;
using System.Diagnostics;
using Malsys.SemanticModel;

namespace Malsys.Processing.Context {
	public class ContextListBuilder<T> {

		private ContextListNode<T> rootNode;
		private ContextListNode<T> contextTailNode;

		private Func<Symbol<T>, bool> isStartBranchSymbol;
		private Func<Symbol<T>, bool> isEndBranchSymbol;


		public ContextListBuilder(Func<Symbol<T>, bool> isStartBranchSymbol, Func<Symbol<T>, bool> isEndBranchSymbol) {

			this.isStartBranchSymbol = isStartBranchSymbol;
			this.isEndBranchSymbol = isEndBranchSymbol;

			Reset();

		}

		public void Reset() {
			rootNode = new ContextListNode<T>();
			contextTailNode = rootNode;
		}


		public ContextListNode<T> RootNode { get { return rootNode; } }


		public void AddSymbolToContext(Symbol<T> symbol) {

			ContextListNode<T> newNode;

			if (isStartBranchSymbol(symbol)) {
				newNode = new ContextListNode<T>();
				Debug.Assert(!contextTailNode.InnerList.IsCompleted);
				contextTailNode.InnerList.Append(newNode);
				contextTailNode = newNode;
			}
			else if (isEndBranchSymbol(symbol)) {
				if (contextTailNode.ParentList == null) {
					Debug.Assert(contextTailNode == rootNode);
					throw new ProcessException("End branch symbol `{0}` occurred when no branch was open.".Fmt(symbol.Name));
				}
				contextTailNode.InnerList.Complete();
				contextTailNode = contextTailNode.ParentList.ParentNode;
				return;
			}
			else {
				newNode = new ContextListNode<T>(symbol);
				Debug.Assert(contextTailNode == rootNode || !contextTailNode.InnerList.IsCompleted);
				contextTailNode.InnerList.Append(newNode);
			}

		}

	}
}

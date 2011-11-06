using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Malsys.Expressions;

namespace Malsys.Rewriters.Iterators {
	public class SingleRewriterIterator : IRewriterIterator, ISymbolProcessor {

		private int iterations;
		private IRewriter rewriter;
		private ISymbolProcessor outputProcessor;

		private List<Symbol<IValue>> inBuffer;
		private List<Symbol<IValue>> outBuffer;



		public SingleRewriterIterator() {

		}

		#region IRewriterIterator Members

		public void Initialize(int iters, IRewriter rwter, ISymbolProcessor outProcessor, IEnumerable<Symbol<IValue>> symbols) {

			iterations = iters;

			rewriter = rwter;
			rewriter.OutputProcessor = this;

			outputProcessor = outProcessor;

			inBuffer = symbols.ToList();
			outBuffer = new List<Symbol<IValue>>(inBuffer.Count);
		}

		public void Start() {
			for (int i = 0; i < iterations; i++) {

				foreach (var s in inBuffer) {
					rewriter.ProcessSymbol(s);
				}
				rewriter.EndProcessing();

				inBuffer.Clear();
				Swap.Them(ref inBuffer, ref outBuffer);
			}

			foreach (var s in inBuffer) {
				outputProcessor.ProcessSymbol(s);
				outputProcessor.EndProcessing();
			}
		}

		#endregion

		#region ISymbolProcessor Members

		public void ProcessSymbol(Symbol<IValue> symbol) {
			outBuffer.Add(symbol);
		}

		public void EndProcessing() {

		}

		#endregion
	}
}

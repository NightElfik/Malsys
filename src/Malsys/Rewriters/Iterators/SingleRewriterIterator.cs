using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Malsys.Expressions;
using System.Diagnostics.Contracts;

namespace Malsys.Rewriters.Iterators {
	public class SingleRewriterIterator : IRewriterIterator {

		private IRewriter rewriter;

		private ISymbolProcessor outProcessor;

		private List<Symbol<IValue>> inBuffer;
		private List<Symbol<IValue>> outBuffer;

		private int iterations;



		public SingleRewriterIterator() {

			rewriter = EmptyRewriter.Instance;
			outProcessor = EmptySymbolProcessor.Instance;

			inBuffer = new List<Symbol<IValue>>();
			outBuffer = new List<Symbol<IValue>>();
		}

		[ContractInvariantMethod]
		private void objectInvariant() {

			Contract.Invariant(rewriter != null);
			Contract.Invariant(outProcessor != null);

			Contract.Invariant(inBuffer != null);
			Contract.Invariant(outBuffer != null);
		}


		#region IRewriterIterator Members

		public IRewriter Rewriter {
			set { rewriter = value; }
		}

		public ISymbolProcessor OutputProcessor {
			set { outProcessor = value; }
		}

		public IEnumerable<Symbol<IValue>> Axiom {
			set {
				inBuffer.Clear();
				inBuffer.AddRange(value);
			}
		}

		public IValue Iterations {
			set {
				if (value.IsConstant && !((Constant)value).IsNaN && ((Constant)value).Value >= 0) {
					iterations = ((Constant)value).GetIntValueRounded();
				}
				else {
					throw new ArgumentException("Iterations value is invalid.");
				}
			}
		}

		public void Start() {

			for (int i = 0; i < iterations; i++) {

				rewriter.BeginProcessing();
				foreach (var s in inBuffer) {
					rewriter.ProcessSymbol(s);
				}
				rewriter.EndProcessing();

				inBuffer.Clear();
				Swap.Them(ref inBuffer, ref outBuffer);
			}

			outProcessor.BeginProcessing();
			foreach (var s in inBuffer) {
				outProcessor.ProcessSymbol(s);
			}
			outProcessor.EndProcessing();
		}

		#endregion

		#region ISymbolProcessor Members

		public void BeginProcessing() {
		}

		public void ProcessSymbol(Symbol<IValue> symbol) {
			outBuffer.Add(symbol);
		}

		public void EndProcessing() {
		}

		#endregion
	}
}

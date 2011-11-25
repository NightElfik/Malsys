using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Malsys.Processing.Components.Common;
using Malsys.Processing.Components.Rewriters;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Processing.Components.RewriterIterators {
	public class SingleRewriterIterator : IRewriterIterator {

		private IRewriter rewriter;

		private ProcessContext context;
		private ISymbolProcessor outProcessor;

		private List<Symbol<IValue>> inBuffer;
		private List<Symbol<IValue>> outBuffer;

		private int iterations;

		private bool aborting = false;
		private bool aborted = false;



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

		[UserSettable]
		public ImmutableList<Symbol<IValue>> Axiom {
			set {
				inBuffer.Clear();
				inBuffer.AddRange(value);
			}
		}

		[UserSettable]
		public IValue Iterations {
			set {
				if (value.IsConstant && !((Constant)value).IsNaN && ((Constant)value).Value >= 0) {
					iterations = ((Constant)value).RoundedIntValue;
				}
				else {
					throw new ArgumentException("Iterations value is invalid.");
				}
			}
		}

		#endregion

		#region ISymbolProcessor Members

		public void ProcessSymbol(Symbol<IValue> symbol) {
			outBuffer.Add(symbol);
		}

		#endregion

		#region IComponent Members

		public bool RequiresMeasure { get { return false; } }

		public void Initialize(ProcessContext context) {
			this.context = context;
		}

		public void Cleanup() { }

		public void BeginProcessing(bool measuring) {
		}

		public void EndProcessing() {
		}

		#endregion

		#region IProcessStarter Members

		public void Start(bool measure) {

			rewrite();

			if (aborted) {
				return;
			}

			if (measure) {
				interpret(true);
			}

			if (aborted) {
				return;
			}

			interpret(false);

		}

		public void Abort() {
			aborting = true;
		}

		#endregion

		private void rewrite() {

			for (int i = 0; i < iterations; i++) {

				rewriter.BeginProcessing(true);
				foreach (var s in inBuffer) {

					if (aborting) {
						rewriter.EndProcessing();
						context.Messages.LogMessage<SingleRewriterIterator>("Aborted", MessageType.Info, Position.Unknown,
							"Iterator aborted while rewriting iteration {0} of {1}.".Fmt(i, iterations));
						aborted = true;
						return;
					}

					rewriter.ProcessSymbol(s);
				}
				rewriter.EndProcessing();

				inBuffer.Clear();
				Swap.Them(ref inBuffer, ref outBuffer);
			}
		}

		private void interpret(bool measuring) {

			outProcessor.BeginProcessing(measuring);
			foreach (var s in inBuffer) {

				if (aborting) {
					rewriter.EndProcessing();
					context.Messages.LogMessage<SingleRewriterIterator>("Aborted", MessageType.Info, Position.Unknown,
						"Iterator aborted while {0}.".Fmt(measuring ? "measuring" : "interpreting"));
					aborted = true;
					return;
				}

				outProcessor.ProcessSymbol(s);
			}
			outProcessor.EndProcessing();
		}
	}
}

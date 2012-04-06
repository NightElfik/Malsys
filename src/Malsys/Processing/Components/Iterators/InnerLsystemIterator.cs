using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Processing.Components.RewriterIterators {
	[Component("Memory buffered iterator", ComponentGroupNames.Iterators)]
	public class InnerLsystemIterator : ISymbolProvider, IProcessStarter {

		private IMessageLogger logger;

		private ISymbolProvider symbolProvider;

		private ISymbolProcessor outProcessor;

		private List<Symbol<IValue>> inBuffer = new List<Symbol<IValue>>();
		private List<Symbol<IValue>> outBuffer = new List<Symbol<IValue>>();

		private int iterations;
		private int currIteration;

		private Stopwatch swDuration = new Stopwatch();
		private TimeSpan timeout;
		private bool aborting = false;
		private bool aborted = false;


		[AccessName("axiom")]
		[UserSettableSybols(IsMandatory = true)]
		public ImmutableList<Symbol<IValue>> Axiom { private get; set; }

		/// <summary>
		/// Number of current iteration. Zero-based, first iteration is 0, last is Iterations - 1.
		/// </summary>
		[AccessName("currentIteration")]
		[UserGettable]
		public Constant CurrentIteration {
			get { return currIteration.ToConst(); }
		}

		[AccessName("iterations", "i")]
		[UserSettable]
		public Constant Iterations {
			set {
				if (!value.IsNaN && value.Value >= 0) {
					iterations = value.RoundedIntValue;
				}
				else {
					throw new InvalidUserValueException("Iterations value is invalid.");
				}
			}
		}


		#region IIterator Members

		[UserConnectable]
		public ISymbolProvider SymbolProvider {
			set { symbolProvider = value; }
		}

		[UserConnectable]
		public ISymbolProcessor OutputProcessor {
			set { outProcessor = value; }
		}

		public IEnumerator<Symbol<IValue>> GetEnumerator() {
			return inBuffer.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return inBuffer.GetEnumerator();
		}


		public bool RequiresMeasure {
			get { return false; }
		}

		public void Initialize(ProcessContext ctxt) {
			logger = ctxt.Logger;
			timeout = ctxt.ProcessingTimeLimit;
		}

		public void Cleanup() {

		}

		public void BeginProcessing(bool measuring) {

		}

		public void EndProcessing() {

		}

		public void Start(bool doMeasure) {

			swDuration.Restart();

			symbolProvider.BeginProcessing(true);

			start();  // never do measure

			symbolProvider.EndProcessing();

			swDuration.Stop();
		}

		public void Abort() {
			aborting = true;
		}

		#endregion


		private void start() {

			inBuffer.Clear();
			inBuffer.AddRange(Axiom);

			for (currIteration = 0; currIteration < iterations; currIteration++) {

				rewriteIteration();
				if (aborted) { return; }

			}

			interpret(false);

		}

		private void rewriteIteration() {

			foreach (var symbol in symbolProvider) {

				if (swDuration.Elapsed > timeout || aborting) {
					logger.LogMessage(aborting ? Message.Abort : Message.Timeout, "rewriting iteration {0} of {1}".Fmt(currIteration, iterations));
					aborted = true;
					return;
				}

				outBuffer.Add(symbol);

			}

			inBuffer.Clear();
			Swap.Them(ref inBuffer, ref outBuffer);

		}

		private void interpret(bool measuring) {

			// do not begin processing of output processor -- we are inner L-system

			foreach (var s in inBuffer) {

				if (swDuration.Elapsed > timeout || aborting) {
					logger.LogMessage(aborting ? Message.Abort : Message.Timeout, measuring ? "measuring" : "interpreting");
					aborted = true;
					return;
				}

				outProcessor.ProcessSymbol(s);
			}

		}


		public enum Message {

			[Message(MessageType.Error, "Time ran out while iterator was {0}.")]
			Timeout,
			[Message(MessageType.Error, "Iterator aborted while {0}.")]
			Abort,

		}
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;
using Malsys.Processing.Components.Common;

namespace Malsys.Processing.Components.RewriterIterators {
	[Component("Memory buffered iterator", ComponentGroupNames.Iterators)]
	public class MemoryBufferedIterator : IIterator {

		private IMessageLogger logger;

		private ISymbolProvider symbolProvider;

		//private ProcessContext context;
		private ISymbolProcessor outProcessor;

		private List<Symbol<IValue>> inBuffer = new List<Symbol<IValue>>();
		private List<Symbol<IValue>> outBuffer = new List<Symbol<IValue>>();

		private bool interpretEveryIteration;
		private int interpretEveryIterationFrom;
		private int iterations;
		private int currIteration;

		private Stopwatch swDuration = new Stopwatch();
		private TimeSpan timeout;
		private bool aborting = false;
		private bool aborted = false;


		/// <summary>
		/// Number of current iteration. Zero-based, first iteration is 0, last is Iterations - 1.
		/// </summary>
		[Alias("currentIteration")]
		[UserGettable]
		public Constant CurrentIteration {
			get { return currIteration.ToConst(); }
		}

		[Alias("iterations", "i")]
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

		[Alias("interpretEveryIteration")]
		[UserSettable]
		public Constant InterpretEveryIteration {
			set {
				interpretEveryIteration = !value.IsZero;
				interpretEveryIterationFrom = 0;
			}
		}

		[Alias("interpretEveryIterationFrom")]
		[UserSettable]
		public Constant InterpretEveryIterationFrom {
			set {
				interpretEveryIteration = true;
				interpretEveryIterationFrom = Math.Max(0, value.RoundedIntValue);
			}
		}


		[UserConnectable(IsOptional = true)]
		public RandomGeneratorProvider RandomGeneratorProvider { private get; set; }


		#region IIterator Members

		[UserConnectable]
		public ISymbolProvider SymbolProvider {
			set { symbolProvider = value; }
		}

		[UserConnectable]
		public ISymbolProvider AxiomProvider { get; set; }

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

			start(doMeasure);

			symbolProvider.EndProcessing();

			swDuration.Stop();
		}

		public void Abort() {
			aborting = true;
		}

		#endregion


		private void start(bool doMeasure) {

			inBuffer.Clear();
			inBuffer.AddRange(AxiomProvider);

			for (currIteration = 0; currIteration <= iterations; currIteration++) {

				if (currIteration != 0) {
					rewriteIteration();
					if (aborted) { return; }
				}

				if ((interpretEveryIteration && currIteration >= interpretEveryIterationFrom) || currIteration == iterations) {
					if (doMeasure) {
						interpret(true);
						if (aborted) { return; }
					}

					interpret(false);
					if (aborted) { return; }
				}
			}

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

			if (RandomGeneratorProvider != null) {
				RandomGeneratorProvider.Reset();
			}

			outProcessor.BeginProcessing(measuring);

			foreach (var s in inBuffer) {

				if (swDuration.Elapsed > timeout || aborting) {
					logger.LogMessage(aborting ? Message.Abort : Message.Timeout, measuring ? "measuring" : "interpreting");
					aborted = true;

					outProcessor.EndProcessing();
					return;
				}

				outProcessor.ProcessSymbol(s);
			}

			outProcessor.EndProcessing();
		}


		public enum Message {

			[Message(MessageType.Error, "Time ran out while iterator was {0}.")]
			Timeout,
			[Message(MessageType.Error, "Iterator aborted while {0}.")]
			Abort,

		}
	}
}

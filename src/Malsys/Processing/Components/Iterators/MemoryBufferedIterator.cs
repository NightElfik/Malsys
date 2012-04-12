using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Malsys.Processing.Components.Common;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Processing.Components.RewriterIterators {
	/// <summary>
	/// Iterates L-system from connected symbol provider with connected rewriter.
	/// Buffers symbols from rewriter in memory.
	/// </summary>
	/// <name>Memory-buffered iterator</name>
	/// <group>Iterators</group>
	public class MemoryBufferedIterator : IIterator {

		protected IMessageLogger logger;

		protected ISymbolProvider symbolProvider;

		protected ISymbolProcessor outProcessor;

		protected List<Symbol<IValue>> inBuffer = new List<Symbol<IValue>>();
		protected List<Symbol<IValue>> outBuffer = new List<Symbol<IValue>>();

		protected bool interpretEveryIteration;
		protected int interpretEveryIterationFrom = -1;
		protected int iterations;
		protected bool resetAfterEachIter;
		protected int currIteration;

		protected Stopwatch swDuration = new Stopwatch();
		protected TimeSpan timeout;
		protected bool aborting = false;
		protected bool aborted = false;


		#region User gettable, settable and connectable properties

		/// <summary>
		/// Number of current iteration. Zero is axiom (no iteration was done), first iteration have number 1
		/// and last is equal to number of all iterations specified by Iterations property.
		/// </summary>
		[AccessName("currentIteration")]
		[UserGettable]
		public Constant CurrentIteration {
			get { return currIteration.ToConst(); }
		}


		/// <summary>
		/// Number of iterations to do with current L-system.
		/// </summary>
		/// <expected>Non-negative number representing number of iterations.</expected>
		/// <default>0</default>
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

		/// <summary>
		/// If set to true resets random generator with random seed after each iteration.
		/// This will cause that same sequence of random numbers will be generated each iteration.
		/// </summary>
		/// <expected>true or false</expected>
		/// <default>false</default>
		//[AccessName("resetRandomAfterEachIteration")]
		//[UserSettable]
		//public Constant ResetRandomAfterEachIteration {
		//    set {
		//        resetAfterEachIter = !value.IsZero;
		//    }
		//}

		/// <summary>
		/// If set to true iterator will send symbols from all iterations to connected interpret.
		/// Otherwise only result of last iteration is interpreted.
		/// </summary>
		/// <expected>true or false</expected>
		/// <default>false</default>
		[AccessName("interpretEveryIteration")]
		[UserSettable]
		public Constant InterpretEveryIteration {
			set {
				interpretEveryIteration = !value.IsZero;
				interpretEveryIterationFrom = -1;
			}
		}

		/// <summary>
		/// Sets interprets all iteration from given number.
		/// </summary>
		/// <expected>true or false</expected>
		/// <default>false</default>
		[AccessName("interpretEveryIterationFrom")]
		[UserSettable]
		public Constant InterpretEveryIterationFrom {
			set {
				interpretEveryIterationFrom = Math.Max(-1, value.RoundedIntValue);
			}
		}


		/// <summary>
		/// Iterator iterates symbols by reading all symbols from SymbolProvider every iteration.
		/// Rewriter should be connected as SymbolProvider and rewriters's SymbolProvider should be this Iterator.
		/// This setup creates loop and iterator rewrites string of symbols every iteration.
		/// </summary>
		[UserConnectable]
		public ISymbolProvider SymbolProvider {
			set { symbolProvider = value; }
		}

		/// <summary>
		/// Axiom provider component provides initial string of symbols.
		/// All symbols are read at begin of processing.
		/// </summary>
		[UserConnectable]
		public ISymbolProvider AxiomProvider { get; set; }

		/// <summary>
		/// Result string of symbols is sent to connected output processor.
		/// It should be InterpretrCaller who calls Interpreter and interprets symbols.
		/// </summary>
		[UserConnectable]
		public ISymbolProcessor OutputProcessor {
			set { outProcessor = value; }
		}

		/// <summary>
		/// Connected RandomGeneratorProvider's random generator is rested after each iteration
		/// if iterator is configured to do that (ResetRandomAfterEachIteration property is set to true).
		/// </summary>
		[UserConnectable(IsOptional = true)]
		public RandomGeneratorProvider RandomGeneratorProvider { private get; set; }


		#endregion





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

		public void Cleanup() { }

		public void BeginProcessing(bool measuring) { }

		public void EndProcessing() { }


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



		private bool interpretCurrentIteration() {

			if (interpretEveryIteration) {
				return true;
			}

			if (interpretEveryIterationFrom >= 0 && currIteration >= interpretEveryIterationFrom) {
				return true;
			}

			if (currIteration == iterations) {
				return true;
			}

			return false;
		}

		private void resetRendomGenerator() {
			if (RandomGeneratorProvider != null) {
				RandomGeneratorProvider.Reset();
			}
		}


		protected virtual void start(bool doMeasure) {

			inBuffer.Clear();
			inBuffer.AddRange(AxiomProvider);

			for (currIteration = 0; currIteration <= iterations; currIteration++) {

				if (currIteration != 0) {
					rewriteIteration();
					if (aborted) { return; }
				}

				if (interpretCurrentIteration()) {
					if (doMeasure) {
						resetRendomGenerator();
						interpret(true);
						resetRendomGenerator();
						if (aborted) { return; }
					}

					interpret(false);
					if (aborted) { return; }
				}
			}

		}

		protected void rewriteIteration() {

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

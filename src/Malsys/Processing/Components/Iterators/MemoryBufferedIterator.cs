// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Malsys.Processing.Components.Common;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Processing.Components.RewriterIterators {
	/// <summary>
	/// Iterates L-system from connected symbol provider with connected rewriter.
	/// Buffers symbols from rewriter in the memory.
	/// </summary>
	/// <name>Memory-buffered iterator</name>
	/// <group>Iterators</group>
	public class MemoryBufferedIterator : IIterator {

		protected List<Symbol<IValue>> inBuffer = new List<Symbol<IValue>>();
		protected List<Symbol<IValue>> outBuffer = new List<Symbol<IValue>>();

		protected int iterationsCount;
		protected int currIteration;

		protected Stopwatch swDuration = new Stopwatch();
		protected TimeSpan timeout;
		protected bool aborting = false;
		protected bool aborted = false;

		private HashSet<int> interpretFollowingIterationsCache = new HashSet<int>();
		private HashSet<int> skipFollowingIterationsCache = new HashSet<int>();




		#region User gettable & settable & connectable properties

		/// <summary>
		/// Number of current iteration. Zero is axiom (no iteration was done), first iteration have number 1
		/// and last is equal to number of all iterations specified by Iterations property.
		/// </summary>
		[AccessName("currentIteration")]
		[UserGettable]
		public Constant CurrentIteration {
			get { return currentIteration; }
		}
		private Constant currentIteration;

		/// <summary>
		/// Number of iterations to do with current L-system.
		/// </summary>
		/// <expected>Non-negative number representing number of iterations.</expected>
		/// <default>0</default>
		[AccessName("iterations", "i")]
		[UserGettable]
		[UserSettable]
		public Constant Iterations {
			get { return iterations; }
			set {
				if (value.IsNaN || value.Value < 0) {
					throw new InvalidUserValueException("Iterations value is invalid.");
				}
				iterations = value;
			}
		}
		private Constant iterations;

		/// <summary>
		/// If set to true iterator will send symbols from all iterations to connected interpret.
		/// Otherwise only result of last iteration is interpreted.
		/// </summary>
		/// <expected>true or false</expected>
		/// <default>false</default>
		[AccessName("interpretEveryIteration")]
		[UserSettable]
		public Constant InterpretEveryIteration { get; set; }

		/// <summary>
		/// Sets interprets all iteration from given number.
		/// </summary>
		/// <expected>positive number (lower than total number of iterations)</expected>
		/// <default>none</default>
		[AccessName("interpretEveryIterationFrom")]
		[UserSettable]
		public Constant InterpretEveryIterationFrom { get; set; }

		/// <summary>
		/// Array with numbers of iterations which will be interpreted.
		/// </summary>
		/// <expected>array of numbers</expected>
		/// <default>{} (empty array)</default>
		[AccessName("interpretFollowingIterations")]
		[UserSettable]
		public ValuesArray InterpretFollowingIterations {
			get {
				return interpretFollowingIterations;
			}
			set {
				if (!value.IsConstArray()) {
					throw new InvalidUserValueException("All members of array are not numbers.");
				}
				interpretFollowingIterations = value;
			}
		}
		private ValuesArray interpretFollowingIterations;

		/// <summary>
		/// Array with numbers of iterations which will be skipped (not interpreted).
		/// This field have priority over properties which are saying what iterations to interpret.
		/// </summary>
		/// <expected>array of numbers</expected>
		/// <default>{} (empty array)</default>
		[AccessName("skipFollowingIterations")]
		[UserSettable]
		public ValuesArray SkipFollowingIterations {
			get {
				return skipFollowingIterations;
			}
			set {
				if (!value.IsConstArray()) {
					throw new InvalidUserValueException("All members of array are not numbers.");
				}
				skipFollowingIterations = value;
			}
		}
		private ValuesArray skipFollowingIterations;


		/// <summary>
		/// Iterator iterates symbols by reading all symbols from SymbolProvider every iteration.
		/// Rewriter should be connected as SymbolProvider and rewriters's SymbolProvider should be this Iterator.
		/// This setup creates loop and iterator rewrites string of symbols every iteration.
		/// When number of iterations is set to 0 (of left default as 0) only axiom is used and this that case this property can be left unconnected.
		/// </summary>
		[UserConnectable(IsOptional = true)]
		public ISymbolProvider SymbolProvider { get; set; }

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
		public ISymbolProcessor OutputProcessor { get; set; }

		/// <summary>
		/// Connected RandomGeneratorProvider's random generator is rested after each iteration
		/// if iterator is configured to do that (ResetRandomAfterEachIteration property is set to true).
		/// </summary>
		[UserConnectable(IsOptional = true)]
		public RandomGeneratorProvider RandomGeneratorProvider { get; set; }

		#endregion





		public IMessageLogger Logger { get; set; }


		public void Reset() {
			currentIteration = Constant.MinusOne;
			Iterations = Constant.Zero;
			InterpretEveryIteration = Constant.False;
			InterpretEveryIterationFrom = Constant.MinusOne;
			InterpretFollowingIterations = SkipFollowingIterations = ValuesArray.Empty;
		}

		public void Initialize(ProcessContext ctxt) {
			timeout = ctxt.ProcessingTimeLimit;
			iterationsCount = iterations.RoundedIntValue;
			interpretFollowingIterationsCache.AddRange(interpretFollowingIterations.Select(x => ((Constant)x).RoundedIntValue));
			skipFollowingIterationsCache.AddRange(skipFollowingIterations.Select(x => ((Constant)x).RoundedIntValue));
		}

		public void Cleanup() {
			interpretFollowingIterationsCache.Clear();
			skipFollowingIterationsCache.Clear();
		}

		public void Dispose() { }


		public bool RequiresMeasure { get { return false; } }


		public void BeginProcessing(bool measuring) { }

		public void EndProcessing() { }


		public void Start(bool doMeasure) {

			swDuration.Restart();
			if (SymbolProvider != null) {
				SymbolProvider.BeginProcessing(false);
			}

			start(doMeasure);

			if (SymbolProvider != null) {
				SymbolProvider.EndProcessing();
			}

			swDuration.Stop();
		}

		public void Abort() {
			aborting = true;
		}

		public IEnumerator<Symbol<IValue>> GetEnumerator() {
			return inBuffer.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return inBuffer.GetEnumerator();
		}


		private void setCurrentIteration(int value) {
			currIteration = value;
			currentIteration = value.ToConst();
		}


		private bool interpretCurrentIteration() {

			if (skipFollowingIterationsCache.Contains(currIteration)) {
				return false;  // skip have priority
			}

			if (InterpretEveryIteration.IsTrue) {
				return true;
			}

			if (InterpretEveryIterationFrom.Value >= 0 && currIteration >= InterpretEveryIterationFrom.Value) {
				return true;
			}

			if (interpretFollowingIterationsCache.Contains(currIteration)) {
				return true;
			}

			if (currIteration == iterationsCount) {
				return true;
			}

			return false;
		}

		private int resetRendomGenerator() {
			if (RandomGeneratorProvider != null) {
				int seed = (int)(RandomGeneratorProvider.Random() * int.MaxValue);
				RandomGeneratorProvider.ResetRandomGenerator(seed);
				return seed;
			}
			return -1;
		}

		private void resetRendomGenerator(int seed) {
			if (RandomGeneratorProvider != null) {
				RandomGeneratorProvider.ResetRandomGenerator(seed);
			}
		}


		protected virtual void start(bool doMeasure) {

			inBuffer.Clear();
			inBuffer.AddRange(AxiomProvider);

			for (setCurrentIteration(0); currIteration <= iterationsCount; setCurrentIteration(currIteration + 1)) {

				if (currIteration != 0) {
					rewriteIteration();
					if (aborted) { return; }
				}

				if (interpretCurrentIteration()) {
					if (doMeasure) {
						// measure
						int seed = resetRendomGenerator();
						interpret(true);
						resetRendomGenerator(seed);
						if (aborted) { return; }
					}

					interpret(false);
					if (aborted) { return; }
				}
			}

		}

		protected void rewriteIteration() {

			if (SymbolProvider == null) {
				throw new ComponentException("SymbolProvider is not set.");
			}

			foreach (var symbol in SymbolProvider) {

				if (swDuration.Elapsed > timeout || aborting) {
					Logger.LogMessage(aborting ? Message.Abort : Message.Timeout, "rewriting iteration {0} of {1}".Fmt(currIteration, iterationsCount));
					aborted = true;
					return;
				}

				outBuffer.Add(symbol);

			}

			inBuffer.Clear();
			Swap.Them(ref inBuffer, ref outBuffer);

		}

		private void interpret(bool measuring) {

			OutputProcessor.BeginProcessing(measuring);

			foreach (var s in inBuffer) {

				if (swDuration.Elapsed > timeout || aborting) {
					Logger.LogMessage(aborting ? Message.Abort : Message.Timeout, measuring ? "measuring" : "interpreting");
					aborted = true;

					OutputProcessor.EndProcessing();
					return;
				}

				OutputProcessor.ProcessSymbol(s);
			}

			OutputProcessor.EndProcessing();
		}


		public enum Message {

			[Message(MessageType.Error, "Time ran out while iterator was {0}.")]
			Timeout,
			[Message(MessageType.Error, "Iterator aborted while {0}.")]
			Abort,

		}
	}
}

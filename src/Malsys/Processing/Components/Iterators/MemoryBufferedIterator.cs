using System;
using System.Collections.Generic;
using System.Diagnostics;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Processing.Components.RewriterIterators {
	[Component("Memory buffered iterator", ComponentGroupNames.Iterators)]
	public class MemoryBufferedIterator : IIterator {

		private IMessageLogger logger;

		private ISymbolProvider symbolProvider;

		private ProcessContext context;
		private ISymbolProcessor outProcessor;

		private List<Symbol<IValue>> inBuffer = new List<Symbol<IValue>>();
		private List<Symbol<IValue>> outBuffer = new List<Symbol<IValue>>();

		private int[] iterationsArray;

		private Stopwatch swDuration = new Stopwatch();
		private TimeSpan timeout;
		private bool aborting = false;
		private bool aborted = false;



		[UserSettable(IsMandatory = true)]
		public IValue Iterations {
			set {
				if (value.IsConstant && !((Constant)value).IsNaN && ((Constant)value).Value >= 0) {
					iterationsArray = new int[] { ((Constant)value).RoundedIntValue };
				}
				else if (value.IsConstArray()) {
					ValuesArray arr = (ValuesArray)value;
					iterationsArray = new int[arr.Length];
					for (int i = 0; i < arr.Length; i++) {
						if (((Constant)arr[i]).IsNaN || ((Constant)arr[i]).Value < 0) {
							throw new InvalidUserValueException("Iterations value `{0}` in given array is invalid (at index {1})."
								.Fmt(arr[i].ToString(), i));
						}
						iterationsArray[i] = ((Constant)arr[i]).RoundedIntValue;
					}
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
		public ISymbolProvider AxiomProvider { get; set; }

		[UserConnectable]
		public ISymbolProcessor OutputProcessor {
			set { outProcessor = value; }
		}

		public IEnumerator<Symbol<IValue>> GetEnumerator() {
			return inBuffer.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return inBuffer.GetEnumerator();
		}
		public bool RequiresMeasure {
			get { return false; }
		}

		public void Initialize(ProcessContext ctxt) {
			logger = ctxt.Logger;
		}

		public void Cleanup() {

		}

		public void BeginProcessing(bool measuring) {

		}

		public void EndProcessing() {

		}

		public void Start(bool doMeasure, TimeSpan timeout) {

			this.timeout = timeout;
			swDuration.Restart();

			foreach (var iter in iterationsArray) {
				start(doMeasure, iter);

				if (aborted) {
					break;
				}
			}

			swDuration.Stop();
		}

		public void Abort() {
			aborting = true;
		}

		#endregion


		private void start(bool doMeasure, int iterations) {

			inBuffer.Clear();
			inBuffer.AddRange(AxiomProvider);

			rewrite(iterations);

			if (aborted) {
				return;
			}

			if (doMeasure) {
				interpret(true);
			}

			if (aborted) {
				return;
			}

			interpret(false);
		}

		private void rewrite(int iterations) {

			for (int i = 0; i < iterations; i++) {

				symbolProvider.BeginProcessing(true);

				foreach (var symbol in symbolProvider) {

					outBuffer.Add(symbol);

					if (swDuration.Elapsed > timeout || aborting) {
						symbolProvider.EndProcessing();
						logger.LogMessage(aborting ? Message.Abort : Message.Timeout, "rewriting iteration {0} of {1}".Fmt(i, iterations));
						aborted = true;
						return;
					}

				}

				symbolProvider.EndProcessing();

				inBuffer.Clear();
				Swap.Them(ref inBuffer, ref outBuffer);
			}
		}

		private void interpret(bool measuring) {

			outProcessor.BeginProcessing(measuring);
			foreach (var s in inBuffer) {

				if (swDuration.Elapsed > timeout || aborting) {
					outProcessor.EndProcessing();
					logger.LogMessage(aborting ? Message.Abort : Message.Timeout, measuring ? "measuring" : "interpreting");
					aborted = true;
					return;
				}

				outProcessor.ProcessSymbol(s);
			}
			outProcessor.EndProcessing();
		}


		public enum Message {

			[Message(MessageType.Error, "Iterator aborted while {0}.")]
			Timeout,
			[Message(MessageType.Error, "Iterator aborted while {0}.")]
			Abort,

		}
	}
}

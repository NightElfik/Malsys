// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Processing.Components.RewriterIterators {
	/// <summary>
	/// Specialized iterator for iterating inner L-systems.
	/// Axiom is directly in iterator as property to optimize number of components.
	/// AxiomProvider property is ignored.
	/// </summary>
	/// <name>Inner L-system iterator</name>
	/// <group>Iterators</group>
	public class InnerLsystemIterator : MemoryBufferedIterator {

		/// <summary>
		/// Axiom is directly in iterator to optimize number of components.
		/// </summary>
		[AccessName("axiom")]
		[UserSettableSybols(IsMandatory = true)]
		public ImmutableList<Symbol<IValue>> Axiom { private get; set; }

		/// <summary>
		/// To allow not connecting AxiomProvider component.
		/// </summary>
		[UserConnectable(IsOptional=true)]
		new public ISymbolProvider AxiomProvider { get; set; }


		protected override void start(bool doMeasure) {
			// never do measure pass
			inBuffer.Clear();
			inBuffer.AddRange(Axiom);

			for (currIteration = 0; currIteration < iterationsCount; currIteration++) {

				rewriteIteration();
				if (aborted) { return; }

			}

			interpret();
		}

		private void interpret() {

			// do not begin processing of output processor -- we are inner L-system

			foreach (var s in inBuffer) {

				if (swDuration.Elapsed > timeout || aborting) {
					Logger.LogMessage(aborting ? Message.Abort : Message.Timeout, "interpreting");
					aborted = true;
					return;
				}

				OutputProcessor.ProcessSymbol(s);
			}

		}
	}
}

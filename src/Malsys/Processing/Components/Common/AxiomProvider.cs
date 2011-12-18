using System.Collections.Generic;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Processing.Components.Common {
	public class AxiomProvider : ISymbolProvider {


		[UserSettable]
		public ImmutableList<Symbol<IValue>> Axiom { get; set; }


		public IEnumerator<Symbol<IValue>> GetEnumerator() {
			return Axiom.GetEnumerator();
		}


		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return Axiom.GetEnumerator();
		}


		public bool RequiresMeasure {
			get { return false; }
		}

		public void Initialize(ProcessContext ctxt) { }

		public void Cleanup() { }

		public void BeginProcessing(bool measuring) { }

		public void EndProcessing() { }

	}
}

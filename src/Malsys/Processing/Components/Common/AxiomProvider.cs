using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Processing.Components.Common {
	public class AxiomProvider : SymbolProvider, ISymbolProvider {

		public AxiomProvider() : base() { }

		/// <summary>
		/// Storage for axiom.
		/// Value is provided to connected component.
		/// </summary>
		[AccessName("axiom")]
		[UserSettableSybols]
		public ImmutableList<Symbol<IValue>> Axiom { set { Symbols = value; } }


	}
}

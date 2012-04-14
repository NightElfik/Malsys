using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Processing.Components.Common {
	/// <summary>
	///	Provides symbols set by user to Axiom property.
	/// </summary>
	/// <name>Axiom provider</name>
	/// <group>Common</group>
	public class AxiomProvider : SymbolProvider, ISymbolProvider {


		/// <summary>
		/// Storage for axiom.
		/// Value is provided to connected component.
		/// </summary>
		[AccessName("axiom")]
		[UserSettableSybols]
		public ImmutableList<Symbol<IValue>> Axiom { set { Symbols = value; } }


	}
}

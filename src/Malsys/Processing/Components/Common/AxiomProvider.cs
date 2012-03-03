using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Processing.Components.Common {
	[Component("Axiom provider", ComponentGroupNames.Common)]
	public class AxiomProvider : SymbolProvider, ISymbolProvider {


		[UserSettableSybols(IsMandatory = true)]
		public ImmutableList<Symbol<IValue>> Axiom { set { Symbols = value; } }


	}
}

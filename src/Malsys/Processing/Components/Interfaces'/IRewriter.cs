using Malsys.SemanticModel.Evaluated;

namespace Malsys.Processing.Components {
	[Component("Rewriter container", ComponentGroupNames.Rewriters)]
	public interface IRewriter : ISymbolProvider {

		ISymbolProvider SymbolProvider { set; }

	}
}

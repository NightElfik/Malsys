using Malsys.SemanticModel.Evaluated;

namespace Malsys.Processing.Components {
	public interface IRewriter : ISymbolProvider {

		ISymbolProvider SymbolProvider { set; }

	}
}

using Malsys.SemanticModel.Evaluated;

namespace Malsys.Processing.Components {
	/// <name>Symbol rewriter container</name>
	/// <group>Rewriters</group>
	public interface IRewriter : ISymbolProvider {

		ISymbolProvider SymbolProvider { set; }

	}
}

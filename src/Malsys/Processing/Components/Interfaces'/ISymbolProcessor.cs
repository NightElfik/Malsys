using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Processing.Components {
	public interface ISymbolProcessor : IProcessComponent {

		void ProcessSymbol(Symbol<IValue> symbol);

	}
}

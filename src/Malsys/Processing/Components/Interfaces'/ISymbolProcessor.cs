using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Processing.Components {
	[Component("Generic symbol processor", ComponentGroupNames.Common)]
	public interface ISymbolProcessor : IComponent {

		void ProcessSymbol(Symbol<IValue> symbol);

	}
}

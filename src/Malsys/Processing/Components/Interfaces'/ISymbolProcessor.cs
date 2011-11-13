using Malsys.Expressions;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Processing.Components {
	public interface ISymbolProcessor {

		void BeginProcessing();

		void ProcessSymbol(Symbol<IValue> symbol);

		void EndProcessing();

	}
}

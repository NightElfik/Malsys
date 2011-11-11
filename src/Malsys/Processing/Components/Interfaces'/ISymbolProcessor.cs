using Malsys.Expressions;

namespace Malsys.Processing.Components {
	public interface ISymbolProcessor {

		void BeginProcessing();

		void ProcessSymbol(Symbol<IValue> symbol);

		void EndProcessing();

	}
}

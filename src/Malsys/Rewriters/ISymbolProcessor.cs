using Malsys.Expressions;

namespace Malsys.Rewriters {
	public interface ISymbolProcessor {

		void BeginProcessing();

		void ProcessSymbol(Symbol<IValue> symbol);

		void EndProcessing();

	}
}

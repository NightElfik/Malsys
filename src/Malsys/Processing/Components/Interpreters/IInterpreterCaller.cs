using System.Collections.Generic;

namespace Malsys.Processing.Components.Interpreters {
	public interface IInterpreterCaller : ISymbolProcessor {

		IInterpreter Interpreter { set; }

		Dictionary<string, string> SymbolsInterpretation { set; }

	}
}

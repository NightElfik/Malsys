using System.Collections.Generic;

namespace Malsys.Processing.Components {
	public interface IInterpreterCaller : ISymbolProcessor {

		IInterpreter Interpreter { set; }

		ProcessContext Context { set; }

	}
}

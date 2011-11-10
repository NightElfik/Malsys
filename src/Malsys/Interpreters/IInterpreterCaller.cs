using System.Collections.Generic;
using Malsys.Rewriters;

namespace Malsys.Interpreters {
	public interface IInterpreterCaller : ISymbolProcessor {

		IInterpreter Interpreter { set; }

		Dictionary<string, string> SymbolsInterpretation { set; }

	}
}

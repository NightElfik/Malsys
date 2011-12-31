
namespace Malsys.Processing.Components {
	[Component("Generic interpreter caller", ComponentGroupNames.Interpreters)]
	public interface IInterpreterCaller : ISymbolProcessor {

		[UserConnectable]
		IInterpreter Interpreter { set; }

	}
}

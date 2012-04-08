
namespace Malsys.Processing.Components {
	/// <name>Interpreter caller container</name>
	/// <group>Interpreters</group>
	public interface IInterpreterCaller : ISymbolProcessor {

		[UserConnectable]
		IInterpreter Interpreter { set; }

	}
}

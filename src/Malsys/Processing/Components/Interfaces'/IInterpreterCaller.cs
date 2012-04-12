
namespace Malsys.Processing.Components {
	/// <summary>
	///	Interpreter callers are responsible for converting symbols for calls of interpreter methods.
	/// </summary>
	/// <name>Interpreter caller interface</name>
	/// <group>Interpreters</group>
	public interface IInterpreterCaller : ISymbolProcessor {

		/// <summary>
		/// Interpreter on which will be interpretation methods called.
		/// </summary>
		[UserConnectable]
		IInterpreter Interpreter { set; }

	}
}

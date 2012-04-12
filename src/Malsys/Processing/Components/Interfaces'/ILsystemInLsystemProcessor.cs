using Malsys.SemanticModel.Evaluated;

namespace Malsys.Processing.Components.Common {
	/// <summary>
	///	This is special interface component for interpreting L-system symbol as another L-system.
	/// </summary>
	/// <name>Inner L-system processor interface</name>
	/// <group>Special</group>
	public interface ILsystemInLsystemProcessor : IComponent {

		/// <summary>
		/// Interpreter of main L-system.
		/// All symbols from inner L-system should be processed with same interpreter as main L-system.
		/// </summary>
		[UserConnectable]
		IInterpreter Interpreter { set; }

		void ProcessLsystem(string name, string configName, IValue[] args);

	}
}

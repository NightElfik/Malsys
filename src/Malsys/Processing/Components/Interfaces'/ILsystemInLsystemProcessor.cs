using Malsys.SemanticModel.Evaluated;

namespace Malsys.Processing.Components.Common {
	public interface ILsystemInLsystemProcessor : IComponent {

		[UserConnectable]
		IInterpreter Interpreter { set; }

		void ProcessLsystem(string name, string configName, IValue[] args);

	}
}

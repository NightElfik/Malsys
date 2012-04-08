
namespace Malsys.Processing.Components {
	/// <name>Interpreter container</name>
	/// <group>Interpreters</group>
	public interface IInterpreter : IProcessComponent {

		IRenderer Renderer { set; }

	}
}

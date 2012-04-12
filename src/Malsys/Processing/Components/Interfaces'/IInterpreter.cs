
namespace Malsys.Processing.Components {
	/// <summary>
	/// Interpreters are responsible for interpreting symbols of L-system.
	/// </summary>
	/// <name>Interpreter interface</name>
	/// <group>Interpreters</group>
	public interface IInterpreter : IProcessComponent {

		IRenderer Renderer { set; }

	}
}

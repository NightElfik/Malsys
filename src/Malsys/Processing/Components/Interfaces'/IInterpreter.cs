
namespace Malsys.Processing.Components {

	[Component("Interpreter container", ComponentGroupNames.Interpreters)]
	public interface IInterpreter : IProcessComponent {

		IRenderer Renderer { set; }

		bool IsRendererCompatible(IRenderer renderer);

	}
}

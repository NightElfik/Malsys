
namespace Malsys.Processing.Components {

	public interface IInterpreter : IComponent {

		IRenderer Renderer { set; }

		bool IsRendererCompatible(IRenderer renderer);

	}
}

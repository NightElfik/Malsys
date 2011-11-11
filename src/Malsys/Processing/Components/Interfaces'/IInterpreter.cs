
namespace Malsys.Processing.Components {

	public interface IInterpreter {

		IRenderer Renderer { set; }

		bool IsRendererCompatible(IRenderer renderer);

		void BeginInterpreting();

		void EndInterpreting();

	}
}

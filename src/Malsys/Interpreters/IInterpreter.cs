using Malsys.Renderers;

namespace Malsys.Interpreters {

	public interface IInterpreter {

		IRenderer Renderer { set; }

		bool IsRendererCompatible(IRenderer renderer);

		void BeginInterpreting();

		void EndInterpreting();

	}
}


namespace Malsys.Processing.Components.Renderers {
	public interface ITextRenderer : IRenderer {

		/// <summary>
		/// Writes given character at given coordinates.
		/// Coordinates can be both positive and negative.
		/// </summary>
		void PutCharAt(char c, int x, int y);

	}
}

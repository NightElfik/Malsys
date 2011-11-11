using System.Collections.Generic;
using Malsys.Media;

namespace Malsys.Processing.Components.Renderers {
	public interface IRenderer2D : IRenderer {

		void MoveTo(PointF point, ColorF color, float width);

		void LineTo(PointF point, ColorF color, float width);

		void DrawPolygon(IEnumerable<PointF> points, ColorF color);

	}
}

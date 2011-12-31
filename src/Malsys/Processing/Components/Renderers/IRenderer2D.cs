using System.Collections.Generic;
using Malsys.Media;

namespace Malsys.Processing.Components.Renderers {
	public interface IRenderer2D : IRenderer {

		/// <remarks>
		/// Initialization of state is important for optimization of measure operations.
		/// With initialization did by MoveTo, it has to be tested all x/y min/max variables,
		/// But with initialized values one point can be only min x/y xor max x/y.
		/// </remarks>
		void InitializeState(PointF point, float width, ColorF color);

		void MoveTo(PointF point, float width, ColorF color);

		void LineTo(PointF point, float width, ColorF color);

		void DrawPolygon(Polygon2D polygon);

	}
}

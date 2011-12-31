using System.Collections.Generic;
using Malsys.Media;

namespace Malsys.Processing.Components.Renderers {
	[Component("Generic 2D renderer", ComponentGroupNames.Renderers)]
	public interface IRenderer2D : IRenderer {

		/// <remarks>
		/// Initialization of state is important for optimization of measure operations.
		/// With initialization did by first operation MoveTo/LineTo it has to be tested
		/// all x/y min/max variables in each measure operation (measure have to do if-then, if-then for each axis).
		/// But with initialized values each next point point can be only min x/y xor max x/y (measure can do if-then-else).
		/// </remarks>
		void InitializeState(PointF point, float width, ColorF color);

		void MoveTo(PointF point, float width, ColorF color);

		void LineTo(PointF point, float width, ColorF color);

		void DrawPolygon(Polygon2D polygon);

	}
}

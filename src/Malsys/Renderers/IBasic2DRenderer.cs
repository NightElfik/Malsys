using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Malsys.Media;

namespace Malsys.Renderers {
	public interface IBasic2DRenderer {

		void DrawTo(PointF Point, float Width, ColorF Color);

		void LineTo(PointF Point, float Width, ColorF Color);

		void DrawPolygon(IEnumerable<PointF> Points, ColorF Color);

	}
}

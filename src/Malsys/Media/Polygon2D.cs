using System.Collections.Generic;

namespace Malsys.Media {
	public class Polygon2D {

		public List<PointF> Ponits;

		public ColorF Color;

		public float StrokeWidth;

		public ColorF StrokeColor;


		public Polygon2D(ColorF color, float strokeWidth, ColorF strokeColor) {

			Color = color;
			StrokeColor = strokeColor;
			StrokeWidth = strokeWidth;

			Ponits = new List<PointF>();

		}


	}
}

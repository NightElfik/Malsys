using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace Malsys.Media {
	/// <remarks>
	/// I know that polygon is by definition 2D object.
	/// </remarks>
	public class Polygon3D {

		public List<Point3D> Ponits;

		public ColorF Color;

		public double StrokeWidth;

		public ColorF StrokeColor;


		public Polygon3D(ColorF color, double strokeWidth, ColorF strokeColor) {

			Color = color;
			StrokeColor = strokeColor;
			StrokeWidth = strokeWidth;

			Ponits = new List<Point3D>();

		}

	}
}

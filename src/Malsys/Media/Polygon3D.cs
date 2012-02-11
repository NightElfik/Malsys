using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace Malsys.Media {
	/// <summary>
	/// </summary>
	/// <remarks>
	/// I know that polygon is by definition 2D.
	/// </remarks>
	public class Polygon3D {

		public List<Point3D> Ponits;

		public ColorF Color;

		public float StrokeWidth;

		public ColorF StrokeColor;


		public Polygon3D(ColorF color, float strokeWidth, ColorF strokeColor) {

			Color = color;
			StrokeColor = strokeColor;
			StrokeWidth = strokeWidth;

			Ponits = new List<Point3D>();

		}

	}
}

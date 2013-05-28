// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace Malsys.Media {
	/// <remarks>
	/// I know that polygon is by definition 2D object.
	/// </remarks>
	public class Polygon3D {

		public List<Point3D> Ponits;
		public List<Quaternion> Rotations;

		public ColorF Color;

		public double StrokeWidth;

		public ColorF StrokeColor;


		public Polygon3D(ColorF color, double strokeWidth, ColorF strokeColor) {

			Color = color;
			StrokeColor = strokeColor;
			StrokeWidth = strokeWidth;

			Ponits = new List<Point3D>();
			Rotations = new List<Quaternion>();

		}


		public void Add(Point3D pt, Quaternion rotation) {
			Ponits.Add(pt);
			Rotations.Add(rotation);
		}

	}
}

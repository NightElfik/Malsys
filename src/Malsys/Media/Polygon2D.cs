// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System.Collections.Generic;
using System.Windows;

namespace Malsys.Media {
	public class Polygon2D {

		public List<Point> Ponits;

		public ColorF Color;

		public double StrokeWidth;

		public ColorF StrokeColor;


		public Polygon2D(ColorF color, double strokeWidth, ColorF strokeColor) {

			Color = color;
			StrokeColor = strokeColor;
			StrokeWidth = strokeWidth;

			Ponits = new List<Point>();

		}


	}
}

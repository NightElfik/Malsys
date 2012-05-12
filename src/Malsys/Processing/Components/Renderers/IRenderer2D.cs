/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System.Windows;
using Malsys.Media;

namespace Malsys.Processing.Components.Renderers {
	/// <summary>
	/// Provides commands for rendering of 2D image.
	/// </summary>
	/// <name>2D renderer interface</name>
	/// <group>Renderers</group>
	public interface IRenderer2D : IRenderer {

		/// <remarks>
		/// Initialization of the state is important for optimization of measure operations.
		/// With initialization did by the first operation MoveTo/LineTo it has to be tested
		/// all x/y min/max variables in each measure operation (measure have to do if-then, if-then for each axis).
		/// But with initialized values each next point point can be only min x/y xor max x/y (measure can do if-then-else).
		/// </remarks>
		void InitializeState(Point point, double width, ColorF color);

		/// <summary>
		/// Moves to given point with given width and color.
		/// </summary>
		void MoveTo(Point point, double width, ColorF color);

		/// <summary>
		/// Draws to given point with given width and color.
		/// </summary>
		void DrawTo(Point point, double width, ColorF color);

		void DrawPolygon(Polygon2D polygon);

		void DrawCircle(Point center, double radius, ColorF color);

	}
}

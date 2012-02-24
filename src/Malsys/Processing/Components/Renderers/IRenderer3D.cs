﻿using System.Windows.Media.Media3D;
using Malsys.Media;

namespace Malsys.Processing.Components.Renderers {
	[Component("Generic 3D renderer", ComponentGroupNames.Renderers)]
	public interface IRenderer3D : IRenderer {

		/// <remarks>
		/// Initialization of the state is important for optimization of measure operations.
		/// With initialization did by the first operation MoveTo/LineTo it has to be tested
		/// all x/y min/max variables in each measure operation (measure have to do if-then, if-then for each axis).
		/// But with initialized values each next point point can be only min x/y xor max x/y (measure can do if-then-else).
		/// </remarks>
		void InitializeState(Point3D startPoint, Quaternion rotation, double width, ColorF color);

		/// <summary>
		/// Moves to given point with given orientation, width and color.
		/// </summary>
		void MoveTo(Point3D endPoint, Quaternion rotation, double width, ColorF color);

		/// <summary>
		/// Draws to given point with given orientation, width and color.
		/// </summary>
		void DrawTo(Point3D endPoint, Quaternion rotation, double width, ColorF color);

		void DrawPolygon(Polygon3D polygon);

	}
}
/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System.Windows;
using Malsys.Media;

namespace Malsys.Processing.Components.Renderers {
	/// <summary>
	/// Prints called commands instead of creating some 2D output.
	/// This is handy if 2D renderer is debugged.
	/// </summary>
	/// <name>2D renderer debugger</name>
	/// <group>Renderers</group>
	public class DebugRenderer2D : DebugRendererBase, IRenderer2D {

		public static readonly string InitName = "InitializeState";
		public static readonly string MoveToName = "MoveTo";
		public static readonly string DrawToName = "DrawTo";
		public static readonly string DrawPolygonName = "DrawPolygon";
		public static readonly string DrawCircleName = "DrawCircle";


		private string logFormat;


		public DebugRenderer2D()
			: this("{0}|{1}|{2}|{3}") {

		}

		public DebugRenderer2D(string logFormat) {
			this.logFormat = logFormat;
		}


		#region IRenderer2D Members

		public void InitializeState(Point point, double width, ColorF color) {
			logState(logFormat, InitName, point, width, color);
		}

		public void MoveTo(Point point, double width, ColorF color) {
			logState(logFormat, MoveToName, point, width, color);
		}

		public void DrawTo(Point point, double width, ColorF color) {
			logState(logFormat, DrawToName, point, width, color);
		}

		public void DrawPolygon(Polygon2D polygon) {
			logState(DrawPolygonName);
		}

		public void DrawCircle(Point center, double radius, ColorF color) {
			logState(logFormat, DrawCircleName, center, radius, color);
		}

		#endregion
	}
}

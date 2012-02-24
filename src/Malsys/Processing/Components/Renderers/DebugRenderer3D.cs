﻿using System.Windows.Media.Media3D;
using Malsys.Media;

namespace Malsys.Processing.Components.Renderers {
	public class DebugRenderer3D : DebugRendererBase, IRenderer3D {

		public static readonly string InitName = "InitializeState";
		public static readonly string MoveToName = "MoveTo";
		public static readonly string DrawToName = "DrawTo";
		public static readonly string DrawPolygonName = "DrawPolygon";


		private string logFormat;


		public DebugRenderer3D(string logFormat = "{0}|{1}|{2}|{3}|{4}") {
			this.logFormat = logFormat;
		}


		#region IRenderer3D Members

		public void InitializeState(Point3D startPoint, Quaternion rotation, double width, ColorF color) {
			logState(logFormat, InitName, startPoint, rotation, width, color);
		}

		public void MoveTo(Point3D endPoint, Quaternion rotation, double width, ColorF color) {
			logState(logFormat, MoveToName, endPoint, rotation, width, color);
		}

		public void DrawTo(Point3D endPoint, Quaternion rotation, double width, ColorF color) {
			logState(logFormat, DrawToName, endPoint, rotation, width, color);
		}

		public void DrawPolygon(Polygon3D polygon) {
			logState(DrawPolygonName);
		}

		#endregion
	}
}

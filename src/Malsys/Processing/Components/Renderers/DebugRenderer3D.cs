using System.Windows.Media.Media3D;
using Malsys.Media;

namespace Malsys.Processing.Components.Renderers {
	/// <summary>
	/// Prints called commands instead of creating some 3D output.
	/// This is handy if 3D scene renderer is debugged.
	/// </summary>
	/// <name>3D renderer debugger</name>
	/// <group>Renderers</group>
	public class DebugRenderer3D : DebugRendererBase, IRenderer3D {

		public static readonly string InitName = "InitializeState";
		public static readonly string MoveToName = "MoveTo";
		public static readonly string DrawToName = "DrawTo";
		public static readonly string DrawPolygonName = "DrawPolygon";
		public static readonly string DrawSphereName = "DrawSphere";


		private string logFormat;


		public DebugRenderer3D()
			: this("{0}|{1}|{2}|{3}|{4}") {

		}

		public DebugRenderer3D(string logFormat) {
			this.logFormat = logFormat;
		}


		#region IRenderer3D Members

		public void InitializeState(Point3D startPoint, Quaternion rotation, double width, ColorF color) {
			logState(logFormat, InitName, startPoint, rotation, width, color);
		}

		public void MoveTo(Point3D endPoint, Quaternion rotation, double width, ColorF color) {
			logState(logFormat, MoveToName, endPoint, rotation, width, color);
		}

		public void DrawTo(Point3D endPoint, Quaternion rotation, double width, ColorF color, double quality) {
			logState(logFormat, DrawToName, endPoint, rotation, width, color);
		}

		public void DrawPolygon(Polygon3D polygon) {
			logState(DrawPolygonName);
		}

		public void DrawSphere(Point3D center, Quaternion rotation, double radius, ColorF color, double quality) {
			logState(logFormat, DrawSphereName, center, rotation, radius, color);
		}

		#endregion
	}
}

using System.Windows;
using System.Windows.Media.Media3D;
using Malsys.Media;

namespace Malsys.Processing.Components.Renderers {
	/// <summary>
	/// Renderer which does nothing.
	/// </summary>
	public class EmptyRenderer : IRenderer2D, IRenderer3D {
		#region IRenderer2D Members

		public void InitializeState(Point startPoint, double width, Media.ColorF color) { }

		public void MoveTo(Point endPoint, double width, Media.ColorF color) { }

		public void DrawTo(Point endPoint, double width, Media.ColorF color) { }

		public void DrawPolygon(Polygon2D polygon) { }

		public void DrawCircle(double radius, Media.ColorF color) { }

		#endregion IRenderer2D Members

		#region IRenderer Members

		public void AddGlobalOutputData(string key, object value) { }

		public void AddCurrentOutputData(string key, object value) { }

		#endregion IRenderer Members

		#region IProcessComponent Members

		public bool RequiresMeasure { get { return false; } }

		public void BeginProcessing(bool measuring) { }

		public void EndProcessing() { }

		#endregion IProcessComponent Members

		#region IComponent Members

		public IMessageLogger Logger { get; set; }

		public void Reset() { }

		public void Initialize(ProcessContext context) {
		}
		public void Cleanup() { }

		public void Dispose() { }

		#endregion IComponent Members

		#region IRenderer3D Members

		public void InitializeState(Point3D startPoint, Quaternion rotation, double width, Media.ColorF color) { }

		public void MoveTo(Point3D endPoint, Quaternion rotation, double width, Media.ColorF color) { }

		public void DrawTo(Point3D endPoint, Quaternion rotation, double width, Media.ColorF color, double quality) { }

		public void DrawPolygon(Polygon3D polygon) { }

		public void DrawSphere(double radius, Media.ColorF color, double quality) { }

		#endregion IRenderer3D Members
	}
}

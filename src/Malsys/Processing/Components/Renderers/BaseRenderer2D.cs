using System.IO;
using System.Windows;
using Malsys.Media;
using Microsoft.FSharp.Collections;

namespace Malsys.Processing.Components.Renderers {
	/// <summary>
	/// Provides implementation of common functionality of all 2D renderers.
	/// </summary>
	/// <name>2D renderer base</name>
	/// <group>Renderers</group>
	public abstract class BaseRenderer2D : IRenderer2D {

		protected ProcessContext context;
		/// <summary>
		/// Output stream must be obtained in BeginProcessing method in process pass
		/// to be possible to add metadata during processing.
		/// </summary>
		protected Stream outputStream;

		protected FSharpMap<string, object> globalMetadata;

		protected bool measuring;

		protected Point measuredMin, currentMeasuredMin;
		protected Point measuredMax, currentMeasuredMax;


		public IMessageLogger Logger { get; set; }

		/// <summary>
		/// Resets global metadata.
		/// </summary>
		public virtual void Reset() {
			globalMetadata = MapModule.Empty<string, object>();
		}

		/// <summary>
		/// Saves the context to variable. Should be called at the beginning of derived method.
		/// </summary>
		public virtual void Initialize(ProcessContext ctxt) {
			context = ctxt;
		}

		/// <summary>
		/// Cleans context.
		/// </summary>
		public virtual void Cleanup() {
			context = null;
		}

		public virtual void Dispose() { }


		public virtual bool RequiresMeasure { get { return false; } }


		/// <summary>
		/// Saves the measuring state.
		/// </summary>
		/// <remarks>
		/// This method is here (not in-lined in derived) because methods in this class are working with measuring variable,
		/// this improved readability of this class and ensures consistency (derived class do not have to "do something", just to call base method).
		/// </remarks>
		public virtual void BeginProcessing(bool measuring) {
			this.measuring = measuring;
		}

		/// <summary>
		/// Saves measured data (in measuring pass).
		/// </summary>
		public virtual void EndProcessing() {
			if (measuring) {
				measuredMin = currentMeasuredMin;
				measuredMax = currentMeasuredMax;
			}
		}


		#region IRenderer2D Members

		public virtual void AddGlobalOutputData(string key, object value) {
			globalMetadata = globalMetadata.Add(key, value);
		}

		public virtual void AddCurrentOutputData(string key, object value) {
			if (outputStream != null) {
				context.OutputProvider.AddMetadata(outputStream, key, value);
			}
		}


		/// <summary>
		/// Initializes measuring (in measure pass) or initial point, width and color (in process pass).
		/// </summary>
		public virtual void InitializeState(Point startPoint, double width, ColorF color) {
			if (measuring) {
				// this is important for optimizing measuring
				currentMeasuredMin.X = startPoint.X - width / 2;
				currentMeasuredMin.Y = startPoint.Y - width / 2;
				currentMeasuredMax.X = startPoint.X + width / 2;
				currentMeasuredMax.Y = startPoint.Y + width / 2;
			}
		}

		public abstract void MoveTo(Point endPoint, double width, ColorF color);

		public abstract void DrawTo(Point endPoint, double width, ColorF color);

		public abstract void DrawPolygon(Polygon2D polygon);

		public abstract void DrawCircle(double radius, ColorF color);

		#endregion IRenderer2D Members


		/// <summary>
		/// Ensures that circle (point + radius) with given center and radius will be in measured rectangle.
		/// </summary>
		protected void measure(double x, double y, double radius = 0) {

			if (x - radius < currentMeasuredMin.X) {
				currentMeasuredMin.X = x - radius;
			}
			if (x + radius > currentMeasuredMax.X) {
				currentMeasuredMax.X = x + radius;
			}

			if (y - radius < currentMeasuredMin.Y) {
				currentMeasuredMin.Y = y - radius;
			}
			if (y + radius > currentMeasuredMax.Y) {
				currentMeasuredMax.Y = y + radius;
			}
		}

		/// <summary>
		/// Ensures that circle (point + radius) with given center and radius center will be in measured rectangle.
		/// </summary>
		protected void measure(Point pt, double radius = 0) {

			if (pt.X - radius < currentMeasuredMin.X) {
				currentMeasuredMin.X = pt.X - radius;
			}
			if (pt.X + radius > currentMeasuredMax.X) {
				currentMeasuredMax.X = pt.X + radius;
			}

			if (pt.Y - radius < currentMeasuredMin.Y) {
				currentMeasuredMin.Y = pt.Y - radius;
			}
			if (pt.Y + radius > currentMeasuredMax.Y) {
				currentMeasuredMax.Y = pt.Y + radius;
			}
		}

	}
}

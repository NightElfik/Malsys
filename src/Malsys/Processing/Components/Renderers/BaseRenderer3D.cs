// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System.IO;
using System.Windows.Media.Media3D;
using Malsys.Media;
using Microsoft.FSharp.Collections;

namespace Malsys.Processing.Components.Renderers {
	/// <summary>
	/// Provides implementation of common functionality of all 3D renderers.
	/// </summary>
	/// <name>3D renderer base</name>
	/// <group>Renderers</group>
	public abstract class BaseRenderer3D : IRenderer3D {

		protected ProcessContext context;
		/// <summary>
		/// Output stream must be obtained in BeginProcessing method in process pass
		/// to be possible to add metadata during processing.
		/// </summary>
		protected Stream outputStream;

		protected FSharpMap<string, object> globalMetadata;

		protected bool measuring;

		protected Point3D lastPoint;
		protected Quaternion lastRotation;
		protected double lastWidth;
		protected ColorF lastColor;

		protected Point3D measuredMin, currentMeasuredMin;
		protected Point3D measuredMax, currentMeasuredMax;



		public IMessageLogger Logger { get; set; }

		/// <summary>
		/// Resets global metadata.
		/// </summary>
		public virtual void Reset() {
			globalMetadata = MapModule.Empty<string, object>();
		}

		/// <summary>
		/// Saves the context to variable.
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



		#region IRenderer3D Members

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
		public virtual void InitializeState(Point3D startPoint, Quaternion rotation, double width, ColorF color) {
			if (measuring) {
				currentMeasuredMin = startPoint;
				currentMeasuredMax = startPoint;
			}
			else {
				lastPoint = startPoint;
				lastRotation = rotation;
				lastWidth = width;
				lastColor = color;
			}
		}

		public abstract void MoveTo(Point3D endPoint, Quaternion rotation, double width, ColorF color);

		public abstract void DrawTo(Point3D endPoint, Quaternion rotation, double width, ColorF color, double quality);

		public abstract void DrawPolygon(Polygon3D polygon);

		public abstract void DrawSphere(double radius, ColorF color, double quality);

		#endregion IRenderer3D Members


		protected void saveLastState(Point3D point, Quaternion rotation, double width, ColorF color) {
			lastPoint = point;
			lastRotation = rotation;
			lastWidth = width;
			lastColor = color;
		}

		/// <summary>
		/// Ensures that sphere (point + radius) with given center and radius will be in measured "cuboid".
		/// </summary>
		protected void measure(Point3D pt, double radius = 0) {

			if (pt.X - radius < currentMeasuredMin.X) {
				currentMeasuredMin.X = pt.X - radius;
			}
			else if (pt.X + radius > currentMeasuredMax.X) {
				currentMeasuredMax.X = pt.X + radius;
			}

			if (pt.Y - radius < currentMeasuredMin.Y) {
				currentMeasuredMin.Y = pt.Y - radius;
			}
			else if (pt.Y + radius > currentMeasuredMax.Y) {
				currentMeasuredMax.Y = pt.Y + radius;
			}

			if (pt.Z - radius < currentMeasuredMin.Z) {
				currentMeasuredMin.Z = pt.Z - radius;
			}
			else if (pt.Z + radius > currentMeasuredMax.Z) {
				currentMeasuredMax.Z = pt.Z + radius;
			}
		}

	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using Malsys.Media;
using Microsoft.FSharp.Collections;
using System.IO;

namespace Malsys.Processing.Components.Renderers {
	public abstract class BaseRenderer3D : IRenderer3D {

		protected ProcessContext context;
		protected Stream outputStream;

		protected FSharpMap<string, object> globalAdditionalData = MapModule.Empty<string, object>();

		protected bool measuring;

		protected Point3D lastPoint;
		protected Quaternion lastRotation;
		protected double lastWidth;
		protected ColorF lastColor;

		protected Point3D measuredMin, currentMeasuredMin;
		protected Point3D measuredMax, currentMeasuredMax;



		protected virtual void measure(Point3D pt) {

			if (pt.X < currentMeasuredMin.X) {
				currentMeasuredMin.X = pt.X;
			}
			else if (pt.X > currentMeasuredMax.X) {
				currentMeasuredMax.X = pt.X;
			}

			if (pt.Y < currentMeasuredMin.Y) {
				currentMeasuredMin.Y = pt.Y;
			}
			else if (pt.Y > currentMeasuredMax.Y) {
				currentMeasuredMax.Y = pt.Y;
			}

			if (pt.Z < currentMeasuredMin.Z) {
				currentMeasuredMin.Z = pt.Z;
			}
			else if (pt.Z > currentMeasuredMax.Z) {
				currentMeasuredMax.Z = pt.Z;
			}
		}


		#region IComponent Members

		public abstract bool RequiresMeasure { get; }

		/// <summary>
		/// Saves the context to variable.
		/// </summary>
		public virtual void Initialize(ProcessContext ctxt) {
			context = ctxt;
		}

		public virtual void Cleanup() {
			context = null;
		}

		public virtual void BeginProcessing(bool measuring) {

			this.measuring = measuring;

		}

		public virtual void EndProcessing() {

			if (measuring) {
				measuredMin = currentMeasuredMin;
				measuredMax = currentMeasuredMax;
			}

		}

		#endregion


		#region IRenderer3D Members

		public void AddGlobalOutputData(string key, object value) {
			globalAdditionalData = globalAdditionalData.Add(key, value);
		}

		public void AddCurrentOutputData(string key, object value) {
			if (outputStream != null) {
				context.OutputProvider.AddAdditionalData(outputStream, key, value);
			}
		}


		/// <summary>
		/// Initializes last point, width and color.
		/// </summary>
		public virtual void InitializeState(Point3D startPoint, Quaternion rotation, double width, ColorF color) {

			lastPoint = startPoint;
			lastRotation = rotation;
			lastWidth = width;
			lastColor = color;

			measuredMin = startPoint;
			measuredMax = startPoint;
		}

		/// <summary>
		/// Saves last point, width and color.
		/// </summary>
		public virtual void MoveTo(Point3D endPoint, Quaternion rotation, double width, ColorF color) {
			lastPoint = endPoint;
			lastRotation = rotation;
			lastWidth = width;
			lastColor = color;
		}

		/// <summary>
		/// Saves last point, width and color. Should be called at the end of derived class implementation.
		/// </summary>
		public virtual void DrawTo(Point3D endPoint, Quaternion rotation, double width, ColorF color) {
			lastPoint = endPoint;
			lastRotation = rotation;
			lastWidth = width;
			lastColor = color;
		}

		public abstract void DrawPolygon(Polygon3D polygon);

		#endregion

	}
}

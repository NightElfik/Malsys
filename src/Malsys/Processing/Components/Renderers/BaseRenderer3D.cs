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


		protected Point3D lastPoint;
		protected double lastWidth;
		protected ColorF lastColor;


		#region IComponent Members

		public abstract bool RequiresMeasure { get; }

		public virtual void Initialize(ProcessContext ctxt) {
			context = ctxt;
		}

		public virtual void Cleanup() {
			context = null;
		}

		public abstract void BeginProcessing(bool measuring);

		public abstract void EndProcessing();

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
		public virtual void InitializeState(Point3D startPoint, double width, ColorF color) {
			lastPoint = startPoint;
			lastWidth = width;
			lastColor = color;
		}

		/// <summary>
		/// Saves last point, width and color.
		/// </summary>
		public virtual void MoveTo(Point3D endPoint, double width, ColorF color) {
			lastPoint = endPoint;
			lastWidth = width;
			lastColor = color;
		}

		/// <summary>
		/// Saves last point, width and color. Should be called at the end of derived class implementation.
		/// </summary>
		public virtual void LineTo(Point3D endPoint, double forwardAxisRotation, double width, ColorF color) {
			lastPoint = endPoint;
			lastWidth = width;
			lastColor = color;
		}

		public abstract void DrawPolygon(Polygon3D polygon);

		#endregion

	}
}

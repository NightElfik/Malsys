using System;
using System.Collections.Generic;
using System.IO;
using Malsys.Media;

namespace Malsys.Processing.Components.Renderers.TwoD {
	public class SvgRenderer2D : IRenderer2D {

		public string FileHeader { get; set; }

		public string SvgHeader { get; set; }

		public string SvgFooter { get; set; }

		private ProcessContext context;

		private bool measuring;

		private TextWriter writer;
		private PointF lastPoint;
		private ColorF lastColor;
		private float lastWidth;

		private float measuredMinX, measuredMinY, measuredMaxX, measuredMaxY;
		private float minX, minY, maxX, maxY;


		public SvgRenderer2D() {
			FileHeader = "<?xml version=\"1.0\" standalone=\"no\"?>\n" +
				"<!DOCTYPE svg PUBLIC \"-//W3C//DTD SVG 1.1//EN\" \"http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd\">";
			SvgHeader = "<svg xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\">";
			SvgFooter = "</svg>";
		}


		#region IComponent Members

		public bool RequiresMeasure { get { return false; } }

		public void Initialize(ProcessContext context) {
			this.context = context;
		}

		public void Cleanup() { }

		public void BeginProcessing(bool measuring) {

			this.measuring = measuring;

			if (measuring) {
				minX = 0;
				maxX = 0;
				minY = 0;
				maxY = 0;
			}
			else {
				string filePath = context.FilesManager.GetNewOutputFilePath(".svg");
				writer = new StreamWriter(filePath);
			}
		}

		public void EndProcessing() {

			if (measuring) {
				measuredMinX = minX;
				measuredMaxX = maxX;
				measuredMinY = minY;
				measuredMaxY = maxY;
			}
			else {
				writer.Close();
			}
		}

		#endregion

		#region IBasic2DRenderer Members

		public void MoveTo(PointF point, ColorF color, float width) {

			if (measuring) {
				measure(point);
			}
			else {
				lastPoint = point;
				lastColor = color;
				lastWidth = width;
			}
		}

		public void LineTo(PointF point, ColorF color, float width) {

			if (measuring) {
				measure(point);
			}
			else {
				writer.WriteLine("<line x1=\"{0}\" y1=\"{1}\" x2=\"{2}\" y2=\"{3}\" stroke=\"#{4}\" stroke-width=\"{5}\" />".FmtInvariant(
					lastPoint.X, lastPoint.Y, point.X, point.Y, color.ToArgbHexString(), width));

				lastPoint = point;
				lastColor = color;
				lastWidth = width;
			}
		}

		public void DrawPolygon(IEnumerable<PointF> points, ColorF color) {
			throw new NotImplementedException();
		}

		#endregion


		private void measure(PointF pt) {
			if (pt.X < minX) {
				minX = pt.X;
			}
			else if (pt.X > maxX) {
				maxX = pt.X;
			}

			if (pt.Y < minY) {
				minY = pt.Y;
			}
			else if (pt.Y > maxY) {
				maxY = pt.Y;
			}

		}
	}
}

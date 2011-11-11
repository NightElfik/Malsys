using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Malsys.Media;
using System.IO;

namespace Malsys.Processing.Components.Renderers.TwoD {
	public class SvgRenderer2D : IRenderer2D {

		public string FileHeader { get; set; }

		public string SvgHeader { get; set; }

		public string SvgFooter { get; set; }


		private TextWriter writer;
		private PointF lastPoint;
		private ColorF lastColor;
		private float lastWidth;


		public SvgRenderer2D() {
			FileHeader = "<?xml version=\"1.0\" standalone=\"no\"?>\n" +
				"<!DOCTYPE svg PUBLIC \"-//W3C//DTD SVG 1.1//EN\" \"http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd\">";
			SvgHeader = "<svg xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\">";
			SvgFooter = "</svg>";
		}


		#region IBasic2DRenderer Members

		public void MoveTo(PointF point, ColorF color, float width) {
			lastPoint = point;
			lastColor = color;
			lastWidth = width;
		}

		public void LineTo(PointF point, ColorF color, float width) {

			writer.WriteLine("<line x1=\"{0}\" y1=\"{1}\" x2=\"{2}\" y2=\"{3}\" stroke=\"#{4}\" stroke-width=\"{5}\" />".FmtInvariant(
				lastPoint.X, lastPoint.Y, point.X, point.Y, color.ToArgbHexString(), width));

			lastPoint = point;
			lastColor = color;
			lastWidth = width;
		}

		public void DrawPolygon(IEnumerable<PointF> points, ColorF color) {
			throw new NotImplementedException();
		}

		#endregion

		#region IRenderer Members

		public ProcessContext Context { get; set; }

		public void BeginRendering() {

			string filePath = Context.FilesManager.GetNewOutputFilePath(".svg");
			writer = new StreamWriter(filePath);

		}

		public void EndRendering() {
			writer.Close();
		}

		#endregion
	}
}

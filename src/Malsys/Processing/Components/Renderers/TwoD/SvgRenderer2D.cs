using System;
using System.Collections.Generic;
using System.IO;
using Malsys.Media;
using System.Diagnostics;
using Malsys.SemanticModel.Evaluated;
using Malsys.SemanticModel;

namespace Malsys.Processing.Components.Renderers.TwoD {
	public class SvgRenderer2D : IRenderer2D {

		private ProcessContext context;

		private bool notMeasuring;

		private TextWriter writer;
		private PointF lastPoint;
		private ColorF lastColor;
		private float lastWidth;

		private float marginT, marginR, marginB, marginL;
		private float measuredMinX, measuredMinY, measuredMaxX, measuredMaxY;
		private float minX, minY, maxX, maxY;

		string tmpFilePath;


		public SvgRenderer2D() {
			FileHeader = "<?xml version=\"1.0\" standalone=\"no\"?>\n" +
				"<!DOCTYPE svg PUBLIC \"-//W3C//DTD SVG 1.1//EN\" \"http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd\">";
			SvgHeader = "<svg xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\" viewBox=\"{0:0.###} {1:0.###} {2:0.###} {3:0.###}\" width=\"{2:0.###}px\" height=\"{3:0.###}px\">";
			SvgFooter = "</svg>";
		}


		public string FileHeader { get; set; }

		public string SvgHeader { get; set; }

		public string SvgFooter { get; set; }

		[UserSettable]
		public IValue Margin {
			set {
				if (value.IsConstant) {
					marginT = marginR = marginB = marginL = (float)((Constant)value).Value;
				}
				else if (value.IsArray) {
					var arr = (ValuesArray)value;
					if (arr.IsConstArrayOfLength(1)) {
						marginT = marginR = marginB = marginL = (float)arr[0].ConstOrDefault();
					}
					else if (arr.IsConstArrayOfLength(2)) {
						marginT = marginB = (float)arr[0].ConstOrDefault();
						marginR = marginL = (float)arr[1].ConstOrDefault();
					}
					else if (arr.IsConstArrayOfLength(4)) {
						marginT = (float)arr[0].ConstOrDefault();
						marginR = (float)arr[1].ConstOrDefault();
						marginB = (float)arr[2].ConstOrDefault();
						marginL = (float)arr[3].ConstOrDefault();
					}
				}
			}
		}


		private void writeSvgFile() {

			using (StreamWriter writer = new StreamWriter(context.OutputProvider.GetOutputStream<SvgRenderer2D>(".svg"))) {

				writer.WriteLine(FileHeader);
				writer.WriteLine(SvgHeader.FmtInvariant(
					minX - marginL,
					minY - marginT,
					maxX - minX + marginL + marginR,
					maxY - minY + marginT + marginB));
				writer.Flush();

				using (Stream reader = new FileStream(tmpFilePath, FileMode.Open, FileAccess.Read)) {
					reader.CopyTo(writer.BaseStream);
				}

				writer.WriteLine(SvgFooter);
			}
		}

		#region IComponent Members

		public bool RequiresMeasure { get { return false; } }

		public void Initialize(ProcessContext ctxt) {
			context = ctxt;
		}

		public void Cleanup() { }

		public void BeginProcessing(bool measuring) {

			notMeasuring = !measuring;

			minX = 0;
			maxX = 0;
			minY = 0;
			maxY = 0;

			if (notMeasuring) {
				//tmpFilePath = context.FilesManager.GetNewTempFilePath();
				//writer = new StreamWriter(tmpFilePath);
			}
		}

		public void EndProcessing() {

			measuredMinX = minX;
			measuredMaxX = maxX;
			measuredMinY = minY;
			measuredMaxY = maxY;

			if (notMeasuring) {
				writer.Close();
				writer = null;
				writeSvgFile();
			}
		}

		#endregion

		#region IBasic2DRenderer Members

		public void MoveTo(PointF point, ColorF color, float width) {

			measure(point);

			lastPoint = point;
			lastColor = color;
			lastWidth = width;
		}

		public void LineTo(PointF point, ColorF color, float width) {

			measure(point);

			if (notMeasuring) {
				writer.WriteLine("<line x1=\"{0:0.###}\" y1=\"{1:0.###}\" x2=\"{2:0.###}\" y2=\"{3:0.###}\" stroke=\"#{4}\" stroke-width=\"{5:0.###}\" />".FmtInvariant(
					lastPoint.X, lastPoint.Y, point.X, point.Y, color.ToRgbHexString(), width));
			}

			lastPoint = point;
			lastColor = color;
			lastWidth = width;
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

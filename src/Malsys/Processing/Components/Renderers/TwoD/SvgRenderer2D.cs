using System;
using System.Collections.Generic;
using System.IO;
using Malsys.Media;
using System.Diagnostics;
using Malsys.SemanticModel.Evaluated;
using Malsys.SemanticModel;
using System.IO.Compression;

namespace Malsys.Processing.Components.Renderers.TwoD {
	public class SvgRenderer2D : IRenderer2D {

		public const string SvgFileExtension = ".svg";
		public const string SvgzFileExtension = ".svgz";

		private ProcessContext context;

		private bool measuring;

		private TextWriter writer;
		private PointF lastPoint;
		private ColorF lastColor;
		private float lastWidth;
		private float originX, originY;
		private bool compress;

		private float marginT, marginR, marginB, marginL;
		private float measuredMinX, measuredMinY, measuredMaxX, measuredMaxY;
		private float minX, minY, maxX, maxY;



		private List<PointF> points;
		private List<ColorF> colors;
		private List<float> widths;


		public SvgRenderer2D() {

			FileHeader = "<?xml version=\"1.0\" standalone=\"no\"?>\n" +
				"<!DOCTYPE svg PUBLIC \"-//W3C//DTD SVG 1.1//EN\" \"http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd\">";
			SvgHeader = "<svg xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\" viewBox=\"{0:0.###} {1:0.###} {2:0.###} {3:0.###}\" width=\"{2:0.###}px\" height=\"{3:0.###}px\">";
			SvgFooter = "</svg>";

			compress = true;  // compress by default
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
					else {
						throw new InvalidUserValueException("Margin can be specified as array only with 1, 2 or 4 constants.");
					}
				}
			}
		}

		[UserSettable]
		public ValuesArray Origin {
			set {
				if (!value.IsConstArrayOfLength(2)) {
					throw new InvalidUserValueException("Origin have to be array of 2 constants representing x and y coordination, like `{-1, 3}`.");
				}

				originX = (float)((Constant)value[0]).Value;
				originY = (float)((Constant)value[1]).Value;
			}
		}

		[UserSettable]
		public Constant CompressSvg {
			set {
				compress = !value.IsZero;
			}
		}


		#region IComponent Members

		public bool RequiresMeasure { get { return true; } }

		public void Initialize(ProcessContext ctxt) {

			context = ctxt;

			points = new List<PointF>();
			colors = new List<ColorF>();
			widths = new List<float>();

		}

		public void Cleanup() { }

		public void BeginProcessing(bool measuring) {

			this.measuring = measuring;

			if (measuring) {
				writer = null;
				minX = originX;
				maxX = originX;
				minY = originY;
				maxY = originY;
			}
			else {
				if (compress) {
					var stream = context.OutputProvider.GetOutputStream<SvgRenderer2D>(SvgzFileExtension);
					var gzipStream = new GZipStream(stream, CompressionMode.Compress);
					writer = new StreamWriter(gzipStream);
				}
				else {
					var stream = context.OutputProvider.GetOutputStream<SvgRenderer2D>(SvgFileExtension);
					writer = new StreamWriter(stream);
				}

				writer.WriteLine(FileHeader);
				writer.WriteLine(SvgHeader.FmtInvariant(
					 minX - marginL,
					 minY - marginT,
					 maxX - minX + marginL + marginR,
					 maxY - minY + marginT + marginB));
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
				writer.WriteLine(SvgFooter);
				writer.Close();
				writer = null;
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
				writer.WriteLine("<line x1=\"{0:0.###}\" y1=\"{1:0.###}\" x2=\"{2:0.###}\" y2=\"{3:0.###}\" stroke=\"#{4}\" stroke-width=\"{5:0.###}\" />"
					.FmtInvariant(lastPoint.X, lastPoint.Y, point.X, point.Y, color.ToRgbHexString(), width));

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

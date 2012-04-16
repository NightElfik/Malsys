using System;
using System.IO;
using System.IO.Compression;
using System.Windows;
using Malsys.Media;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;
using Microsoft.FSharp.Collections;

namespace Malsys.Processing.Components.Renderers {
	/// <summary>
	/// Provides commands for rendering 2D image.
	/// Result is vector image in SVG (Scalable Vector Graphics, plain text XML).
	/// Result is by default compressed by GZip (svgz).
	/// </summary>
	/// <name>2D SVG renderer</name>
	/// <group>Renderers</group>
	public class SvgRenderer2D : IRenderer2D {

		public const string FileHeader = "<?xml version=\"1.0\" standalone=\"no\"?>\n"
			+ "<!DOCTYPE svg PUBLIC \"-//W3C//DTD SVG 1.1//EN\" \"http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd\">";
		public const string SvgHeader = "<svg xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\""
			+ " viewBox=\"{0:0.###} {1:0.###} {2:0.###} {3:0.###}\" width=\"{4:0.###}px\" height=\"{5:0.###}px\" style=\"stroke-linecap: {6}\">";
		public const string SvgFooter = "</svg>";


		private ProcessContext context;
		private FSharpMap<string, object> globalAdditionalData = MapModule.Empty<string, object>();

		private bool measuring;

		private Stream outputStream;
		private TextWriter writer;

		private double invertY = -1;

		private double lastX, lastY;
		private double lastWidth;
		private ColorF lastColor;

		private double marginT = 2, marginR = 2, marginB = 2, marginL = 2;
		private double measuredMinX, measuredMinY, measuredMaxX, measuredMaxY;
		private double minX, minY, maxX, maxY;


		public IMessageLogger Logger { get; set; }



		/// <summary>
		/// Margin of result image.
		/// </summary>
		/// <expected>One number (ao array with one number) for all margins, array of two numbers for vertical and horizontal margin
		/// or array of four numbers as top, right, bottom and left margin respectively.</expected>
		/// <default>2</default>
		[AccessName("margin")]
		[UserSettable]
		public IValue Margin {
			set {
				if (value.IsConstant) {
					marginT = marginR = marginB = marginL = ((Constant)value).Value;
				}
				else if (value.IsArray) {
					var arr = (ValuesArray)value;
					if (arr.IsConstArrayOfLength(1)) {
						marginT = marginR = marginB = marginL = arr[0].ConstOrDefault();
					}
					else if (arr.IsConstArrayOfLength(2)) {
						marginT = marginB = arr[0].ConstOrDefault();
						marginR = marginL = arr[1].ConstOrDefault();
					}
					else if (arr.IsConstArrayOfLength(4)) {
						marginT = arr[0].ConstOrDefault();
						marginR = arr[1].ConstOrDefault();
						marginB = arr[2].ConstOrDefault();
						marginL = arr[3].ConstOrDefault();
					}
					else {
						throw new InvalidUserValueException("Margin can be specified as array only with 1, 2 or 4 constants.");
					}
				}
			}
		}

		/// <summary>
		/// If set to true result SBG image is compressed by GZip.
		/// GZipped SVG images are standard and all programs supporting SVG should be able to open it.
		/// GZipping SVG significantly reduces its size.
		/// </summary>
		/// <expected>true or false</expected>
		/// <default>true</default>
		[AccessName("compressSvg")]
		[UserSettable]
		public Constant CompressSvg { get; set; }

		/// <summary>
		/// Scale of result image.
		/// </summary>
		/// <expected>Positive number.</expected>
		/// <default>1</default>
		[AccessName("scale")]
		[UserSettable]
		public Constant Scale { get; set; }

		/// <summary>
		/// Cap of each rendered line.
		/// </summary>
		/// <expected>0 for no caps, 1 for square caps, 2 for round caps</expected>
		/// <default>2 (round caps)</default>
		[AccessName("lineCap")]
		[UserSettable]
		public Constant LineCap { get; set; }


		#region IComponent Members

		public bool RequiresMeasure { get { return true; } }

		public void Initialize(ProcessContext ctxt) {
			context = ctxt;
		}

		public void Cleanup() {
			context = null;
			Margin = new Constant(2d);
			CompressSvg = Constant.True;
			Scale = Constant.One;
			LineCap = new Constant(2d);
		}

		public void BeginProcessing(bool measuring) {

			this.measuring = measuring;
			var localAdditionalData = globalAdditionalData;
			if (CompressSvg.IsTrue) {
				localAdditionalData = localAdditionalData.Add(OutputMetadataKeyHelper.OutputIsGZipped, true);
			}

			if (measuring) {
				writer = null;
			}
			else {
				double svgWidth = measuredMaxX - measuredMinX + marginL + marginR;
				double svgHeight = measuredMaxY - measuredMinY + marginT + marginB;
				double svgWidthScaled = svgWidth * Scale.Value;
				double svgHeighScaled = svgHeight * Scale.Value;

				localAdditionalData = localAdditionalData.Add(OutputMetadataKeyHelper.OutputWidth, (int)Math.Round(svgWidthScaled));
				localAdditionalData = localAdditionalData.Add(OutputMetadataKeyHelper.OutputHeight, (int)Math.Round(svgHeighScaled));

				outputStream = context.OutputProvider.GetOutputStream<SvgRenderer2D>(
					"SVG result from `{0}`".Fmt(context.Lsystem.Name),
					MimeType.Image.SvgXml, false, localAdditionalData);

				if (CompressSvg.IsTrue) {
					var gzipStream = new GZipStream(outputStream, CompressionMode.Compress);
					writer = new StreamWriter(gzipStream);
				}
				else {
					writer = new StreamWriter(outputStream);
				}

				writer.WriteLine(FileHeader);
				writer.WriteLine(SvgHeader.FmtInvariant(
					measuredMinX - marginL,
					measuredMinY - marginT,
					svgWidth,
					svgHeight,
					svgWidthScaled,
					svgHeighScaled,
					getLineCapString(LineCap)));
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


		#region IRenderer2D Members

		public void AddGlobalOutputData(string key, object value) {
			globalAdditionalData = globalAdditionalData.Add(key, value);
		}

		public void AddCurrentOutputData(string key, object value) {
			if (outputStream != null) {
				context.OutputProvider.AddMetadata(outputStream, key, value);
			}
		}

		public void InitializeState(Point point, double width, ColorF color) {

			point.Y *= invertY;

			if (measuring) {
				minX = maxX = point.X;
				maxY = minY = point.Y;
			}
			else {
				lastX = point.X;
				lastY = point.Y;
				lastWidth = width;
				lastColor = color;
			}

		}

		public void MoveTo(Point point, double width, ColorF color) {

			point.Y *= invertY;

			if (measuring) {
				measure(point.X, point.Y);
			}
			else {
				lastX = point.X;
				lastY = point.Y;
				lastWidth = width;
				lastColor = color;
			}
		}

		public void DrawTo(Point point, double width, ColorF color) {

			point.Y *= invertY;

			if (measuring) {
				measure(point.X, point.Y);
			}
			else {
				writer.WriteLine("<line x1=\"{0:0.###}\" y1=\"{1:0.###}\" x2=\"{2:0.###}\" y2=\"{3:0.###}\" stroke=\"#{4}\" stroke-width=\"{5:0.###}\" />"
					.FmtInvariant(lastX, lastY, point.X, point.Y, color.ToRgbHexString(), width));

				lastX = point.X;
				lastY = point.Y;
				lastWidth = width;
				lastColor = color;
			}
		}

		public void DrawPolygon(Polygon2D polygon) {

			if (measuring) {
				foreach (var pt in polygon.Ponits) {
					measure(pt.X, pt.Y * invertY);
				}
			}
			else {
				writer.Write("<polygon fill=\"#{0}\" stroke-width=\"{1:0.###}\" stroke=\"#{2}\" points=\""
					.FmtInvariant(polygon.Color.ToRgbHexString(), polygon.StrokeWidth, polygon.StrokeColor.ToRgbHexString()));

				foreach (var pt in polygon.Ponits) {
					writer.Write("{0:0.###},{1:0.###} ".FmtInvariant(pt.X, pt.Y * invertY));
				}

				writer.WriteLine("\" />");
			}
		}

		public void DrawCircle(Point center, double radius, ColorF color) {

			center.Y *= -1;

			if (measuring) {
				measure(center.X + radius, center.Y + radius);
				measure(center.X - radius, center.Y - radius);
			}
			else {
				writer.Write("<circle cx=\"{0:0.###}\" cy=\"{1:0.###}\" r=\"{2:0.###}\" fill=\"#{3}\" stroke-width=\"0px\" />"
					.FmtInvariant(center.X, center.Y, radius, color.ToRgbHexString()));
			}
		}

		#endregion


		private string getLineCapString(Constant c) {
			switch (c.RoundedIntValue) {
				case 1: return "square";
				case 2: return "round";
				default: return "butt";
			}
		}

		private void measure(double x, double y) {
			if (x < minX) {
				minX = x;
			}
			else if (x > maxX) {
				maxX = x;
			}

			if (y < minY) {
				minY = y;
			}
			else if (y > maxY) {
				maxY = y;
			}
		}
	}
}

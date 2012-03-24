using System.IO;
using System.IO.Compression;
using Malsys.Media;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;
using Microsoft.FSharp.Collections;

namespace Malsys.Processing.Components.Renderers {
	[Component("2D SVG renderer", ComponentGroupNames.Renderers)]
	public class SvgRenderer2D : IRenderer2D {

		private const float invertY = -1;
		public const string SvgFileExtension = ".svg";
		public const string SvgzFileExtension = ".svgz";

		private ProcessContext context;
		private FSharpMap<string, object> globalAdditionalData = MapModule.Empty<string, object>();

		private bool measuring;

		private Stream outputStream;
		private TextWriter writer;

		private float lastX, lastY;
		private float lastWidth;
		private ColorF lastColor;

		private bool compress = true;  // compress by default

		private float marginT = 2, marginR = 2, marginB = 2, marginL = 2;
		private float measuredMinX, measuredMinY, measuredMaxX, measuredMaxY;
		private float minX, minY, maxX, maxY;


		public SvgRenderer2D() {

			FileHeader = "<?xml version=\"1.0\" standalone=\"no\"?>\n" +
				"<!DOCTYPE svg PUBLIC \"-//W3C//DTD SVG 1.1//EN\" \"http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd\">";
			SvgHeader = "<svg xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\" viewBox=\"{0:0.###} {1:0.###} {2:0.###} {3:0.###}\" width=\"{2:0.###}px\" height=\"{3:0.###}px\">";
			SvgFooter = "</svg>";

		}


		public string FileHeader { get; set; }

		public string SvgHeader { get; set; }

		public string SvgFooter { get; set; }


		[Alias("margin")]
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

		[Alias("compressSvg")]
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
		}

		public void Cleanup() { }

		public void BeginProcessing(bool measuring) {

			this.measuring = measuring;
			var localAdditionalData = globalAdditionalData;
			if (compress) {
				localAdditionalData = localAdditionalData.Add(OutputMetadataKeyHelper.OutputIsGZipped, true);
			}

			if (measuring) {
				writer = null;
			}
			else {

				outputStream = context.OutputProvider.GetOutputStream<SvgRenderer2D>(
					"SVG result from `{0}`".Fmt(context.Lsystem.Name),
					MimeType.Image.SvgXml, false, localAdditionalData);

				if (compress) {
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
					 measuredMaxX - measuredMinX + marginL + marginR,
					 measuredMaxY - measuredMinY + marginT + marginB));

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

		public void InitializeState(PointF point, float width, ColorF color) {

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

		public void MoveTo(PointF point, float width, ColorF color) {

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

		public void DrawTo(PointF point, float width, ColorF color) {

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
				writer.Write("<polygon fill=\"#{0}\" stroke-width=\"{1:0.###}px\" stroke=\"#{2}\" points=\""
					.FmtInvariant(polygon.Color.ToRgbHexString(), polygon.StrokeWidth, polygon.StrokeColor.ToRgbHexString()));

				foreach (var pt in polygon.Ponits) {
					writer.Write("{0:0.###},{1:0.###} ".FmtInvariant(pt.X, pt.Y * invertY));
				}

				writer.WriteLine("\" />");
			}
		}

		#endregion


		private void measure(float x, float y) {
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

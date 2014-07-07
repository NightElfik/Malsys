// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System;
using System.IO;
using System.IO.Compression;
using System.Windows;
using Malsys.Media;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Processing.Components.Renderers {
	/// <summary>
	/// Provides commands for rendering 2D image.
	/// Result is vector image in SVG (Scalable Vector Graphics, plain text XML).
	/// Result is by default compressed by GZip (svgz).
	/// </summary>
	/// <name>2D SVG renderer</name>
	/// <group>Renderers</group>
	public class SvgRenderer2D : BaseRenderer2D {

		private const double invertY = -1;

		public const string FileHeader = "<?xml version=\"1.0\" standalone=\"no\"?>\n"
			+ "<!DOCTYPE svg PUBLIC \"-//W3C//DTD SVG 1.1//EN\" \"http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd\">";
		public const string SvgHeader = "<svg xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\""
			+ " viewBox=\"{0} {1} {2} {3}\" width=\"{4}px\" height=\"{5}px\" style=\"fill:none;stroke-linecap:{6}\">";
		public const string SvgBackground = "<rect x=\"0\" y=\"0\" width=\"100%\" height=\"100%\" style=\"fill:#{0}\" />";
		public const string SvgFooter = "</svg>";

		private TextWriter writer;

		private double marginT, marginR, marginB, marginL;

		private Point lastPoint;

		private int roundingSigDigits;

		SvgPolylinesDrawer linesDrawer = new SvgPolylinesDrawer();

		#region User gettable and settable properties

		/// <summary>
		/// Margin of result image.
		/// Margin is not applied if canvas size is specified.
		/// </summary>
		/// <expected>
		/// One number (or array with one number) for size of all (top, right, bottom and left) margins,
		/// array of two numbers for vertical and horizontal margins,
		/// or array of four numbers as top, right, bottom and left margin respectively.
		/// </expected>
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
		/// When set it overrides measured dimensions of image and uses given values.
		/// Setting this value can significantly improve performance because this component will not need measure pass before rendering.
		/// However some other component in the system may need the measure pass as well so performance may not be improved.
		/// </summary>
		/// <expected>Four numbers representing x, y, width and height of canvas.</expected>
		/// <default>none</default>
		[AccessName("canvasOriginSize")]
		[UserSettable]
		public ValuesArray CanvasOriginSize {
			set {
				if (!value.IsConstArrayOfLength(4)) {
					throw new InvalidUserValueException("CanvasOriginSize must be array of 4 constants.");
				}
				canvasOriginSize = value;
			}
		}
		private ValuesArray canvasOriginSize;

		/// <summary>
		/// When set the output is scaled to fit given rectangle (width and height).
		/// </summary>
		/// <expected>Array of two numbers representing maximum width and height of the output.</expected>
		/// <default>none</default>
		[AccessName("scaleOutputToFit")]
		[UserSettable]
		public ValuesArray ScaleOutputToFit {
			set {
				if (!value.IsConstArrayOfLength(2)) {
					throw new InvalidUserValueException("ScaleOutputToFit must be array of 2 constants.");
				}

				int wid = ((Constant)value[0]).RoundedIntValue;
				int hei = ((Constant)value[1]).RoundedIntValue;
				if (wid > 0 && hei > 0) {
					scaleOutputToFit.Width = wid;
					scaleOutputToFit.Height = hei;
				}
				else {
					throw new InvalidUserValueException("Width and height values of ScaleOutputToFit property must be positive.");
				}
			}
		}
		private Size scaleOutputToFit;

		/// <summary>
		/// If set to true result SVG image is compressed by GZip (resulting in SVGZ).
		/// GZipped SVG images are standard and all programs supporting SVG should be able to open it.
		/// GZipping SVG significantly reduces its size.
		/// </summary>
		/// <expected>true or false</expected>
		/// <default>true</default>
		[AccessName("compressSvg")]
		[UserSettable]
		public Constant CompressSvg { get; set; }

		/// <summary>
		/// If set to true, renderer will try to optimize result SVG markup to be as smallest as possible.
		/// Optimized and un-optimized SVGs might be rendered slightly differently.
		/// </summary>
		/// <expected>true or false</expected>
		/// <default>true</default>
		[AccessName("optimizeSvg")]
		[UserSettable]
		public Constant OptimizeSvg { get; set; }

		/// <summary>
		/// If set to true, renderer will try to group elements to save as much characters as possible.
		/// This will change z-order of rendered shapes and it is likely that over-optimized SVG will be rendered differently than non-optimized.
		/// </summary>
		/// <expected>true or false</expected>
		/// <default>false</default>
		[AccessName("overoptimizeSvg")]
		[UserSettable]
		public Constant OveroptimizeSvg { get; set; }

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

		/// <summary>
		/// Number of significant digits in of printed coordinates.
		/// </summary>
		/// <expected>Positive integer.</expected>
		/// <default>4</default>
		[AccessName("outputSignificantDigitsCount")]
		[UserSettable]
		public Constant OutputSignificantDigitsCount { get; set; }

		/// <summary>
		/// Background color of rendered image.
		/// </summary>
		/// <expected>Number representing ARGB color (in range from 0 to 2^32 - 1) or array of numbers (in range from 0.0 to 1.0) of length of 3 (RGB) or 4 (ARGB).</expected>
		/// <default>transparent</default>
		[AccessName("bgColor")]
		[UserSettable]
		public IValue BackgroundColor { get; set; }
		protected ColorF bgColor;

		#endregion User gettable and settable properties


		#region IComponent Members

		public override bool RequiresMeasure {
			get {
				// do not measure when canvas size is already known
				return canvasOriginSize == null;
			}
		}

		public override void Reset() {
			base.Reset();
			Margin = Constant.Two;
			CompressSvg = Constant.True;
			OptimizeSvg = Constant.True;
			OveroptimizeSvg = Constant.False;
			Scale = Constant.One;
			LineCap = Constant.Two;
			canvasOriginSize = null;
			scaleOutputToFit = Size.Empty;
			OutputSignificantDigitsCount = new Constant(4);
			BackgroundColor = ((double)0xFFFFFFFF).ToConst();
		}


		public override void BeginProcessing(bool measuring) {
			base.BeginProcessing(measuring);

			var localMetadata = globalMetadata;
			if (CompressSvg.IsTrue) {
				localMetadata = localMetadata.Add(OutputMetadataKeyHelper.OutputIsGZipped, true);
			}

			if (measuring) {
				writer = null;
				return;
			}

			var colorParser = new ColorParser(Logger);
			colorParser.TryParseColor(BackgroundColor, out bgColor, Logger);


			double minX, minY, maxX, maxY;

			if (canvasOriginSize != null) {
				minX = ((Constant)canvasOriginSize[0]).Value;
				minY = ((Constant)canvasOriginSize[1]).Value;
				maxX = minX + ((Constant)canvasOriginSize[2]).Value;
				maxY = minY + ((Constant)canvasOriginSize[3]).Value;
			}
			else {
				minX = measuredMin.X - marginL;
				minY = measuredMin.Y - marginB;
				maxX = measuredMax.X + marginR;
				maxY = measuredMax.Y + marginT;
			}

			double svgWidth = maxX - minX;
			double svgHeight = maxY - minY;

			double scale;
			if (!scaleOutputToFit.IsEmpty) {
				scale = MathHelper.GetScaleSizeToFitScale(svgWidth, svgHeight, scaleOutputToFit.Width, scaleOutputToFit.Height);
			}
			else {
				scale = Scale.Value;
			}

			int svgWidthScaled = (int)Math.Ceiling(svgWidth * scale);
			int svgHeighScaled = (int)Math.Ceiling(svgHeight * scale);

			localMetadata = localMetadata.Add(OutputMetadataKeyHelper.OutputWidth, svgWidthScaled);
			localMetadata = localMetadata.Add(OutputMetadataKeyHelper.OutputHeight, svgHeighScaled);

			outputStream = context.OutputProvider.GetOutputStream<SvgRenderer2D>(
				"SVG result from `{0}`".Fmt(context.Lsystem.Name),
				MimeType.Image.SvgXml, false, localMetadata);

			if (CompressSvg.IsTrue) {
				var gzipStream = new GZipStream(outputStream, CompressionMode.Compress);
				writer = new StreamWriter(gzipStream);
			}
			else {
				writer = new StreamWriter(outputStream);
			}

			writer.WriteLine(FileHeader);
			writer.WriteLine(SvgHeader.FmtInvariant(
				minX.RoundToShortestString(roundingSigDigits, true),
				minY.RoundToShortestString(roundingSigDigits, true),
				svgWidth.RoundToShortestString(roundingSigDigits, true),
				svgHeight.RoundToShortestString(roundingSigDigits, true),
				svgWidthScaled,
				svgHeighScaled,
				getLineCapString(LineCap)));
			if (!bgColor.IsTransparent) {
				writer.WriteLine(SvgBackground.FmtInvariant(bgColor.ToRgbHexStringOptimized()));
			}

		}

		public override void EndProcessing() {
			base.EndProcessing();

			if (measuring) {
				return;
			}

			linesDrawer.Flush();

			writer.WriteLine(SvgFooter);
			writer.Close();
			writer = null;

			// output stream flushed and closed by writer
			outputStream = null;
		}

		#endregion IComponent Members


		#region IRenderer2D Members


		public override void InitializeState(Point startPoint, double width, ColorF color) {

			bool optimizeSvg = OptimizeSvg.IsTrue;
			bool overoptimizeSvg = OveroptimizeSvg.IsTrue;
			roundingSigDigits = Math.Max(1, OutputSignificantDigitsCount.RoundedIntValue);

			startPoint.Y *= invertY;
			base.InitializeState(startPoint, width, color);

			lastPoint = startPoint;

			linesDrawer.Initialize(writer, roundingSigDigits, startPoint, width, color, optimizeSvg, overoptimizeSvg);

		}

		public override void MoveTo(Point endPoint, double width, ColorF color) {

			endPoint.Y *= invertY;

			if (measuring) {
				measure(endPoint, width / 2);
			}
			else {
				linesDrawer.MoveTo(endPoint, width, color);
			}

			lastPoint = endPoint;
		}

		public override void DrawTo(Point endPoint, double width, ColorF color) {

			endPoint.Y *= invertY;

			if (measuring) {
				measure(endPoint, width / 2);
			}
			else {
				linesDrawer.DrawTo(endPoint, width, color);
			}

			lastPoint = endPoint;
		}

		public override void DrawPolygon(Polygon2D polygon) {

			if (polygon.Ponits.Count < 3) {
				Logger.LogMessage(Message.InvalidPolygon, polygon.Ponits.Count);
				return;
			}

			if (measuring) {
				double measureRadius = polygon.StrokeWidth / 2;
				foreach (var pt in polygon.Ponits) {
					measure(pt.X, pt.Y * invertY, measureRadius);
				}
			}
			else {
				linesDrawer.CloseCurrentElement();
				writer.Write("<polygon fill=\"#{0}\" stroke-width=\"{1}\" stroke=\"#{2}\" points=\""
					.FmtInvariant(polygon.Color.ToRgbHexStringOptimized(),
						polygon.StrokeWidth.RoundToShortestString(roundingSigDigits, true),
						polygon.StrokeColor.ToRgbHexStringOptimized()));

				foreach (var pt in polygon.Ponits) {
					writer.Write(pt.X.RoundToShortestString(roundingSigDigits, true) + ","
						+ (pt.Y * invertY).RoundToShortestString(roundingSigDigits, true) + " ");
				}

				writer.WriteLine("\" />");
			}
		}

		public override void DrawCircle(double radius, ColorF color) {

			if (measuring) {
				// last point is already measured but radius may be wrong
				measure(lastPoint, radius);
			}
			else {
				linesDrawer.CloseCurrentElement();
				writer.Write("<circle cx=\"{0}\" cy=\"{1}\" r=\"{2}\" fill=\"#{3}\" stroke-width=\"0px\" />"
					.FmtInvariant(lastPoint.X.RoundToShortestString(roundingSigDigits, true),
						lastPoint.Y.RoundToShortestString(roundingSigDigits, true),
						radius.RoundToShortestString(roundingSigDigits, true),
						color.ToRgbHexStringOptimized()));
			}
		}

		#endregion IRenderer2D Members


		private string getLineCapString(Constant c) {
			switch (c.RoundedIntValue) {
				case 1: return "square";
				case 2: return "round";
				default: return "butt";
			}
		}




		public enum Message {
			[Message(MessageType.Warning, "Invalid polygon with {0} points ignored.")]
			InvalidPolygon
		}

	}
}

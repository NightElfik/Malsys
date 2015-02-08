using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using Malsys.Media;
using Malsys.Processing;
using Malsys.Processing.Components;
using Malsys.Processing.Components.Renderers;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;
using Microsoft.FSharp.Collections;
using Point2D = System.Windows.Point;  // to avoid ambiguity between System.Windows.Point and System.Drawing.Point

namespace Malsys.BitmapRenderers.Components {
	/// <summary>
	/// Provides commands for rendering 2D image.
	/// Result is bitmap image in png, jpg or gif format (user settable).
	/// </summary>
	/// <name>GDI Bitmap renderer</name>
	/// <group>Renderers</group>
	public class GdiBitmapRenderer : BaseRenderer2D {

		private double marginT, marginR, marginB, marginL;

		private Bitmap bitmap;
		private Graphics g;

		private Point2D lastPoint;
		private double lastWidth;
		private ColorF lastColor;

		private LineCap lineCap;
		private LineJoin lineJoin;

		private Pen currentPen = new Pen(Color.Black);
		private SolidBrush currentBrush = new SolidBrush(Color.Black);

		protected string outputMimeType;
		protected ImageFormat outputImageFormat;
		protected PixelFormat pixelFormat;


		#region User settable properties

		/// <summary>
		/// Margin of result image.
		/// Margin is not applied if canvas size is specified,
		/// however, it is used as margin for trimmed images (if AutoTrim is true).
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
						throw new InvalidUserValueException("Margin can be specified only as array with 1, 2 or 4 constants.");
					}
				}
			}
		}

		/// <summary>
		/// When set it overrides measured dimensions of image and uses given values.
		/// Setting this value can significantly improve performance because this component will not need measure pass before rendering.
		/// However some other component in the system may need the measure pass as well so performance may not be improved.
		/// </summary>
		/// <expected>Array of four numbers representing x, y, width and height of canvas.</expected>
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
		protected ValuesArray canvasOriginSize;

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
		protected Size scaleOutputToFit;

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
		/// Background color of rendered image.
		/// Some output formats may not support transparent backgrounds.
		/// </summary>
		/// <expected>Number representing ARGB color (in range from 0 to 2^32 - 1) or array of numbers (in range from 0.0 to 1.0) of length of 3 (RGB) or 4 (ARGB).</expected>
		/// <default>#FFFFFF (white)</default>
		[AccessName("bgColor")]
		[UserSettable]
		public IValue BackgroundColor { get; set; }
		protected ColorF bgColor;

		/// <summary>
		/// Type of output bitmap.
		/// </summary>
		/// <expected>0 for png, 1 for jpeg, 2 for gif</expected>
		/// <default>0 (png)</default>
		[AccessName("outputBitmapType")]
		[UserSettable]
		public Constant OutputBitmapType { get; set; }

		/// <summary>
		/// Trims empty areas of the image.
		/// This option works only if canvas size and origin is explicitly set (otherwise result image is "trimmed" automatically).
		/// Moreover, metadata about original position in non-trimmed image are saved to allow reconstruct original image.
		/// </summary>
		/// <expected>boolean (true of false)</expected>
		/// <default>false</default>
		[AccessName("autoTrim")]
		[UserSettable]
		public Constant AutoTrim { get; set; }

		/// <summary>
		/// If set to false rendering will not be "anti-aliased (image may seem to be less detailed).
		/// However some images are "blurry" with anti-aliased rendering and also turning off anti-aliasing may improve
		/// performance and output size may be significantly smaller.
		/// </summary>
		/// <expected>boolean (true of false)</expected>
		/// <default>true</default>
		[AccessName("antiAlias")]
		[UserSettable]
		public Constant AntiAlias { get; set; }

		#endregion User settable properties


		#region IRenderer2D Members

		public override void Reset() {
			base.Reset();
			canvasOriginSize = null;
			scaleOutputToFit = new Size(0, 0);
			Margin = Constant.Two;
			Scale = Constant.One;
			LineCap = Constant.Two;
			BackgroundColor = 0xFFFFFF.ToConst();
			OutputBitmapType = Constant.Zero;
			AutoTrim = Constant.False;
			AntiAlias = Constant.True;
		}

		public override void Initialize(ProcessContext context) {
			base.Initialize(context);

			var colorParser = new ColorParser(Logger);

			colorParser.TryParseColor(BackgroundColor, out bgColor, Logger);

			switch ((int)OutputBitmapType.Value) {
				case 1:
					outputMimeType = MimeType.Image.Jpeg;
					outputImageFormat = ImageFormat.Jpeg;
					break;
				case 2:
					outputMimeType = MimeType.Image.Gif;
					outputImageFormat = ImageFormat.Gif;
					break;
				default:
					outputMimeType = MimeType.Image.Png;
					outputImageFormat = ImageFormat.Png;
					break;
			}

			switch ((int)LineCap.Value) {
				case 0:
					lineCap = System.Drawing.Drawing2D.LineCap.Flat;
					lineJoin = LineJoin.MiterClipped;
					break;
				case 1:
					lineCap = System.Drawing.Drawing2D.LineCap.Square;
					lineJoin = LineJoin.MiterClipped;
					break;
				default:
					lineCap = System.Drawing.Drawing2D.LineCap.Round;
					lineJoin = LineJoin.Round;
					break;
			}

			currentPen.LineJoin = lineJoin;
			currentPen.StartCap = lineCap;
			currentPen.EndCap = lineCap;

		}

		public override void Cleanup() {
			base.Cleanup();
			if (g != null) {
				// dispose graphic in case of EndProcessing is not called (exception)
				g.Dispose();
				g = null;
			}
		}



		public override bool RequiresMeasure {
			get {
				// do not measure when canvas size is already known
				return canvasOriginSize == null || AutoTrim.IsTrue;
			}
		}


		public override void BeginProcessing(bool measuring) {
			base.BeginProcessing(measuring);

			if (measuring) {
				return;
			}


			var localMetadata = globalMetadata;

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

			double rawWidth = maxX - minX;
			double rawHeight = maxY - minY;
			double scale;

			if (!scaleOutputToFit.IsEmpty) {
				scale = MathHelper.GetScaleSizeToFitScale(rawWidth, rawHeight, scaleOutputToFit.Width, scaleOutputToFit.Height);
			}
			else {
				scale = Scale.Value;
			}

			// translation must be integer but in scaled coordinates
			float translateX = (float)(Math.Floor(minX * scale) / scale);
			float translateY = (float)(Math.Floor(minY * scale) / scale);
			float rawHeightRoundedInScaledCoords = (float)(Math.Floor(maxY * scale) / scale - Math.Floor(minY * scale) / scale);

			int bitmapWidth = (int)Math.Max(1, Math.Ceiling(rawWidth * scale));
			int bitmapHeight = (int)Math.Max(1, Math.Ceiling(rawHeight * scale));

			if (canvasOriginSize != null && AutoTrim.IsTrue) {
				// max offset is dimension - 1 (to not produce bitmap with one dimension 0)
				int maxHorizOffset = bitmapWidth - 1;
				int maxVertOffset = bitmapHeight - 1;
				// offset after scaling should be integer to not destroy pixel grid alignment
				int offsetL = (int)MathHelper.Clamp((Math.Floor(measuredMin.X - marginL) - Math.Ceiling(minX)) * scale, 0, maxHorizOffset);
				int offsetB = (int)MathHelper.Clamp((Math.Floor(measuredMin.Y - marginL) - Math.Ceiling(minY)) * scale, 0, maxVertOffset);
				int offsetR = (int)MathHelper.Clamp((Math.Floor(maxX) - Math.Ceiling(measuredMax.X + marginR)) * scale, 0, maxHorizOffset);
				int offsetT = (int)MathHelper.Clamp((Math.Floor(maxY) - Math.Ceiling(measuredMax.Y + marginT)) * scale, 0, maxVertOffset);

				// shift non-scaled coordinates (after scaling the offset will be integer)
				minX += offsetL / scale;
				minY += offsetB / scale;
				maxX -= offsetR / scale;
				maxY -= offsetT / scale;

				translateX += offsetL / (float)scale;
				translateY += offsetB / (float)scale;

				rawWidth = maxX - minX;
				rawHeight = maxY - minY;
				rawHeightRoundedInScaledCoords = (float)(Math.Floor(maxY * scale) / scale - Math.Floor(minY * scale) / scale);
				bitmapWidth = (int)Math.Max(1, Math.Ceiling(rawWidth * scale));
				bitmapHeight = (int)Math.Max(1, Math.Ceiling(rawHeight * scale));

				localMetadata = localMetadata.Add(OutputMetadataKeyHelper.OffsetLeft, offsetL);
				localMetadata = localMetadata.Add(OutputMetadataKeyHelper.OffsetBottom, offsetB);
				localMetadata = localMetadata.Add(OutputMetadataKeyHelper.OffsetRight, offsetR);
				localMetadata = localMetadata.Add(OutputMetadataKeyHelper.OffsetTop, offsetT);
			}


			pixelFormat = bgColor.IsTransparent ? PixelFormat.Format32bppArgb : PixelFormat.Format24bppRgb;

			try {
				bitmap = new Bitmap(bitmapWidth, bitmapHeight, pixelFormat);
			}
			catch (Exception ex) {
				throw new ComponentException("Failed to allocate bitmap {0}×{1} px.".Fmt(bitmapWidth, bitmapHeight), ex);
			}

			g = Graphics.FromImage(bitmap);
			g.SmoothingMode = AntiAlias.IsTrue ? SmoothingMode.HighQuality : SmoothingMode.None;

			// convert drawing area to "normalized" coordinates (minimum starts at 0)
			// translate by integer amount to preserve align to pixel grid of the bitmap
			g.TranslateTransform(-translateX, -translateY, MatrixOrder.Append);

			// "inverse" y coordinate -- in Malsys [0,0] is in bottom-left but in bitmap is in top-left, this will do the conversion
			g.TranslateTransform(0f, -rawHeightRoundedInScaledCoords, MatrixOrder.Append);
			g.MultiplyTransform(new Matrix(1f, 0f, 0f, -1f, 0, 0), MatrixOrder.Append);  // reflection about vector (1, 0) -- about axis x (inverses y coordinate)

			// apply scale
			g.ScaleTransform((float)scale, (float)scale, MatrixOrder.Append);

			g.Clear(Color.FromArgb((int)bgColor.ToArgb()));

			localMetadata = localMetadata.Add(OutputMetadataKeyHelper.OutputWidth, bitmapWidth);
			localMetadata = localMetadata.Add(OutputMetadataKeyHelper.OutputHeight, bitmapHeight);

			// get output stream now to allow adding of metadata during processing (see base class)
			outputStream = createStream(localMetadata);
		}

		public override void EndProcessing() {
			base.EndProcessing();

			if (measuring) {
				return;
			}

			bitmap.Save(outputStream, outputImageFormat);

			g.Dispose();
			g = null;
			bitmap.Dispose();
			bitmap = null;

			outputStream.Flush();
			outputStream = null;
		}


		public override void InitializeState(Point2D startPoint, double width, ColorF color) {
			base.InitializeState(startPoint, width, color);

			lastPoint = startPoint;
			lastWidth = width;
			lastColor = color;
		}

		public override void MoveTo(Point2D endPoint, double width, ColorF color) {
			if (measuring) {
				measure(endPoint, width / 2);
			}

			lastPoint = endPoint;
			lastWidth = width;
			lastColor = color;
		}

		public override void DrawTo(Point2D endPoint, double width, ColorF color) {
			if (measuring) {
				measure(endPoint, width / 2);
			}
			else {
				g.DrawLine(createPen(color, width),
					(float)lastPoint.X, (float)lastPoint.Y, (float)endPoint.X, (float)endPoint.Y);
			}

			lastPoint = endPoint;
			lastWidth = width;
			lastColor = color;
		}

		public override void DrawPolygon(Polygon2D polygon) {

			if (polygon.Ponits.Count < 3) {
				Logger.LogMessage(Message.InvalidPolygon, polygon.Ponits.Count);
				return;
			}

			if (measuring) {
				double measureRadius = polygon.StrokeWidth / 2;
				foreach (var pt in polygon.Ponits) {
					measure(pt, measureRadius);
				}
			}
			else {
				PointF[] points = new PointF[polygon.Ponits.Count];
				for (int i = points.Length - 1; i >= 0; i--) {
					points[i].X = (float)polygon.Ponits[i].X;
					points[i].Y = (float)polygon.Ponits[i].Y;
				}

				g.FillPolygon(createBrush(polygon.Color), points);

				if (polygon.StrokeWidth.EpsilonCompareTo(0) == 1) {
					g.DrawPolygon(createPen(polygon.StrokeColor, polygon.StrokeWidth), points);
				}
			}
		}

		public override void DrawCircle(double radius, ColorF color) {
			if (measuring) {
				// last point is already measured but radius may be wrong
				measure(lastPoint, radius);
			}
			else {
				g.FillEllipse(createBrush(color), (float)(lastPoint.X - radius), (float)(lastPoint.Y - radius), 2f * (float)radius, 2f * (float)radius);
			}
		}


		#endregion IRenderer2D Members


		protected virtual Stream createStream(FSharpMap<string, object> metadata) {
			return context.OutputProvider.GetOutputStream<GdiBitmapRenderer>("Bitmap image", outputMimeType, false, metadata);
		}


		private Pen createPen(ColorF color, double width) {
			//return new Pen(Color.FromArgb((int)color.ToArgb()), (float)width) {
			//    LineJoin = lineJoin,
			//    StartCap = lineCap,
			//    EndCap = lineCap
			//};
			currentPen.Color = Color.FromArgb((int)color.ToArgb());
			currentPen.Width = (float)width;
			return currentPen;
		}

		private Brush createBrush(ColorF color) {
			//return new SolidBrush(Color.FromArgb((int)color.ToArgb()));
			currentBrush.Color = Color.FromArgb((int)color.ToArgb());
			return currentBrush;
		}

		public enum Message {
			[Message(MessageType.Warning, "Invalid polygon with {0} points ignored.")]
			InvalidPolygon
		}
	}
}

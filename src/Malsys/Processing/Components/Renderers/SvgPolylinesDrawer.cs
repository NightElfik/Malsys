using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Malsys.Media;

namespace Malsys.Processing.Components.Renderers {
	public class SvgPolylinesDrawer {

		private TextWriter writer;

		private int roundDigits;

		private Point lastPoint;
		private double lastWidth;
		private ColorF lastColor;


		private bool optimize;
		private bool overOptimize;
		private bool lineStarted;

		private string lastOptPtX;
		private string lastOptPtY;
		private string lastOptWidthStr;
		private string lastOptColorStr;

		private Dictionary<string, SvgPolyline> polylines;



		/// <param name="tw">Text writer for writing output SVG tags.</param>
		/// <param name="roundingSignifDigits">Number of significant digits for rounding.</param>
		/// <param name="startPoint">Initial point.</param>
		/// <param name="width">Initial width.</param>
		/// <param name="color">Initial color.</param>
		/// <param name="optimize">Do optimizations which should not affect the result.</param>
		/// <param name="overOptimize">Do optimizations which can affect the result (z-order).</param>
		public void Initialize(TextWriter tw, int roundingSignifDigits, Point startPoint, double width, ColorF color, bool optimize, bool overOptimize) {

			writer = tw;

			this.optimize = optimize;
			this.overOptimize = overOptimize;

			if (this.overOptimize) {
				polylines = new Dictionary<string, SvgPolyline>();
			}

			roundDigits = roundingSignifDigits;

			lastPoint = startPoint;
			lastWidth = width;
			lastColor = color;

			lineStarted = false;
		}

		public void MoveTo(Point endPoint, double width, ColorF color) {
			lastPoint = endPoint;
		}

		public void DrawTo(Point endPoint, double width, ColorF color) {

			string widStr = widthToString(width);
			string colorStr = colorToString(color);

			if (overOptimize) {
				appendToPolyline(lastPoint, endPoint, widStr, colorStr);
			}
			else if (optimize) {
				drawOptimizedLine(lastPoint, endPoint, widStr, colorStr);
			}
			else {
				writer.WriteLine("<line x1=\"{0}\" y1=\"{1}\" x2=\"{2}\" y2=\"{3}\" stroke=\"#{4}\" stroke-width=\"{5}\" />"
					.FmtInvariant(lastPoint.X.RoundToShortestString(roundDigits, true),
						lastPoint.Y.RoundToShortestString(roundDigits, true),
						endPoint.X.RoundToShortestString(roundDigits, true),
						endPoint.Y.RoundToShortestString(roundDigits, true),
						colorStr,
						widStr));
			}

			lastPoint = endPoint;
		}

		public void CloseCurrentElement() {

			if (optimize) {
				endOptimizedLine();
			}

		}

		public void Flush() {
			if (optimize) {
				endOptimizedLine();
			}

			if (overOptimize) {
				printLines();
			}
		}


		private void printLines() {

			var linesGrouppedByWidth = polylines.Values.Where(l => l.IsEmpty == false).GroupBy(l => l.Width);
			var linesGrouppedByColor = polylines.Values.Where(l => l.IsEmpty == false).GroupBy(l => l.Color);


			// color codes are longer so favor colors over widths slightly
			if (linesGrouppedByColor.Count() <= linesGrouppedByWidth.Count() * 1.1) {
				// main group by color
				foreach (var colorGroup in linesGrouppedByColor) {
					Debug.Assert(colorGroup.Count() > 0);

					if (colorGroup.Count() == 1) {
						SvgPolyline line = colorGroup.First();
						Debug.Assert(line.IsEmpty == false);
						writer.Write("<path stroke=\"#{0}\" stroke-width=\"{1}\" d=\"{2}\" />", line.Color, line.Width, line.GetSvgPath());
					}
					else {
						writer.Write("<g stroke=\"#{0}\">", colorGroup.Key);
						var addGrouppedByWidth = colorGroup.GroupBy(l => l.Width);

						foreach (var widthGroup in addGrouppedByWidth) {
							Debug.Assert(widthGroup.Count() > 0);

							if (widthGroup.Count() == 1) {
								SvgPolyline line = widthGroup.First();
								Debug.Assert(line.IsEmpty == false);
								writer.Write("<path stroke-width=\"{0}\" d=\"{1}\" />", widthGroup.Key, line.GetSvgPath());
							}
							else {
								writer.Write("<g stroke-width=\"{0}\">", widthGroup.Key);
								foreach (var line in widthGroup) {
									writer.Write("<path d=\"{0}\" />", line.GetSvgPath());
								}
								writer.Write("</g>");
							}
						}
						writer.Write("</g>");
					}
				}
			}
			else {
				// main group by width
				foreach (var widthGroup in linesGrouppedByWidth) {
					Debug.Assert(widthGroup.Count() > 0);

					if (widthGroup.Count() == 1) {
						SvgPolyline line = widthGroup.First();
						Debug.Assert(line.IsEmpty == false);
						writer.Write("<path stroke=\"#{0}\" stroke-width=\"{1}\" d=\"{2}\" />", line.Color, line.Width, line.GetSvgPath());
					}
					else {
						writer.Write("<g stroke-width=\"{0}\">", widthGroup.Key);
						var addGrouppedByColor = widthGroup.GroupBy(l => l.Color);

						foreach (var colorGroup in addGrouppedByColor) {
							Debug.Assert(colorGroup.Count() > 0);

							if (colorGroup.Count() == 1) {
								SvgPolyline line = colorGroup.First();
								Debug.Assert(line.IsEmpty == false);
								writer.Write("<path stroke=\"#{0}\" d=\"{1}\" />", colorGroup.Key, line.GetSvgPath());
							}
							else {
								writer.Write("<g stroke=\"#{0}\">", colorGroup.Key);
								foreach (var line in colorGroup) {
									writer.Write("<path d=\"{0}\" />", line.GetSvgPath());
								}
								writer.Write("</g>");
							}
						}
						writer.Write("</g>");
					}
				}
			}
		}

		private string widthToString(double wid) {
			return wid.RoundToShortestString(roundDigits, true);
		}

		private string colorToString(ColorF clr) {
			return clr.ToRgbHexStringOptimized();
		}


		private void drawOptimizedLine(Point startPt, Point endPt, string widthStr, string colorStr) {

			if (lastOptColorStr != colorStr || lastOptWidthStr != widthStr) {
				endOptimizedLine();
			}

			string startPtX = startPt.X.RoundToShortestString(roundDigits, true);
			string startPtY = startPt.Y.RoundToShortestString(roundDigits, true);
			string endPtX = endPt.X.RoundToShortestString(roundDigits, true);
			string endPtY = endPt.Y.RoundToShortestString(roundDigits, true);

			if (!lineStarted) {
				lineStarted = true;
				writer.Write("<path stroke=\"#{0}\" stroke-width=\"{1}\" d=\"".FmtInvariant(
					colorStr, widthStr));
				lastOptPtX = null;
				lastOptPtY = null;
				lastOptColorStr = colorStr;
				lastOptWidthStr = widthStr;
			}
			else {
				if (startPtX == endPtX && startPtY == endPtY) {
					return;
				}
			}

			if (startPtX != lastOptPtX || startPtY != lastOptPtY) {
				writer.Write("M {0} {1} ".FmtInvariant(startPtX, startPtY));
			}

			writer.Write("L {0} {1} ".FmtInvariant(endPtX, endPtY));
			lastOptPtX = endPtX;
			lastOptPtY = endPtY;
		}

		private void endOptimizedLine() {
			if (!lineStarted) {
				return;
			}

			writer.Write("\" />");
			lineStarted = false;
		}


		private void appendToPolyline(Point startPt, Point endPt, string widthStr, string colorStr) {

			string hash = widthStr + "#" + colorStr;
			SvgPolyline line;
			if (!polylines.TryGetValue(hash, out line)) {
				line = new SvgPolyline(widthStr, colorStr);
				polylines.Add(hash, line);
			}

			line.Add(startPt.X.RoundToShortestString(roundDigits, true),
				startPt.Y.RoundToShortestString(roundDigits, true),
				endPt.X.RoundToShortestString(roundDigits, true),
				endPt.Y.RoundToShortestString(roundDigits, true));
		}

		private class SvgPolyline {

			public bool IsEmpty { get { return lastX == null || lastY == null; } }

			public string Width { get; private set; }
			public string Color { get; private set; }

			private StringBuilder tagBuilder = new StringBuilder();
			private string lastX = null;
			private string lastY = null;


			public SvgPolyline(string widthStr, string colorStr) {
				Width = widthStr;
				Color = colorStr;
			}

			public void Add(string fromX, string fromY, string toX, string toY) {
				if (lastX != fromX || lastY != fromY) {
					tagBuilder.Append("M ");
					tagBuilder.Append(fromX);
					tagBuilder.Append(" ");
					tagBuilder.Append(fromY);
					tagBuilder.Append(" ");
				}

				tagBuilder.Append("L ");
				tagBuilder.Append(toX);
				tagBuilder.Append(" ");
				tagBuilder.Append(toY);
				tagBuilder.Append(" ");

				lastX = toX;
				lastY = toY;
			}

			public string GetSvgPath() {
				return tagBuilder.ToString();
			}

		}

	}
}

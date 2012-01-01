using System.IO;
using System.IO.Compression;
using Malsys.SemanticModel;
using System;

namespace Malsys.Processing.Components.Renderers.TwoD {
	public class TextRenderer : ITextRenderer {


		private ProcessContext context;

		private bool compress = false;

		private bool measuring;

		private int measuredMinX, measuredMinY, measuredMaxX, measuredMaxY,
			measuredWidth, measuredHeight;
		private int minX, minY, maxX, maxY;

		private char[][] resultBuffer;


		[UserSettable]
		public Constant Compress {
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

			if (measuring) {
				resultBuffer = null;
			}
			else {
				resultBuffer = new char[measuredHeight][];
				for (int y = 0; y < measuredHeight; y++) {
					var arr = new char[measuredWidth];
					for (int x = 0; x < measuredWidth; x++) {
						arr[x] = ' ';
					}
					resultBuffer[y] = arr;
				}
			}
		}

		public void EndProcessing() {

			if (measuring) {
				measuredMinX = minX;
				measuredMaxX = maxX;
				measuredMinY = minY;
				measuredMaxY = maxY;

				measuredWidth = maxX - minX + 1;
				measuredHeight = maxY - minY + 1;
			}
			else {

				TextWriter writer;

				if (compress) {
					var stream = context.OutputProvider.GetOutputStream<SvgRenderer2D>(".ascii.txt.zip");
					var gzipStream = new GZipStream(stream, CompressionMode.Compress);
					writer = new StreamWriter(gzipStream);
				}
				else {
					var stream = context.OutputProvider.GetOutputStream<SvgRenderer2D>(".ascii.txt");
					writer = new StreamWriter(stream);
				}

				using (writer) {
					foreach (var row in resultBuffer) {
						writer.WriteLine(new string(row));
					}
				}
			}

		}

		#endregion

		#region ITextRenderer

		public void PutCharAt(char c, int x, int y) {

			y *= -1;

			if (measuring) {
				measure(x, y);
			}
			else {
				resultBuffer[y - measuredMinY][x - measuredMinX] = c;
			}

		}

		#endregion

		private void measure(int x, int y) {
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

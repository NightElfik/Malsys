// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System.IO;
using Microsoft.FSharp.Collections;

namespace Malsys.Processing.Components.Renderers {
	/// <summary>
	/// Provides commands for rendering plain text ASCII art.
	/// </summary>
	/// <name>Text renderer</name>
	/// <group>Renderers</group>
	public class TextRenderer : ITextRenderer {

		private ProcessContext context;
		private FSharpMap<string, object> globalAdditionalData = MapModule.Empty<string, object>();
		private FSharpMap<string, object> localAdditionalData;

		//private bool compress = false;

		private bool measuring;

		private int measuredMinX, measuredMinY, measuredMaxX, measuredMaxY,
			measuredWidth, measuredHeight;
		private int minX, minY, maxX, maxY;

		private char[][] resultBuffer;

		#region IComponent Members

		public IMessageLogger Logger { get; set; }


		public void Reset() { }

		public void Initialize(ProcessContext ctxt) {
			context = ctxt;
		}


		public void Cleanup() {
			context = null;
		}

		public void Dispose() { }



		public bool RequiresMeasure { get { return true; } }


		public void BeginProcessing(bool measuring) {

			this.measuring = measuring;
			localAdditionalData = globalAdditionalData;

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
				var stream = context.OutputProvider.GetOutputStream<TextRenderer>(
					"Text result from `{0}`".Fmt(context.Lsystem.Name), MimeType.Text.Plain, false, localAdditionalData);

				using (var writer = new StreamWriter(stream)) {
					foreach (var row in resultBuffer) {
						writer.WriteLine(new string(row).TrimEnd());
					}
				}
			}

		}

		#endregion IComponent Members


		#region ITextRenderer

		public void AddGlobalOutputData(string key, object value) {
			globalAdditionalData = globalAdditionalData.Add(key, value);
		}

		public void AddCurrentOutputData(string key, object value) {
			localAdditionalData = localAdditionalData.Add(key, value);
		}


		public void PutCharAt(char c, int x, int y) {

			y *= -1;

			if (measuring) {
				measure(x, y);
			}
			else {
				resultBuffer[y - measuredMinY][x - measuredMinX] = c;
			}

		}

		#endregion ITextRenderer


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

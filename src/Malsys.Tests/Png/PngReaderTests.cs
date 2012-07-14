using System.IO;
using System.Text;
using Malsys.BitmapRenderers.Png;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Malsys.Tests.Png {
	[TestClass]
	public class PngReaderTests {

		[TestMethod]
		public void MinimalPngTest() {
			var stream = new MemoryStream(Properties.Resources.MinimalPng_png);

			string actual = readPng(stream);
			string expected = StringHelper.JoinLines("Start PNG",
				"Start chunks",
				"IHDR [13]",
				"IDAT [14]",
				"IEND [0]",
				"End PNG");

			Assert.AreEqual(expected, actual);

		}

		private string readPng(Stream stream) {

			var sb = new StringBuilder();
			byte[] buffer = new byte[1024];

			var pngReader = new PngReader(stream);
			sb.AppendLine("Start PNG");

			pngReader.ReadPngHeader();
			sb.AppendLine("Start chunks");

			PngReader.ChunkReader chunkReader;
			while ((chunkReader = pngReader.ReadChunk()) != null) {
				sb.AppendLine(chunkReader.Name + " [" + chunkReader.Length + "]");

				int readedLength, readedTotal = 0;
				while ((readedLength = chunkReader.Read(buffer, 0, buffer.Length)) > 0) {
					readedTotal += readedLength;
				}

				Assert.AreEqual(chunkReader.Length, (uint)readedTotal);
			}

			sb.Append("End PNG");

			return sb.ToString();
		}

	}
}

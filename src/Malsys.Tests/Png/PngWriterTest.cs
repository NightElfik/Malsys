using System;
using System.IO;
using Malsys.BitmapRenderers.Png;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Malsys.Tests.Png {
	[TestClass]
	public class PngWriterTest {
		[TestMethod]
		public void ReadWriteMinimalPng() {
			readWritePng(Properties.Resources.MinimalPng_png);
		}

		private void readWritePng(byte[] data) {

			var readStream = new MemoryStream(data);
			var writeStream = new MemoryStream(data.Length);

			var pngReader = new PngReader(readStream);
			var pngWriter = new PngWriter(writeStream);
			byte[] buffer = new byte[1024];

			pngReader.ReadPngHeader();
			pngWriter.WritePngHeader();

			PngReader.ChunkReader chunkReader;
			while ((chunkReader = pngReader.ReadChunk()) != null) {
				var chunkWriter = pngWriter.StartChunk(chunkReader.Name, chunkReader.Length);

				int readedLength;
				while ((readedLength = chunkReader.Read(buffer, 0, buffer.Length)) > 0) {
					chunkWriter.Write(buffer, 0, readedLength);
				}
			}

			writeStream.Close();
			var actual = writeStream.ToArray();

			if (data.Length == actual.Length) {
				Console.WriteLine("exp\texp x\tact x\tact");
				for (int i = 0; i < data.Length; i++) {
					Console.WriteLine("{0}\t{1}\t{2}\t{3}",
						(data[i] >= 32 && data[i] < 127) ? (char)data[i] : ' ',
						data[i].ToString("X"),
						actual[i].ToString("X"),
						(actual[i] >= 32 && actual[i] < 127) ? (char)actual[i] : ' ');
				}
			}

			CollectionAssert.AreEqual(data, actual);

		}
	}
}

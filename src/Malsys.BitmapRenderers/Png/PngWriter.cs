using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics.Contracts;

namespace Malsys.BitmapRenderers.Png {
	public class PngWriter {

		private Stream stream;
		private ChunkWriter currentChunkWriter = null;
		private bool lastChunkWritten = false;

		private byte[] buffer = new byte[8];


		public PngWriter(Stream s) {
			stream = s;
		}


		public void WritePngHeader() {
			buffer[0] = 137;
			buffer[1] = 80;
			buffer[2] = 78;
			buffer[3] = 71;
			buffer[4] = 13;
			buffer[5] = 10;
			buffer[6] = 26;
			buffer[7] = 10;
			stream.Write(buffer, 0, 8);
		}

		public ChunkWriter StartChunk(string name, uint length) {
			if (lastChunkWritten) {
				throw new InvalidOperationException("PNG image was already closed by IEDN chunk.");
			}

			if (currentChunkWriter != null) {
				currentChunkWriter.WriteChecksum();
				currentChunkWriter = null;
			}

			currentChunkWriter = new ChunkWriter(name, length, this);
			currentChunkWriter.WriteChunkHeader();

			if (name == PngHelper.IEND) {
				if (length != 0) {
					throw new ArgumentException("IEND chunk do not have zero (0) length.");
				}
				lastChunkWritten = true;
				currentChunkWriter.WriteChecksum();
				currentChunkWriter = null;
				return null;
			}

			return currentChunkWriter;
		}



		public class ChunkWriter {

			private static uint[] crcTable = new uint[256];

			/// <summary>
			/// Initializes CRC table.
			/// </summary>
			/// <remarks>
			/// Source: http://www.w3.org/TR/PNG/#11IDAT, Annex D.
			/// </remarks>
			static ChunkWriter() {

				for (int n = 0; n < 256; n++) {
					uint c = (uint)n;
					for (int k = 0; k < 8; k++) {
						if ((c & 1u) == 1u) {
							c = 0xEDB88320 ^ (c >> 1);
						}
						else {
							c = c >> 1;
						}
					}
					crcTable[n] = c;
				}
			}


			/// <summary>
			/// A four-byte CRC calculated on the preceding bytes in the chunk, including the chunk type field and chunk data fields,
			/// but not including the length field.
			/// </summary>
			private uint crc32 = 0xFFFFFFFF;

			private PngWriter parentWriter;
			private uint bytesRemaining;

			public readonly string Name;
			public readonly uint Length;


			public ChunkWriter(string name, uint length, PngWriter parentPngWriter) {

				Contract.Requires<ArgumentNullException>(name != null);
				Contract.Requires<ArgumentException>(name.Length == 4);
				Contract.Requires<ArgumentNullException>(parentPngWriter != null);

				Name = name;
				Length = length;

				parentWriter = parentPngWriter;
				bytesRemaining = length;
			}

			public void Write(byte[] buffer, int offset, int count) {

				Contract.Requires<ArgumentNullException>(buffer != null);
				Contract.Requires<ArgumentException>(offset >= 0 && offset < buffer.Length);
				Contract.Requires<ArgumentException>(count >= 0);
				Contract.Requires<ArgumentException>(offset + count <= buffer.Length);

				if (bytesRemaining < count) {
					throw new InvalidOperationException("Chunk too small.");
				}

				updateCrc(buffer, offset, count);
				parentWriter.stream.Write(buffer, offset, count);

				bytesRemaining -= (uint)count;
			}


			/// <remarks>
			/// Source: http://www.w3.org/TR/PNG/#11IDAT, Annex D.
			/// </remarks>
			private void updateCrc(byte[] buffer, int offset, int count) {
				for (int i = 0; i < count; i++) {
					crc32 = crcTable[(crc32 ^ (uint)buffer[offset + i]) & 0xFF] ^ (crc32 >> 8);
				}
			}

			internal void WriteChunkHeader() {
				var buffer = new byte[4];
				buffer[0] = (byte)((Length >> 24) & 0xFFu);
				buffer[1] = (byte)((Length >> 16) & 0xFFu);
				buffer[2] = (byte)((Length >> 8) & 0xFFu);
				buffer[3] = (byte)(Length & 0xFFu);
				parentWriter.stream.Write(buffer, 0, 4);

				buffer[0] = (byte)Name[0];
				buffer[1] = (byte)Name[1];
				buffer[2] = (byte)Name[2];
				buffer[3] = (byte)Name[3];

				updateCrc(buffer, 0, 4);  // CRC do not include length field
				parentWriter.stream.Write(buffer, 0, 4);
			}

			internal void WriteChecksum() {
				if (bytesRemaining > 0) {
					throw new InvalidOperationException("Chunk data not completed, {0} bytes missing.".Fmt(bytesRemaining));
				}

				uint crc = ~crc32;

				var stream = parentWriter.stream;
				stream.WriteByte((byte)((crc >> 24) & 0xFFu));
				stream.WriteByte((byte)((crc >> 16) & 0xFFu));
				stream.WriteByte((byte)((crc >> 8) & 0xFFu));
				stream.WriteByte((byte)(crc & 0xFFu));
			}





		}

	}
}

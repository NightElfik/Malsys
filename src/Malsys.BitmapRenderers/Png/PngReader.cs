using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;

namespace Malsys.BitmapRenderers.Png {
	public class PngReader {

		private Stream stream;
		private ChunkReader currentChunkReader = null;
		private bool lastChunkReaded = false;


		private byte[] buffer = new byte[8];

		public PngReader(Stream s) {
			stream = s;
		}





		public void ReadPngHeader() {
			readBuffer(8); // PNG signature
			if (buffer[0] != 137 || buffer[1] != 80 || buffer[2] != 78 || buffer[3] != 71
					|| buffer[4] != 13 || buffer[5] != 10 || buffer[6] != 26 || buffer[7] != 10) {
				throw new InvalidDataException("PNG signature is invalid.");
			}
		}

		public ChunkReader ReadChunk() {

			if (currentChunkReader != null) {
				currentChunkReader.SeekToEnd();
				stream.Read(buffer, 0, 4);  // last chunk checksum
				currentChunkReader = null;
			}

			if (lastChunkReaded) {
				return null;
			}

			readBuffer(8);
			uint chunkLength = (uint)buffer[0] << 24 | (uint)buffer[1] << 16 | (uint)buffer[2] << 8 | (uint)buffer[3];
			string chunkName = Encoding.ASCII.GetString(buffer, 4, 4);

			lastChunkReaded = chunkName == PngHelper.IEND;

			currentChunkReader = new ChunkReader(chunkName, chunkLength, this);
			return currentChunkReader;
		}


		public void Dispose() {
			stream = null;
		}

		private void readBuffer(int length) {
			Contract.Requires<ArgumentException>(length <= buffer.Length);

			if (stream.Read(buffer, 0, length) != length) {
				throw new InvalidDataException("PNG file is invalid, expected more data.");
			}
		}



		public class ChunkReader {

			private PngReader parentReader;
			private uint bytesRemaining;

			public readonly string Name;
			public readonly uint Length;

			private readonly long streamPosition;


			public ChunkReader(string name, uint length, PngReader parentPngReader) {

				Contract.Requires<ArgumentNullException>(name != null);
				Contract.Requires<ArgumentException>(name.Length == 4);
				Contract.Requires<ArgumentNullException>(parentPngReader != null);

				Name = name;
				Length = length;

				parentReader = parentPngReader;
				bytesRemaining = length;

				streamPosition = parentReader.stream.Position;
			}

			/// <summary>
			/// Returns true if this chunk is critical.
			/// Critical chunks are necessary for successful display of the contents of the datastream.
			/// </summary>
			public bool IsCritical { get { return char.IsUpper(Name[0]); } }

			public bool IsPrivate { get { return char.IsUpper(Name[1]); } }

			public bool IsSafeToCopy { get { return char.IsUpper(Name[3]); } }


			/// <summary>
			/// Number of remaining bytes of data in current chunk.
			/// </summary>
			public uint BytesRemaining { get { return bytesRemaining; } }


			/// <summary>
			/// Reads a sequence of bytes from the current chunk data and advances the position within the stream by the number of bytes read.
			/// </summary>
			/// <returns>The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes
			/// are not currently available in current chunk, or zero (0) if the end of the chunk has been reached.</returns>
			public int Read(byte[] buffer, int offset, int count) {

				Contract.Requires<ArgumentNullException>(buffer != null);
				Contract.Requires<ArgumentException>(offset >= 0 && offset < buffer.Length);
				Contract.Requires<ArgumentException>(count >= 0);
				Contract.Requires<ArgumentException>(offset + count <= buffer.Length);

				if (bytesRemaining == 0) {
					return 0;
				}

				count = (int)Math.Min((uint)count, bytesRemaining);
				int bytesReaded = parentReader.stream.Read(buffer, offset, count);

				Debug.Assert(bytesReaded <= bytesRemaining);
				bytesRemaining -= (uint)bytesReaded;

				return bytesReaded;
			}


			public void SeekToEnd() {
				parentReader.stream.Seek(bytesRemaining, SeekOrigin.Current);
			}

			public ChunkReader Next() {
				return parentReader.ReadChunk();
			}

			public void Reset() {
				bytesRemaining = Length;
				parentReader.stream.Position = streamPosition;

				parentReader.lastChunkReaded = false;
			}


		}
	}
}

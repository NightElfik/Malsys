using System;
using Malsys.BitmapRenderers.Png.Chunks;
using System.Diagnostics.Contracts;
using System.Text;

namespace Malsys.BitmapRenderers.Png {
	/// <summary>
	/// All public members of this class are thread safe.
	/// However, it uses ThreadStatic buffer which must be initialized by Initialize method.
	/// </summary>
	public static class PngHelper {

		public static readonly string IEND = "IEND";
		public static readonly string IDAT = "IDAT";

		[ThreadStatic]
		private static byte[] buffer;
		private const int bufferLength = 4096;  // 4 KB


		/// <summary>
		/// Instantiates buffer which is marked as ThreadStatic.
		/// This method should be called only once per thread.
		/// </summary>
		public static void Initialize() {
			if (buffer == null) {
				buffer = new byte[bufferLength];
			}
		}

		#region Reader

		public static byte ReadByte(this PngReader.ChunkReader reader) {
			reader.Read(buffer, 0, 1);
			return buffer[0];
		}

		public static ushort ReadUshort(this PngReader.ChunkReader reader) {
			reader.Read(buffer, 0, 2);
			return (ushort)((uint)buffer[2] << 8 | (uint)buffer[3]);
		}

		public static uint ReadUint(this PngReader.ChunkReader reader) {
			reader.Read(buffer, 0, 4);
			return (uint)buffer[0] << 24 | (uint)buffer[1] << 16 | (uint)buffer[2] << 8 | (uint)buffer[3];
		}


		public static ChunkIHDR ReadIHDR(this PngReader.ChunkReader reader) {

			Contract.Requires<ArgumentNullException>(reader != null);
			Contract.Requires<ArgumentException>(reader.Name == ChunkIHDR.Name);
			Contract.Requires<ArgumentException>(reader.Length == ChunkIHDR.Length);

			var ihdr = new ChunkIHDR();
			ihdr.Width = reader.ReadUint();
			ihdr.Height = reader.ReadUint();
			ihdr.BitDepth = reader.ReadByte();
			ihdr.ColourType = reader.ReadByte();
			ihdr.CompressionMethod = reader.ReadByte();
			ihdr.FilterMethod = reader.ReadByte();
			ihdr.InterlaceMethod = reader.ReadByte();
			return ihdr;
		}

		#endregion


		#region Writer

		public static void Write(this PngWriter.ChunkWriter writer, byte value) {
			buffer[0] = value;
			writer.Write(buffer, 0, 1);
		}

		public static void Write(this PngWriter.ChunkWriter writer, ushort value) {
			buffer[0] = (byte)((value >> 8) & 0xFFu);
			buffer[1] = (byte)(value & 0xFFu);
			writer.Write(buffer, 0, 2);
		}

		public static void Write(this PngWriter.ChunkWriter writer, uint value) {
			buffer[0] = (byte)((value >> 24) & 0xFFu);
			buffer[1] = (byte)((value >> 16) & 0xFFu);
			buffer[2] = (byte)((value >> 8) & 0xFFu);
			buffer[3] = (byte)(value & 0xFFu);
			writer.Write(buffer, 0, 4);
		}

		public static void Write(this PngWriter.ChunkWriter writer, string str) {
			var strBytes = Encoding.ASCII.GetBytes(str);
			writer.Write(strBytes, 0, strBytes.Length);
		}


		public static void WriteIEND(this PngWriter pngWriter) {
			var chunkWriter = pngWriter.StartChunk(IEND, 0);
		}

		public static void WriteIHDR(this PngWriter pngWriter, ChunkIHDR ihdr) {

			var chunkWriter = pngWriter.StartChunk(ChunkIHDR.Name, ChunkIHDR.Length);
			chunkWriter.Write(ihdr.Width);
			chunkWriter.Write(ihdr.Height);
			chunkWriter.Write(ihdr.BitDepth);
			chunkWriter.Write(ihdr.ColourType);
			chunkWriter.Write(ihdr.CompressionMethod);
			chunkWriter.Write(ihdr.FilterMethod);
			chunkWriter.Write(ihdr.InterlaceMethod);
		}

		public static void WriteAcTL(this PngWriter pngWriter, ChunkAcTL actl) {

			var chunkWriter = pngWriter.StartChunk(ChunkAcTL.Name, ChunkAcTL.Length);
			chunkWriter.Write(actl.FramesCount);
			chunkWriter.Write(actl.PlaysCount);
		}

		public static void WriteFcTL(this PngWriter pngWriter, ChunkFcTL fctl) {

			var chunkWriter = pngWriter.StartChunk(ChunkFcTL.Name, ChunkFcTL.Length);
			chunkWriter.Write(fctl.SequenceNumber);
			chunkWriter.Write(fctl.Width);
			chunkWriter.Write(fctl.Height);
			chunkWriter.Write(fctl.OffsetX);
			chunkWriter.Write(fctl.OffsetY);
			chunkWriter.Write(fctl.DelayNumerator);
			chunkWriter.Write(fctl.DelayDenominator);
			chunkWriter.Write(fctl.DisposeOperation);
			chunkWriter.Write(fctl.BlendOperation);
		}

		public static void WriteTEXt(this PngWriter pngWriter, ChunkTEXt text) {

			var chunkWriter = pngWriter.StartChunk(ChunkTEXt.Name, text.Length);
			chunkWriter.Write(text.Keyword);
			chunkWriter.Write((byte)0);  // null char -- delimiter
			chunkWriter.Write(text.Text);
		}


		public static void CopyFrom(this PngWriter.ChunkWriter writer, PngReader.ChunkReader source) {

			int readedBytes;
			while ((readedBytes = source.Read(buffer, 0, bufferLength)) > 0) {
				writer.Write(buffer, 0, readedBytes);
			}

		}


		#endregion


	}
}

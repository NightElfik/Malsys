
namespace Malsys.BitmapRenderers.Png.Chunks {
	public class ChunkIHDR {

		public static readonly string Name = "IHDR";
		public static readonly uint Length = 13;

		public uint Width;
		public uint Height;

		public byte BitDepth;
		public byte ColourType;

		public byte CompressionMethod;

		public byte FilterMethod;

		public byte InterlaceMethod;

	}
}

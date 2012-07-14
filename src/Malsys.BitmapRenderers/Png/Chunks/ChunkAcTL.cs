
namespace Malsys.BitmapRenderers.Png.Chunks {
	/// <summary>
	/// Animation Control Chunk.
	/// </summary>
	public class ChunkAcTL {

		public static readonly string Name = "acTL";
		public static readonly uint Length = 8;

		/// <summary>
		/// Total number of frames in the animation
		/// </summary>
		public uint FramesCount;

		/// <summary>
		/// Number of times that the animation should play.
		/// If it is 0, the animation should play indefinitely.
		/// If nonzero, the animation should come to rest on the final frame at the end of the last play.
		/// </summary>
		public uint PlaysCount;

	}
}

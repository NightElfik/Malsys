
namespace Malsys.BitmapRenderers.Png.Chunks {
	/// <summary>
	/// Frame Control Chunk
	/// </summary>
	public class ChunkFcTL {

		public static readonly string Name = "fcTL";
		public static readonly uint Length = 26;

		public uint SequenceNumber;

		public uint Width;
		public uint Height;

		public uint OffsetX;
		public uint OffsetY;

		/// <summary>
		/// Specifies numerator of fraction indicating the time to display the current frame, in seconds.
		/// </summary>
		/// <remarks>
		/// If the the value of the numerator is 0 the decoder should render the next frame as quickly as possible, though viewers may impose a reasonable lower bound.
		/// </remarks>
		public ushort DelayNumerator;
		/// <summary>
		/// Specifies denominator of fraction indicating the time to display the current frame, in seconds.
		/// </summary>
		/// <remarks>
		/// If the denominator is 0, it is to be treated as if it were 100 (that is, DelayNumerator then specifies 1/100ths of a second).
		/// </remarks>
		public ushort DelayDenominator;

		/// <summary>
		/// Specifies how the output buffer should be changed at the end of the delay (before rendering the next frame).
		/// </summary>
		/// <remarks>
		/// 0 – no disposal is done on this frame before rendering the next; the contents of the output buffer are left as is.
		/// 1 – the frame's region of the output buffer is to be cleared to fully transparent black before rendering the next frame.
		/// 2 – the frame's region of the output buffer is to be reverted to the previous contents before rendering the next frame.
		/// </remarks>
		public byte DisposeOperation;

		/// <summary>
		/// Specifies whether the frame is to be alpha blended into the current output buffer content, or whether it should completely replace its region in the output buffer.
		/// </summary>
		/// <remarks>
		/// 0 – all color components of the frame, including alpha, overwrite the current contents of the frame's output buffer region.
		/// 1 – the frame should be composited onto the output buffer based on its alpha, using a simple OVER operation as described in the
		///		"Alpha Channel Processing" section of the PNG specification.
		/// </remarks>
		public byte BlendOperation;

	}
}

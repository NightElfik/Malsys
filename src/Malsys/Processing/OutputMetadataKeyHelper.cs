
namespace Malsys.Processing {
	/// <summary>
	/// Helper class for easy work with magic metadata keys.
	/// </summary>
	public static class OutputMetadataKeyHelper {

		/// <summary>
		/// Suggested file extension.
		/// </summary>
		public const string OutputSuggestedFileExt = "fileExt";

		/// <summary>
		/// Output is g-zipped.
		/// Expected value is bool.
		/// </summary>
		public const string OutputIsGZipped = "gzipped";

		/// <summary>
		/// Output is ASCII art.
		/// Expected value is bool.
		/// </summary>
		public const string OutputIsAsciiArt = "asciiArt";

		/// <remarks>
		/// Output contains more packed outputs.
		/// Expected value is bool.
		/// </remarks>
		public const string PackedOutputs = "packed";

		/// <remarks>
		/// Width of an output.
		/// Expected value is positive integer.
		/// </remarks>
		public const string OutputWidth = "width";

		/// <remarks>
		/// Height of an output.
		/// Expected value is positive integer.
		/// </remarks>
		public const string OutputHeight = "height";

		/// <summary>
		/// Top offset from original position (after trimming).
		/// Expected value is positive integer.
		/// </summary>
		public const string OffsetTop = "offsetTop";

		/// <summary>
		/// Right offset from original position (after trimming).
		/// Expected value is positive integer.
		/// </summary>
		public const string OffsetRight = "offsetRight";

		/// <summary>
		/// Bottom offset from original position (after trimming).
		/// Expected value is positive integer.
		/// </summary>
		public const string OffsetBottom = "offsetBottom";

		/// <summary>
		/// Left offset from original position (after trimming).
		/// Expected value is positive integer.
		/// </summary>
		public const string OffsetLeft = "offsetLeft";

		/// <remarks>
		/// Output is PNG animation.
		/// Expected value is bool.
		/// </remarks>
		public const string OutputInPngAnimation = "pngAnimation";

		/// <remarks>
		/// Output is OBJ + MTL + JSON archive.
		/// Expected value is a string of OBJ, MTL and JSON files separated by space.
		/// </remarks>
		public const string ObjMetadata = "objMetadata";

	}
}

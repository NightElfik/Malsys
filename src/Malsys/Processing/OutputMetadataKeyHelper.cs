// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.

namespace Malsys.Processing {
	/// <summary>
	/// Helper class for easy work with magic metadata keys.
	/// </summary>
	public static class OutputMetadataKeyHelper {

		/// <remarks>
		/// Suggested file extension.
		/// </remarks>
		public const string OutputSuggestedFileExt = "fileExt";

		/// <remarks>
		/// Expected value is bool.
		/// </remarks>
		public const string OutputIsGZipped = "gzipped";

		/// <remarks>
		/// Expected value is bool.
		/// </remarks>
		public const string OutputIsAsciiArt = "asciiArt";

		/// <remarks>
		/// Expected value is bool.
		/// </remarks>
		public const string PackedOutputs = "packed";

		/// <remarks>
		/// Expected value is positive integer.
		/// </remarks>
		public const string OutputWidth = "width";

		/// <remarks>
		/// Expected value is positive integer.
		/// </remarks>
		public const string OutputHeight = "height";

		/// <summary>
		/// Top offset from original position (after trimming).
		/// </summary>
		/// <remarks>
		/// Expected value is positive integer.
		/// </remarks>
		public const string OffsetTop = "offsetTop";

		/// <summary>
		/// Right offset from original position (after trimming).
		/// </summary>
		/// <remarks>
		/// Expected value is positive integer.
		/// </remarks>
		public const string OffsetRight = "offsetRight";

		/// <summary>
		/// Bottom offset from original position (after trimming).
		/// </summary>
		/// <remarks>
		/// Expected value is positive integer.
		/// </remarks>
		public const string OffsetBottom = "offsetBottom";

		/// <summary>
		/// Left offset from original position (after trimming).
		/// </summary>
		/// <remarks>
		/// Expected value is positive integer.
		/// </remarks>
		public const string OffsetLeft = "offsetLeft";

		/// <remarks>
		/// Expected value is bool.
		/// </remarks>
		public const string OutputInPngAnimation = "pngAnimation";

	}
}

/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */

namespace Malsys.Processing {
	/// <summary>
	/// Helper class to avoid typos in output metadata keys and values.
	/// </summary>
	public static class OutputMetadataKeyHelper {

		/// <summary>
		/// Expected value is bool.
		/// </summary>
		public const string OutputIsGZipped = "gzipped";

		/// <summary>
		/// Expected value is bool.
		/// </summary>
		public const string OutputIsAsciiArt = "asciiArt";

		/// <summary>
		/// Expected value is bool.
		/// </summary>
		public const string PackedOutputs = "packed";

		/// <summary>
		/// Width of output image. Expected value is positive integer.
		/// </summary>
		public const string OutputWidth = "width";

		/// <summary>
		/// Height of output image. Expected value is positive integer.
		/// </summary>
		public const string OutputHeight = "height";

	}
}

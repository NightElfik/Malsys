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
		public const string OutputIsGZipped = "Output is gzipped";

		/// <summary>
		/// Expected value is bool.
		/// </summary>
		public const string OutputIsAsciiArt = "Ascii art";

		/// <summary>
		/// Expected value is bool.
		/// </summary>
		public const string PackedOutputs = "All outputs packed";

		/// <summary>
		/// Width of output image. Expected value is positive integer.
		/// </summary>
		public const string OutputWidth = "Width";

		/// <summary>
		/// Height of output image. Expected value is positive integer.
		/// </summary>
		public const string OutputHeight = "Height";

	}
}

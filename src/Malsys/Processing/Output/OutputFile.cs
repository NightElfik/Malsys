using System;
using System.Collections.Generic;

namespace Malsys.Processing.Output {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class OutputFile {

		public readonly string Name;

		public readonly string FilePath;

		public readonly string MimeType;

		public readonly Type SourceType;

		public readonly KeyValuePair<string, object>[] Metadata;


		public OutputFile(string name, string filePath, string mimeType, Type sourceType, KeyValuePair<string, object>[] metadata) {

			Name = name;
			FilePath = filePath;
			MimeType = mimeType;
			SourceType = sourceType;
			Metadata = metadata;
		}

	}
}

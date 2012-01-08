using System;
using Microsoft.FSharp.Collections;

namespace Malsys.Processing.Output {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class OutputFile {

		public readonly string Name;

		public readonly string FilePath;

		public readonly string MimeType;

		public readonly Type SourceType;

		public readonly FSharpMap<string, object> AdditionalData;


		public OutputFile(string name, string filePath, string mimeType, Type sourceType, FSharpMap<string, object> additionalData) {

			Name = name;
			FilePath = filePath;
			MimeType = mimeType;
			SourceType = sourceType;
			AdditionalData = additionalData;
		}

	}
}

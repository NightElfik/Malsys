/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System;
using Microsoft.FSharp.Collections;

namespace Malsys.Processing.Output {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class OutputFile {

		public readonly string Name;

		public readonly string FilePath;

		public readonly string MimeType;

		public readonly Type SourceType;

		public readonly FSharpMap<string, object> Metadata;


		public OutputFile(string name, string filePath, string mimeType, Type sourceType, FSharpMap<string, object> metadata) {

			Name = name;
			FilePath = filePath;
			MimeType = mimeType;
			SourceType = sourceType;
			Metadata = metadata;
		}

	}
}

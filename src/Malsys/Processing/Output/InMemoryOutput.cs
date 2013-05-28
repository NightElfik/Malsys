// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System;
using Microsoft.FSharp.Collections;

namespace Malsys.Processing.Output {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class InMemoryOutput {

		public readonly byte[] OutputData;

		public readonly string MimeType;

		public readonly Type SourceType;

		public FSharpMap<string, object> AdditionalData;


		public InMemoryOutput(byte[] outputData, string mimeType, Type sourceType, FSharpMap<string, object> additionalData) {

			OutputData = outputData;
			MimeType = mimeType;
			SourceType = sourceType;
			AdditionalData = additionalData;

		}

	}
}

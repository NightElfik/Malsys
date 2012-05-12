using System.Collections.Generic;
/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System.IO;

namespace Malsys.Processing {
	public interface IOutputProvider {

		/// <summary>
		/// Returns opened <see cref="Stream" /> for saving output.
		/// </summary>
		/// <remarks>
		/// The stream will be automatically closed at the end of the processing.
		/// If some service like <see cref="StreamWriter" /> uses the stream, caller should not forget to flush it before leaving.
		/// If <see cref="IOutputProvider" /> closes underlying stream, <see cref="StreamWriter" /> will not be able to flush remaining data.
		/// </remarks>
		/// <typeparam name="TCaller">Caller type for identification of the caller.</typeparam>
		/// <param name="outputName">Name of output (for user).</param>
		/// <param name="mimeType">Mime type of the output.</param>
		/// <param name="temp">If true, contents saved in the stream will be erased at the end of processing.</param>
		/// <param name="metadata">Metadata associated with output.</param>
		Stream GetOutputStream<TCaller>(string outputName, string mimeType, bool temp = false, IDictionary<string, object> metadata = null);

		/// <summary>
		/// Adds metadata to output identified by stream.
		/// </summary>
		void AddMetadata(Stream outputStream, string key, object value);

	}
}

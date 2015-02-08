using System.Collections.Generic;
using System.IO;
using Microsoft.FSharp.Collections;

namespace Malsys.Processing {
	public interface IOutputProvider {

		/// <summary>
		/// Returns opened <see cref="Stream" /> for saving output.
		/// </summary>
		/// <remarks>
		/// The stream will be automatically closed at the end of the processing thus, components should not close it
		/// to enable seeking in stream by other services.
		/// If some class like StreamWriter uses the stream, caller should not forget to flush it before leaving.
		/// If IOutputProvider closes underlying stream, StreamWriter will not be able to flush remaining data.
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

		/// <summary>
		/// Returns all metadata associated with given stream so far.
		/// </summary>
		/// <remarks>
		/// Metadata added to the stream later will not appear in returned collection (returned collection is immutable).
		/// </remarks>
		FSharpMap<string, object> GetMetadata(Stream outputStream);

		/// <summary>
		/// Returns number of provided outputs.
		/// </summary>
		int OutputsCount { get; }
	}
}

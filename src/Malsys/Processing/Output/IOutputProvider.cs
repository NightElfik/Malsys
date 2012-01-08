using System.IO;
using Microsoft.FSharp.Collections;

namespace Malsys.Processing {
	public interface IOutputProvider {

		/// <summary>
		/// Returns opened <see cref="Stream" /> for saving output.
		/// </summary>
		/// <remarks>
		/// The stream will be automatically closed at the end of the processing.
		/// If some service like <see cref="StreamWriter" /> uses the stream,
		/// caller should not forget to flush it before leaving.
		/// If <see cref="IOutputProvider" /> closes underlying stream,
		/// <see cref="StreamWriter" /> will not be able to flush remaining data.
		/// </remarks>
		/// <typeparam name="TCaller">Caller type for identification of the caller.</typeparam>
		/// <param name="outputName">Name of output (for user).</param>
		/// <param name="mimeType">Mime type of the output.</param>
		/// <param name="temp">If true, contents saved in the stream will be erased at the end of processing.
		///		Can be used for saving temporary data.</param>
		/// <param name="additionalData">Additional data about output.
		///		Data can be added even after this call using method AddAdditionalData.</param>
		Stream GetOutputStream<TCaller>(string outputName, string mimeType, bool temp = false, FSharpMap<string, object> additionalData = null);

		void AddAdditionalData(Stream outputStream, string key, object value);

	}
}

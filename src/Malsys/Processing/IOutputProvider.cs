using System.IO;

namespace Malsys.Processing {
	public interface IOutputProvider {

		/// <summary>
		/// Created new file and returns opened <see cref="FileStream" />.
		/// The stream will be automatically closed at the end of the processing.
		/// </summary>
		/// <typeparam name="TCaller">Caller type for identification caller.</typeparam>
		/// <param name="fileNameSuffix">Suffix of stream identifier. If <see cref="IFilesManager" /> is implemented
		///	to return file streams this suffix will be suffix of file name.</param>
		/// <param name="temp">If true, contents saved in the stream will be erased at the end of processing.</param>
		FileStream GetOutputStream<TCaller>(string fileNameSuffix, bool temp = false);

	}
}

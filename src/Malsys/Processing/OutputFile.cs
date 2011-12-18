
namespace Malsys.Processing {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class OutputFile {

		public readonly string FilePath;

		public readonly string LsystemName;

		public readonly string Source;


		public OutputFile(string filePath, string lsysName, string src) {
			FilePath = filePath;
			LsystemName = lsysName;
			Source = src;
		}

	}
}

using Malsys.SemanticModel.Evaluated;

namespace Malsys.Processing {
	public interface IFilesManager : IOutputProvider {

		LsystemEvaled CurrentLsystem { get; set; }

		/// <summary>
		/// Closes all opened streams and deletes temporary files.
		/// </summary>
		void Cleanup();

	}
}

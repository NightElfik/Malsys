
namespace Malsys.SemanticModel.Compiled {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class ProcessComponentsConnection {

		public readonly string SourceName;
		public readonly string TargetName;
		public readonly string TargetInputName;


		public ProcessComponentsConnection(string sourceName, string targetName, string targetInputName) {

			SourceName = sourceName;
			TargetName = targetName;
			TargetInputName = targetInputName;
		}

	}
}

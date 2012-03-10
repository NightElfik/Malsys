
namespace Malsys.SemanticModel.Compiled {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class ProcessComponentsConnection {

		public readonly bool IsVirtual;
		public readonly string SourceName;
		public readonly string TargetName;
		public readonly string TargetInputName;


		public ProcessComponentsConnection(bool isVirtual, string sourceName, string targetName, string targetInputName) {

			IsVirtual = isVirtual;
			SourceName = sourceName;
			TargetName = targetName;
			TargetInputName = targetInputName;

		}

	}
}

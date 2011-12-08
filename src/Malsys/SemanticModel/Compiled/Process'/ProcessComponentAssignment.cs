
namespace Malsys.SemanticModel.Compiled {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class ProcessComponentAssignment {

		public readonly string ComponentTypeName;
		public readonly string ContainerName;

		public ProcessComponentAssignment(string component, string container) {

			ComponentTypeName = component;
			ContainerName = container;
		}
	}
}

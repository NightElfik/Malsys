
namespace Malsys.SemanticModel.Compiled {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class ProcessComponent {

		public readonly string Name;
		public readonly string TypeName;


		public ProcessComponent(string name, string typeName) {

			Name = name;
			TypeName = typeName;
		}

	}
}

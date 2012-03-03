using System.Reflection;

namespace Malsys.Reflection.Components {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class ComponentGettablePropertyMetadata {

		public readonly ImmutableList<string> Names;

		public readonly PropertyInfo PropertyInfo;


		public ComponentGettablePropertyMetadata(ImmutableList<string> names, PropertyInfo propertyInfo) {

			Names = names;
			PropertyInfo = propertyInfo;

		}

	}
}

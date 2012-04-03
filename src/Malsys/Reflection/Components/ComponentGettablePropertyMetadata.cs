using System.Reflection;

namespace Malsys.Reflection.Components {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class ComponentGettablePropertyMetadata {

		public readonly ImmutableList<string> Names;

		public readonly PropertyInfo PropertyInfo;

		public readonly bool IsGettableBeforeInitialiation;


		public ComponentGettablePropertyMetadata(ImmutableList<string> names, PropertyInfo propertyInfo, bool isGettableBeforeInitialiation) {

			Names = names;
			PropertyInfo = propertyInfo;
			IsGettableBeforeInitialiation = isGettableBeforeInitialiation;

		}

	}
}

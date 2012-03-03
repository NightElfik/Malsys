using System.Reflection;
using Malsys.SemanticModel.Evaluated;
using System;

namespace Malsys.Reflection.Components {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class ComponentConnectablePropertyMetadata {

		public readonly ImmutableList<string> Names;

		/// <summary>
		/// Indicating whether component connection is optional.
		/// </summary>
		public readonly bool IsOptional;

		/// <summary>
		/// Indicating whether more than one component can be connected.
		/// </summary>
		public readonly bool AllowMultiple;

		public readonly PropertyInfo PropertyInfo;


		public Type PropertyType { get { return PropertyInfo.PropertyType; } }


		public ComponentConnectablePropertyMetadata(ImmutableList<string> names, PropertyInfo propertyInfo, bool isOptional, bool allowMultiple) {

			Names = names;
			PropertyInfo = propertyInfo;
			IsOptional = isOptional;
			AllowMultiple = allowMultiple;

		}

	}
}

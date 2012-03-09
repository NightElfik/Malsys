using System;
using System.Reflection;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Reflection.Components {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class ComponentSettablePropertyMetadata {

		public readonly ImmutableList<string> Names;

		public readonly bool IsMandatory;

		public readonly PropertyInfo PropertyInfo;

		/// <summary>
		/// Property type is assignable to IValue.
		/// </summary>
		public Type PropertyType { get { return PropertyInfo.PropertyType; } }

		public readonly ExpressionValueTypeFlags ExpressionValueType;


		public ComponentSettablePropertyMetadata(ImmutableList<string> names, PropertyInfo propertyInfo, bool isMandatory) {

			Names = names;
			PropertyInfo = propertyInfo;
			IsMandatory = isMandatory;

			ExpressionValueType = IValueExtensions.IValueTypeToEnum(propertyInfo.PropertyType);

		}

	}

	/// <remarks>
	/// Immutable.
	/// </remarks>
	/// <remarks>
	/// Property type is equal to ImmutableList<Symbol<IValue>>.
	/// </remarks>
	public class ComponentSettableSybolsPropertyMetadata {

		public readonly ImmutableList<string> Names;

		public readonly bool IsMandatory;

		public readonly PropertyInfo PropertyInfo;


		public ComponentSettableSybolsPropertyMetadata(ImmutableList<string> names, PropertyInfo propertyInfo, bool isMandatory) {

			Names = names;
			PropertyInfo = propertyInfo;
			IsMandatory = isMandatory;

		}

	}
}

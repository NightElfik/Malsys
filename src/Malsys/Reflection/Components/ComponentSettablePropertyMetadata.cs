using System;
using System.Reflection;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Reflection.Components {
	/// <remarks>
	/// Nearly immutable (only doc strings can be set later).
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


		public string SummaryDoc { get; private set; }
		public string ExpectedValueDoc { get; private set; }
		public string DefaultValueDoc { get; private set; }


		public ComponentSettablePropertyMetadata(ImmutableList<string> names, PropertyInfo propertyInfo, bool isMandatory) {

			Names = names;
			PropertyInfo = propertyInfo;
			IsMandatory = isMandatory;

			ExpressionValueType = IValueExtensions.IValueTypeToEnum(propertyInfo.PropertyType);

		}

		public void SetDocumentation(string summaryDoc, string expectedValueDoc, string defaultValueDoc) {
			SummaryDoc = summaryDoc;
			ExpectedValueDoc = expectedValueDoc;
			DefaultValueDoc = defaultValueDoc;
		}

	}

	/// <remarks>
	/// Nearly immutable (only doc strings can be set later).
	/// </remarks>
	/// <remarks>
	/// Property type is equal to <see cref="ImmutableList{Symbol{IValue}}"/>.
	/// </remarks>
	public class ComponentSettableSybolsPropertyMetadata {

		public readonly ImmutableList<string> Names;

		public readonly bool IsMandatory;

		public readonly PropertyInfo PropertyInfo;


		public string SummaryDoc { get; private set; }


		public ComponentSettableSybolsPropertyMetadata(ImmutableList<string> names, PropertyInfo propertyInfo, bool isMandatory) {

			Names = names;
			PropertyInfo = propertyInfo;
			IsMandatory = isMandatory;

		}

		public void SetDocumentation(string summaryDoc) {
			SummaryDoc = summaryDoc;
		}

	}
}

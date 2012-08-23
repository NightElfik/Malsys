/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
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


		public readonly string SummaryDoc;
		public readonly string ExpectedValueDoc;
		public readonly string DefaultValueDoc;
		public readonly string TypicalValueDoc;


		public ComponentSettablePropertyMetadata(ImmutableList<string> names, PropertyInfo propertyInfo, bool isMandatory,
				string summaryDoc = null, string expectedValueDoc = null, string defaultValueDoc = null, string typicalValueDoc = null) {

			Names = names;
			PropertyInfo = propertyInfo;
			IsMandatory = isMandatory;

			SummaryDoc = summaryDoc ?? "";
			ExpectedValueDoc = expectedValueDoc ?? "";
			DefaultValueDoc = defaultValueDoc ?? "";
			TypicalValueDoc = typicalValueDoc ?? "";

			ExpressionValueType = IValueExtensions.IValueTypeToEnum(propertyInfo.PropertyType);

		}

	}

	/// <remarks>
	/// Immutable.
	/// </remarks>
	/// <remarks>
	/// Property type should be equal to ImmutableList{Symbol{IValue}}.
	/// </remarks>
	public class ComponentSettableSybolsPropertyMetadata {

		public readonly ImmutableList<string> Names;

		public readonly bool IsMandatory;

		public readonly PropertyInfo PropertyInfo;


		public readonly string SummaryDoc;


		public ComponentSettableSybolsPropertyMetadata(ImmutableList<string> names, PropertyInfo propertyInfo, bool isMandatory, string summaryDoc = null) {

			Names = names;
			PropertyInfo = propertyInfo;
			IsMandatory = isMandatory;

			SummaryDoc = summaryDoc ?? "";

		}

	}
}

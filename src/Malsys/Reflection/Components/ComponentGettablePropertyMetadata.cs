using System.Reflection;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Reflection.Components {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class ComponentGettablePropertyMetadata {

		public readonly ImmutableList<string> Names;

		public readonly PropertyInfo PropertyInfo;

		public readonly bool IsGettableBeforeInitialiation;

		public readonly ExpressionValueTypeFlags ExpressionValueType;


		public readonly string SummaryDoc;



		public ComponentGettablePropertyMetadata(ImmutableList<string> names, PropertyInfo propertyInfo, bool isGettableBeforeInitialiation, string summaryDoc = null) {

			Names = names;
			PropertyInfo = propertyInfo;
			IsGettableBeforeInitialiation = isGettableBeforeInitialiation;

			SummaryDoc = summaryDoc ?? "";

			ExpressionValueType = IValueExtensions.IValueTypeToEnum(propertyInfo.PropertyType);

		}


	}
}

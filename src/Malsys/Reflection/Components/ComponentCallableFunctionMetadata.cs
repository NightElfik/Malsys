// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System.Reflection;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Reflection.Components {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class ComponentCallableFunctionMetadata {

		public readonly ImmutableList<string> Names;

		public readonly MethodInfo MethodInfo;

		public readonly int ParamsCount;

		public readonly ImmutableList<ExpressionValueTypeFlags> ParamsTypesCyclicPattern;

		public readonly bool IsGettableBeforeInitialiation;

		public readonly ExpressionValueTypeFlags ExpressionValueReturnType;


		public readonly string SummaryDoc;
		public readonly string ParamsDoc;



		public ComponentCallableFunctionMetadata(ImmutableList<string> names, MethodInfo methodInfo, int paramsCount,
				ImmutableList<ExpressionValueTypeFlags> paramsTypesCyclicPattern, bool isGettableBeforeInitialiation, string summaryDoc = null, string paramsDoc = null) {

			Names = names;
			ParamsCount = paramsCount;
			ParamsTypesCyclicPattern = paramsTypesCyclicPattern;
			MethodInfo = methodInfo;
			IsGettableBeforeInitialiation = isGettableBeforeInitialiation;

			SummaryDoc = summaryDoc ?? "";
			ParamsDoc = paramsDoc ?? "";

			ExpressionValueReturnType = IValueExtensions.IValueTypeToEnum(MethodInfo.ReturnType);

		}


	}
}

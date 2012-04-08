using System;
using System.Diagnostics.Contracts;
using System.Reflection;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Reflection.Components {
	/// <remarks>
	/// Nearly immutable (only doc strings can be set later).
	/// </remarks>
	public class ComponentCallableFunctionMetadata {

		public readonly ImmutableList<string> Names;

		public readonly MethodInfo MethodInfo;

		public readonly int ParamsCount;

		public readonly ImmutableList<ExpressionValueTypeFlags> ParamsTypesCyclicPattern;

		public readonly bool IsGettableBeforeInitialiation;

		public readonly ExpressionValueTypeFlags ExpressionValueReturnType;


		public string SummaryDoc { get; private set; }
		public string ParamsDoc { get; private set; }



		public ComponentCallableFunctionMetadata(ImmutableList<string> names, MethodInfo methodInfo, int paramsCount,
				ImmutableList<ExpressionValueTypeFlags> paramsTypesCyclicPattern, bool isGettableBeforeInitialiation) {

			Names = names;
			ParamsCount = paramsCount;
			ParamsTypesCyclicPattern = paramsTypesCyclicPattern;
			MethodInfo = methodInfo;
			IsGettableBeforeInitialiation = isGettableBeforeInitialiation;

			ExpressionValueReturnType = IValueExtensions.IValueTypeToEnum(MethodInfo.ReturnType);

		}

		public void SetDocumentation(string summaryDoc, string paramsDoc) {

			Contract.Requires<ArgumentNullException>(summaryDoc != null);
			Contract.Requires<ArgumentNullException>(paramsDoc != null);

			SummaryDoc = summaryDoc;
			ParamsDoc = paramsDoc;

		}

	}
}

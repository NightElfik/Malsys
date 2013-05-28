// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System.Reflection;

namespace Malsys.Reflection.Components {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class ComponentInterpretationMethodMetadata {

		public readonly ImmutableList<string> Names;

		public readonly MethodInfo MethodInfo;

		public readonly int ParamsCount;

		public readonly int MandatoryParamsCount;


		public readonly string SummaryDoc;
		public readonly string ParamsDoc;


		public ComponentInterpretationMethodMetadata(ImmutableList<string> names, MethodInfo methodInfo, int paramsCount, int mandatoryParamsCount,
				string summaryDoc = null, string paramsDoc = null) {

			Names = names;
			ParamsCount = paramsCount;
			MethodInfo = methodInfo;
			MandatoryParamsCount = mandatoryParamsCount;

			SummaryDoc = summaryDoc ?? "";
			ParamsDoc = paramsDoc ?? "";

		}


	}
}

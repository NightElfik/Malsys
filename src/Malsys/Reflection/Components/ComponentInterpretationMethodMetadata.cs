using System;
using System.Diagnostics.Contracts;
using System.Reflection;

namespace Malsys.Reflection.Components {
	/// <remarks>
	/// Nearly immutable (only doc strings can be set later).
	/// </remarks>
	public class ComponentInterpretationMethodMetadata {

		public readonly ImmutableList<string> Names;

		public readonly MethodInfo MethodInfo;

		public readonly int ParamsCount;

		public readonly int MandatoryParamsCount;


		public string SummaryDoc { get; private set; }

		public string ParamsDoc { get; private set; }


		public ComponentInterpretationMethodMetadata(ImmutableList<string> names, MethodInfo methodInfo, int paramsCount, int mandatoryParamsCount) {

			Names = names;
			ParamsCount = paramsCount;
			MethodInfo = methodInfo;
			MandatoryParamsCount = mandatoryParamsCount;

		}


		public void SetDocumentation(string summaryDoc, string paramsDoc) {

			Contract.Requires<ArgumentNullException>(summaryDoc != null);
			Contract.Requires<ArgumentNullException>(paramsDoc != null);

			SummaryDoc = summaryDoc;
			ParamsDoc = paramsDoc;
		}

	}
}

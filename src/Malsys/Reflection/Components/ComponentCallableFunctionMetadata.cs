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



		public ComponentCallableFunctionMetadata(ImmutableList<string> names, MethodInfo methodInfo, int paramsCount,
				ImmutableList<ExpressionValueTypeFlags> paramsTypesCyclicPattern) {

			Names = names;
			ParamsCount = paramsCount;
			ParamsTypesCyclicPattern = paramsTypesCyclicPattern;
			MethodInfo = methodInfo;

		}

	}
}

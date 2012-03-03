using System.Reflection;

namespace Malsys.Reflection.Components {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class ComponentCallableFunctionMetadata {

		public readonly ImmutableList<string> Names;

		public readonly MethodInfo MethodInfo;


		public ComponentCallableFunctionMetadata(ImmutableList<string> names, MethodInfo methodInfo) {

			Names = names;
			MethodInfo = methodInfo;

		}

	}
}

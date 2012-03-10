using System;

namespace Malsys.Processing.Components.Interpreters {
	/// <summary>
	/// Indicates that marked method represents symbol interpretation method.
	/// </summary>
	/// <remarks>
	/// Marked method must have just one parameter of type IValue[].
	/// Return type must be void.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
	public sealed class SymbolInterpretationAttribute : Attribute {

		/// <summary>
		/// Minimal arguments count (length of IValue[]) to invoke marked method.
		/// </summary>
		public readonly int RequiredParametersCount;


		public SymbolInterpretationAttribute(int requiredParamsCount = 0) {
			RequiredParametersCount = requiredParamsCount;
		}


	}
}

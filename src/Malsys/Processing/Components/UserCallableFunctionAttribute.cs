using System;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Processing.Components {
	/// <summary>
	/// Indicates that marked method can be called by user from input code.
	/// </summary>
	/// <remarks>
	/// Marked method must have two parameters of types IValue[] and IExpressionEvaluatorContext.
	/// Return type must be assignable to IValue.
	/// Attribute inherence do not work on properties in interface, do not forget to add it to derived types too.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
	public sealed class UserCallableFunctionAttribute : Attribute {

		public int ParamsCount { get; set; }

		public ExpressionValueTypeFlags[] ParamsTypesCyclicPattern { get; set; }


		public UserCallableFunctionAttribute() {
			ParamsCount = 0;
			ParamsTypesCyclicPattern = new ExpressionValueTypeFlags[0];
		}

		public UserCallableFunctionAttribute(int paramsCount) {
			ParamsTypesCyclicPattern = new ExpressionValueTypeFlags[1] { ExpressionValueTypeFlags.Any };
		}

		public UserCallableFunctionAttribute(int paramsCount, params ExpressionValueTypeFlags[] paramsTypesCyclicPattern) {
			ParamsCount = paramsCount;
			ParamsTypesCyclicPattern = paramsTypesCyclicPattern;
		}

	}
}

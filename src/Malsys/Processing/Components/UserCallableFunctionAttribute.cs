// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Processing.Components {
	/// <summary>
	/// Indicates that marked method can be called by user from input code.
	/// </summary>
	/// <remarks>
	/// Marked method must have two parameters of types IValue[] and IExpressionEvaluatorContext.
	/// Return type must be assignable to IValue.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
	public sealed class UserCallableFunctionAttribute : Attribute {

		public int ParamsCount { get; set; }

		public ExpressionValueTypeFlags[] ParamsTypesCyclicPattern { get; set; }

		public bool IsCallableBeforeInitialiation { get; set; }


		/// <summary>
		/// Marks function that takes no parameters.
		/// </summary>
		public UserCallableFunctionAttribute() {
			ParamsCount = 0;
			ParamsTypesCyclicPattern = new ExpressionValueTypeFlags[0];
		}

		/// <summary>
		/// Marks function that takes given count of parameters with any value type.
		/// </summary>
		public UserCallableFunctionAttribute(int paramsCount) {
			ParamsTypesCyclicPattern = new ExpressionValueTypeFlags[1] { ExpressionValueTypeFlags.Any };
		}

		/// <summary>
		/// Marks function that takes given count of parameters given value type pattern.
		/// </summary>
		public UserCallableFunctionAttribute(int paramsCount, params ExpressionValueTypeFlags[] paramsTypesCyclicPattern) {
			ParamsCount = paramsCount;
			ParamsTypesCyclicPattern = paramsTypesCyclicPattern;
		}

	}
}

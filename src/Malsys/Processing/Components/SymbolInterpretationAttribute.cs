// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System;
using System.Diagnostics.Contracts;

namespace Malsys.Processing.Components {
	/// <summary>
	/// Indicates that marked method represents symbol interpretation method.
	/// </summary>
	/// <remarks>
	/// Marked method must have just one parameter of type ArgsStorage.
	/// Return type must be void.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
	public sealed class SymbolInterpretationAttribute : Attribute {

		/// <summary>
		/// Actual parameters count.
		/// </summary>
		public readonly int ParametersCount;

		/// <summary>
		/// Minimal arguments count to invoke marked method.
		/// </summary>
		public readonly int RequiredParametersCount;


		public SymbolInterpretationAttribute(int parametersCount = 0, int requiredParamsCount = 0) {

			Contract.Requires<ArgumentException>(requiredParamsCount <= parametersCount);

			ParametersCount = parametersCount;
			RequiredParametersCount = requiredParamsCount;

		}


	}
}

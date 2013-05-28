// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
﻿using System;
using System.Diagnostics.Contracts;
using Malsys.Evaluators;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Resources {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class FunctionCore {

		/// <summary>
		/// If function have params count equal to <c>FunctionInfo.AnyParamsCount</c> constant, any number of arguments can be supplied.
		/// </summary>
		public readonly int ParametersCount;

		/// <summary>
		/// Evaluation function.
		/// </summary>
		public readonly Func<IValue[], IExpressionEvaluatorContext, IValue> EvalFunction;

		/// <summary>
		/// Cyclic pattern of types of parameters of function.
		/// </summary>
		public readonly ImmutableList<ExpressionValueTypeFlags> ParamsTypesCyclicPattern;



		public FunctionCore(int paramsCount, ImmutableList<ExpressionValueTypeFlags> paramsTypesCyclicPattern, Func<IValue[], IExpressionEvaluatorContext, IValue> evalFunc) {

			Contract.Requires<ArgumentOutOfRangeException>(paramsCount >= 0);
			Contract.Requires<ArgumentNullException>(evalFunc != null);
			Contract.Requires<ArgumentNullException>(paramsTypesCyclicPattern != null);


			ParametersCount = paramsCount;
			EvalFunction = evalFunc;

			if (paramsCount == 0) {
				ParamsTypesCyclicPattern = ImmutableList<ExpressionValueTypeFlags>.Empty;
			}
			else {
				ParamsTypesCyclicPattern = paramsTypesCyclicPattern;
			}

		}

	}
}
// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Evaluators {
	/// <summary>
	/// Object for storing the variable name and evaluation delegate in the IExpressionEvaluatorContext.
	/// </summary>
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class VariableInfo {

		public readonly string Name;
		public readonly Func<IValue> ValueFunc;
		public readonly object Metadata;


		public VariableInfo(string name, Func<IValue> valueFunc, object metadata = null) {
			Name = name;
			ValueFunc = valueFunc;
			Metadata = metadata;
		}

	}
}

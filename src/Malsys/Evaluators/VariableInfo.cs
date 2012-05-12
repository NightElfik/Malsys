/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Evaluators {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class VariableInfo {

		public readonly string Name;
		public readonly Func<IValue> ValueFunc;
		public readonly object Metadata;


		public VariableInfo(string name, Func<IValue> valueFunc, object metadata) {
			Name = name;
			ValueFunc = valueFunc;
			Metadata = metadata;
		}

	}
}

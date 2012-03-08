using System;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Evaluators {
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

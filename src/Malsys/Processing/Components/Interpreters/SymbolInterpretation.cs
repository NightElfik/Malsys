using System;

namespace Malsys.Processing.Components.Interpreters {
	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
	public sealed class SymbolInterpretationAttribute : Attribute {


		public SymbolInterpretationAttribute(string doc) {
			Documentation = doc;
		}

		public SymbolInterpretationAttribute(int requiredParamsCount, string doc) {
			Documentation = doc;
			RequiredParametersCount = requiredParamsCount;
		}


		public string Documentation { get; private set; }

		public int RequiredParametersCount { get; private set; }

	}
}

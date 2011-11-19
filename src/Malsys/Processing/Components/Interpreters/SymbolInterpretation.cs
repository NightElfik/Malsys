using System;

namespace Malsys.Processing.Components.Interpreters {
	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
	public sealed class SymbolInterpretationAttribute : Attribute {

		public SymbolInterpretationAttribute() {
		}

	}
}

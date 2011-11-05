using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Malsys.Interpreters {
	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
	public sealed class SymbolInterpretationAttribute : Attribute {

		public SymbolInterpretationAttribute() {
		}

	}
}

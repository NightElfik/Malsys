using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Malsys.Expressions;

namespace Malsys.Rewriters {
	public interface ISymbolProcessor {

		void ProcessSymbol(Symbol<IValue> symbol);

		void EndProcessing();

	}
}

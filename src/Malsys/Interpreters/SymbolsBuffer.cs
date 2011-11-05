using System.Collections.Generic;
using Malsys.Rewriters;
using Symbol = Malsys.Symbol<Malsys.Expressions.IValue>;

namespace Malsys.Interpreters {
	public class SymbolsBuffer : ISymbolProcessor {

		List<Symbol> buffer = new List<Symbol>();


		#region ISymbolProcessor Members

		public void ProcessSymbol(Symbol symbol) {


			buffer.Add(symbol);
		}

		public void EndProcessing() {

		}

		#endregion


		public List<Symbol> GetAndClear() {
			var buff = buffer;
			buffer = new List<Symbol>();
			return buff;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbol = Malsys.Symbol<Malsys.Expressions.IValue>;
using SymbolPatern = Malsys.Symbol<string>;
using Malsys.Expressions;
using Malsys.Renderers;

namespace Malsys.Interpreters {
	public class Basic2DInterpreter : IInterpreter {

		IBasic2DRenderer renderer;


		public Basic2DInterpreter(IBasic2DRenderer rend) {
			renderer = rend;
		}


		#region IInterpret Members

		public void Initialize() {
			throw new NotImplementedException();
		}

		#endregion


		#region Symbols interpretation methods

		[SymbolInterpretation]
		public void Nothing(ImmutableList<IValue> prms) {

		}

		[SymbolInterpretation]
		public void DrawLineForward(ImmutableList<IValue> prms) {

		}

		[SymbolInterpretation]
		public void MoveForward(ImmutableList<IValue> prms) {

		}

		#endregion

	}
}

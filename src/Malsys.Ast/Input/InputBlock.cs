using System.Collections.Generic;

namespace Malsys.Ast {
	public class InputBlock : ImmutableList<IInputStatement> {

		public InputBlock()
			: base(ImmutableList<IInputStatement>.Empty) {
		}

		public InputBlock(IEnumerable<IInputStatement> statements)
			: base(statements) {
		}
	}
}

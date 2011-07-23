using System.Collections.Generic;

namespace Malsys.Ast {
	public class SymbolPatternsList : ImmutableList<SymbolPattern>, IAstVisitable {

		public SymbolPatternsList(Position pos) : base(pos) { }

		public SymbolPatternsList(IEnumerable<SymbolPattern> vals, Position pos) : base(vals, pos) { }


		#region IAstVisitable Members

		public void Accept(IAstVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}

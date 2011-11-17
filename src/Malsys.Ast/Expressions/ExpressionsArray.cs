using System.Collections.Generic;

namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class ExpressionsArray : ImmutableListPos<Expression>, IExpressionMember {

		public ExpressionsArray(Position pos)
			: base(ImmutableList<Expression>.Empty, pos) {

		}

		public ExpressionsArray(ImmutableListPos<Expression> vals, Position beginSep, Position endSep)
			: base(vals, beginSep, endSep, vals.Position) {

		}


		#region IAstExpressionVisitable Members

		public void Accept(IExpressionVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}

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


		#region IExpressionMember Members

		public ExpressionMemberType MemberType { get { return ExpressionMemberType.Array; } }

		#endregion

		#region IAstVisitable Members

		new public void Accept(IAstVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}

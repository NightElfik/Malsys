using System.Collections.Generic;

namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class ExpressionsArray : ImmutableListPos<Expression>, IExpressionMember {

		public ExpressionsArray(Position pos)
			: base(ImmutableList<Expression>.Empty, pos) {

		}

		public ExpressionsArray(IEnumerable<Expression> vals, Position pos)
			: base(vals, pos) {

		}


		#region IExpressionMember Members

		public ExpressionMemberType MemberType { get { return ExpressionMemberType.Array; } }

		#endregion

		#region IAstVisitable Members

		public void Accept(IAstVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}

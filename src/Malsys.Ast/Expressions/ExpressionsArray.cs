using System.Collections.Generic;

namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class ExpressionsArray : ImmutableList<Expression>, IExpressionMember {

		public ExpressionsArray(Position pos)
			: base(ImmutableList<Expression>.Empty) {

			Position = pos;
		}

		public ExpressionsArray(IEnumerable<Expression> vals, Position pos)
			: base(vals) {

			Position = pos;
		}


		#region IToken Members

		public Position Position { get; private set; }

		#endregion

		#region IExpressionMember Members

		public ExpressionMemberType MemberType { get { return ExpressionMemberType.Array; } }

		#endregion
	}
}

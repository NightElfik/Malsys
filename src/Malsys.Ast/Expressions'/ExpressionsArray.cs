
namespace Malsys.Ast {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class ExpressionsArray : ImmutableListPos<Expression>, IExpressionMember {


		public ExpressionsArray(Position pos)
			: base(ImmutableList<Expression>.Empty, pos) {

		}

		public ExpressionsArray(ImmutableListPos<Expression> vals, Position beginSep, Position endSep)
			: base(vals, beginSep, endSep, vals.Position) {

		}


		public ExpressionMemberType MemberType {
			get { return ExpressionMemberType.ExpressionsArray; }
		}

	}
}

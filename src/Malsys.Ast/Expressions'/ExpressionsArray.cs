// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.

namespace Malsys.Ast {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class ExpressionsArray : ImmutableListPos<Expression>, IExpressionMember {


		public ExpressionsArray(PositionRange pos)
			: base(ImmutableList<Expression>.Empty, pos) {

		}

		public ExpressionsArray(ImmutableListPos<Expression> vals, PositionRange beginSep, PositionRange endSep)
			: base(vals, beginSep, endSep, vals.Position) {

		}


		public ExpressionMemberType MemberType {
			get { return ExpressionMemberType.ExpressionsArray; }
		}

	}
}

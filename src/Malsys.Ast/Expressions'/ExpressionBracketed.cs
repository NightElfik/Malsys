// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.

namespace Malsys.Ast {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class ExpressionBracketed : IExpressionMember {

		public readonly Expression Expression;

		public ExpressionBracketed(Expression expression, PositionRange pos) {
			Expression = expression;
			Position = pos;
		}


		public PositionRange Position { get; private set; }


		public ExpressionMemberType MemberType {
			get { return ExpressionMemberType.ExpressionBracketed; }
		}

	}
}

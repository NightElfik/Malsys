// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.

namespace Malsys.Ast {
	public class EmptyExpression : IExpressionMember {

		public PositionRange Position { get; private set; }


		public EmptyExpression(PositionRange pos) {
			Position = pos;
		}


		public ExpressionMemberType MemberType {
			get { return ExpressionMemberType.EmptyExpression; }
		}

	}
}

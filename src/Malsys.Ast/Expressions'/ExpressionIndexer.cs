// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.

namespace Malsys.Ast {
	public class ExpressionIndexer : IExpressionMember {

		public Expression Index;

		public PositionRange Position { get; private set; }


		public ExpressionIndexer(Expression index, PositionRange pos) {
			Index = index;
			Position = pos;
		}


		public ExpressionMemberType MemberType {
			get { return ExpressionMemberType.ExpressionIndexer; }
		}

	}
}

// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.

namespace Malsys.Ast {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class ExpressionIndexer : IExpressionMember {

		public readonly Expression Index;


		public ExpressionIndexer(Expression index, PositionRange pos) {
			Index = index;
			Position = pos;
		}



		public PositionRange Position { get; private set; }


		public ExpressionMemberType MemberType {
			get { return ExpressionMemberType.ExpressionIndexer; }
		}

	}
}

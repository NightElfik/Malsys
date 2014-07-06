// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.

namespace Malsys.Ast {
	public class ExpressionFunction : IExpressionMember {

		public Identifier NameId;
		public ListPos<Expression> Arguments;


		public ExpressionFunction(Identifier name, ListPos<Expression> args, PositionRange pos) {
			NameId = name;
			Arguments = args;
			Position = pos;
		}


		public PositionRange Position { get; private set; }


		public ExpressionMemberType MemberType {
			get { return ExpressionMemberType.ExpressionFunction; }
		}

	}
}

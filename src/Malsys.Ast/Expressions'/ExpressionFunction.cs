// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.

namespace Malsys.Ast {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class ExpressionFunction : IExpressionMember {

		public readonly Identifier NameId;
		public readonly ImmutableListPos<Expression> Arguments;


		public ExpressionFunction(Identifier name, ImmutableListPos<Expression> args, PositionRange pos) {
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

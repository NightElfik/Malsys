// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.

namespace Malsys.Ast {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class Operator : IExpressionMember {

		/// <remarks>
		/// Can be null for procedurally created operators like implicit multiplication.
		/// </remarks>
		public readonly string Syntax;


		public Operator(string syntax, PositionRange pos) {
			Syntax = syntax;
			Position = pos;
		}



		public PositionRange Position { get; private set; }


		public ExpressionMemberType MemberType {
			get { return ExpressionMemberType.Operator; }
		}

	}
}

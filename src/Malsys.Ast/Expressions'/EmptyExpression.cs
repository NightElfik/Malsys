﻿/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */

namespace Malsys.Ast {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class EmptyExpression : IExpressionMember {

		public static readonly EmptyExpression Instance = new EmptyExpression(PositionRange.Unknown);


		public EmptyExpression(PositionRange pos) {
			Position = pos;
		}


		public PositionRange Position { get; private set; }


		public ExpressionMemberType MemberType {
			get { return ExpressionMemberType.EmptyExpression; }
		}

	}
}

// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System.Collections;
using System.Collections.Generic;

namespace Malsys.Ast {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class Expression : IEnumerable<IExpressionMember>, IFunctionStatement {

		public static readonly Expression Empty = new Expression(PositionRange.Unknown);


		public readonly ImmutableList<IExpressionMember> Members;


		public Expression(PositionRange pos) {

			Members = ImmutableList<IExpressionMember>.Empty;
			Position = pos;
		}

		public Expression(IEnumerable<IExpressionMember> mmbrs, PositionRange pos) {

			Members = new ImmutableList<IExpressionMember>(mmbrs);
			Position = pos;
		}


		public IExpressionMember this[int i] { get { return Members[i]; } }

		public int MembersCount { get { return Members.Length; } }

		public bool IsEmpty { get { return Members.Length == 0; } }



		public PositionRange Position { get; private set; }


		IEnumerator IEnumerable.GetEnumerator() {
			return Members.GetEnumerator();
		}

		public IEnumerator<IExpressionMember> GetEnumerator() {
			return ((IEnumerable<IExpressionMember>)Members).GetEnumerator();
		}


		public FunctionStatementType StatementType {
			get { return FunctionStatementType.Expression; }
		}

	}
}

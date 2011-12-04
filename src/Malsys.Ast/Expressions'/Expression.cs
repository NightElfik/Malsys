using System.Collections;
using System.Collections.Generic;

namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class Expression : IEnumerable<IExpressionMember>, IFunctionStatement {

		public static readonly Expression Empty = new Expression(Position.Unknown);


		public readonly ImmutableList<IExpressionMember> Members;


		public Expression(Position pos) {

			Members = ImmutableList<IExpressionMember>.Empty;
			Position = pos;
		}

		public Expression(IEnumerable<IExpressionMember> mmbrs, Position pos) {

			Members = new ImmutableList<IExpressionMember>(mmbrs);
			Position = pos;
		}


		public IExpressionMember this[int i] { get { return Members[i]; } }

		public int MembersCount { get { return Members.Length; } }

		public bool IsEmpty { get { return Members.Length == 0; } }



		public Position Position { get; private set; }


		IEnumerator IEnumerable.GetEnumerator() {
			return Members.GetEnumerator();
		}

		public IEnumerator<IExpressionMember> GetEnumerator() {
			return ((IEnumerable<IExpressionMember>)Members).GetEnumerator();
		}

		public void Accept(IFunctionVisitor visitor) {
			visitor.Visit(this);
		}

	}
}

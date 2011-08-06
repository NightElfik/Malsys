using System.Collections;
using System.Collections.Generic;

namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class Expression : IToken, IEnumerable<IExpressionMember>, IExprInteractiveStatement {

		public readonly ImmutableList<IExpressionMember> Members;
		public readonly int MembersCount;


		public Expression(Position pos) {
			Members = ImmutableList<IExpressionMember>.Empty;
			MembersCount = 0;
			Position = pos;
		}

		public Expression(IEnumerable<IExpressionMember> mmbrs, Position pos) {
			Members = new ImmutableList<IExpressionMember>(mmbrs);
			Position = pos;

			MembersCount = Members.Length;
		}


		public IExpressionMember this[int i] { get { return Members[i]; } }

		public bool IsEmpty { get { return MembersCount == 0; } }


		#region IToken Members

		public Position Position { get; private set; }

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator() {
			return Members.GetEnumerator();
		}

		#endregion

		#region IEnumerable<IExpressionMember> Members

		public IEnumerator<IExpressionMember> GetEnumerator() {
			return ((IEnumerable<IExpressionMember>)Members).GetEnumerator();
		}

		#endregion
	}
}

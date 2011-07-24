using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class Expression : IToken, IAstVisitable, IEnumerable<IExpressionMember>, IExprInteractiveStatement {

		public IExpressionMember this[int i] { get { return members[i]; } }

		public readonly int MembersCount;

		private IExpressionMember[] members;


		public Expression(IEnumerable<IExpressionMember> mmbrs, Position pos) {
			members = mmbrs.ToArray();
			Position = pos;

			MembersCount = members.Length;
		}


		#region IToken Members

		public Position Position { get; private set; }

		#endregion

		#region IAstVisitable Members

		public void Accept(IAstVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator() {
			return members.GetEnumerator();
		}

		#endregion

		#region IEnumerable<IExpressionMember> Members

		public IEnumerator<IExpressionMember> GetEnumerator() {
			return ((IEnumerable<IExpressionMember>)members).GetEnumerator();
		}

		#endregion
	}
}

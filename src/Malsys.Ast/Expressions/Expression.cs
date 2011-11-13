using System.Collections;
using System.Collections.Generic;

namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class Expression : IBindable, IEnumerable<IExpressionMember> {

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

		#region IBindableVisitable Members

		public void Accept(IBindableVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}

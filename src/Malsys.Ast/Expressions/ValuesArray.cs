using System.Collections.Generic;
using System.Linq;

namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class ExpressionsArray : IExpressionMember {

		public Expression this[int i] { get { return values[i]; } }
		public readonly int MembersCount;

		private Expression[] values;


		public ExpressionsArray(Position pos) {
			values = new Expression[0];
			Position = pos;

			MembersCount = values.Length;
		}

		public ExpressionsArray(IEnumerable<Expression> vals, Position pos) {
			values = vals.ToArray();
			Position = pos;

			MembersCount = values.Length;
		}


		#region IToken Members

		public Position Position { get; private set; }

		#endregion

		#region IAstVisitable Members

		public void Accept(IAstVisitor visitor) {
			throw new System.NotImplementedException();
		}

		#endregion

		#region IExpressionMember Members

		public ExpressionMemberType MemberType { get { return ExpressionMemberType.Array; } }

		#endregion
	}
}

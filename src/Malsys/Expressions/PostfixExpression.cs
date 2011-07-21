using System.Collections.Generic;
using System.Linq;
using Malsys.Expressions;

namespace Malsys {
	/// <summary>
	/// Postfix expression is arithmetic expression in Reverse Polish notation (RPN).
	/// </summary>
	public class PostfixExpression : IExpressionValue {
		#region Static members

		public static readonly PostfixExpression Empty = new PostfixExpression() { members = new IPostfixExpressionMember[0] };
		public static readonly PostfixExpression[] EmptyArray = new PostfixExpression[0];

		#endregion

		#region Properties

		public object this[int i] { get { return members[i]; } }

		public int Count { get { return members.Length; } }

		public bool IsEmpty { get { return members.Length == 0; } }

		#endregion


		private IPostfixExpressionMember[] members;


		#region Constructors

		private PostfixExpression() { }

		public PostfixExpression(params IPostfixExpressionMember[] members) {
			this.members = members;
		}

		public PostfixExpression(IList<IPostfixExpressionMember> members) {
			this.members = members.ToArray();
		}

		#endregion

		internal IPostfixExpressionMember[] GetMembers() {
			return members;
		}

		#region IExpressionValue Members

		public bool IsExpression { get { return true; } }
		public bool IsArray { get { return false; } }

		#endregion
	}
}

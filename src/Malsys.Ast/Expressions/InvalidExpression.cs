
namespace Malsys.Ast {
	public class InvalidExpression : IToken, IExpressionMember {


		public InvalidExpression(Position pos) {
			Position = pos;
		}

		#region IExpressionMember Members

		public ExpressionMemberType MemberType {
			get { return ExpressionMemberType.Invalid; }
		}

		#endregion

		#region IToken Members

		public Position Position { get; private set; }

		#endregion

		#region IAstVisitable Members

		public void Accept(IAstVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}

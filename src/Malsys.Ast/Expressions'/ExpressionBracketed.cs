
namespace Malsys.Ast {
	public class ExpressionBracketed : IExpressionMember {

		public Expression Expression;

		public PositionRange Position { get; private set; }


		public ExpressionBracketed(Expression expression, PositionRange pos) {
			Expression = expression;
			Position = pos;
		}


		public ExpressionMemberType MemberType {
			get { return ExpressionMemberType.ExpressionBracketed; }
		}

	}
}

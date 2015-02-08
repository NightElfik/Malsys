
namespace Malsys.Ast {
	public class EmptyExpression : IExpressionMember {

		public PositionRange Position { get; private set; }


		public EmptyExpression(PositionRange pos) {
			Position = pos;
		}


		public ExpressionMemberType MemberType {
			get { return ExpressionMemberType.EmptyExpression; }
		}

	}
}

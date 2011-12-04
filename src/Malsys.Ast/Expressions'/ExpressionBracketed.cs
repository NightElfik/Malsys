
namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class ExpressionBracketed : IExpressionMember {

		public readonly Expression Expression;

		public ExpressionBracketed(Expression expression, Position pos) {
			Expression = expression;
			Position = pos;
		}


		public Position Position { get; private set; }


		public void Accept(IExpressionVisitor visitor) {
			visitor.Visit(this);
		}

	}
}

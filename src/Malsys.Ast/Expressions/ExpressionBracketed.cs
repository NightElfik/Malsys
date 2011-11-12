
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


		#region IToken Members

		public Position Position { get; private set; }

		#endregion

		#region IAstExpressionVisitable Members

		public void Accept(IAstExpressionVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}

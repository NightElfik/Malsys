
namespace Malsys.Ast {
	public class ExpressionBracketed : IToken, IAstVisitable, IExpressionMember {
		public readonly Expression Expression;

		public ExpressionBracketed(Expression expression, Position pos) {
			Expression = expression;
			Position = pos;
		}


		#region IToken Members

		public Position Position { get; private set; }

		#endregion

		#region IAstVisitable Members

		public void Accept(IAstVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion

		#region IExpressionMember Members

		public bool IsConstant { get { return false; } }
		public bool IsVariable { get { return false; } }
		public bool IsArray { get { return false; } }
		public bool IsOperator { get { return false; } }
		public bool IsFunction { get { return false; } }
		public bool IsIndexer { get { return false; } }
		public bool IsBracketedExpression { get { return true; } }

		#endregion
	}
}

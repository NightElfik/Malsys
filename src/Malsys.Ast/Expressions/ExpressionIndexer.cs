
namespace Malsys.Ast {
	public class ExpressionIndexer : IToken, IAstVisitable, IExpressionMember {
		public readonly Expression Index;

		public ExpressionIndexer(Expression index, Position pos) {
			Index = index;
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
		public bool IsIndexer { get { return true; } }
		public bool IsBracketedExpression { get { return false; } }

		#endregion
	}
}

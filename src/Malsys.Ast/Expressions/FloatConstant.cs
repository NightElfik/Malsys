
namespace Malsys.Ast {
	public class FloatConstant : IToken, IAstVisitable, IExpressionMember {
		public readonly double Value;

		public FloatConstant(double value, Position pos) {
			Value = value;
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

		public bool IsConstant { get { return true; } }
		public bool IsVariable { get { return false; } }
		public bool IsArray { get { return false; } }
		public bool IsOperator { get { return false; } }
		public bool IsFunction { get { return false; } }
		public bool IsIndexer { get { return false; } }
		public bool IsBracketedExpression { get { return false; } }

		#endregion
	}
}

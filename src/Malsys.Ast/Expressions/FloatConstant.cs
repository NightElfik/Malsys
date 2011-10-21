
namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class FloatConstant : IToken, IExpressionMember {

		public readonly double Value;


		public FloatConstant(double value, Position pos) {
			Value = value;
			Position = pos;
		}


		#region IToken Members

		public Position Position { get; private set; }

		#endregion

		#region IExpressionMember Members

		public ExpressionMemberType MemberType { get { return ExpressionMemberType.Constant; } }

		#endregion

		#region IAstVisitable Members

		public void Accept(IAstVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}

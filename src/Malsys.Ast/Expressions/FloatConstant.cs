
namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class FloatConstant : IToken, IExpressionMember {

		public readonly double Value;
		public readonly ConstantFormat Format;


		public FloatConstant(double value, ConstantFormat cf, Position pos) {
			Value = value;
			Format = cf;
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


namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class FloatConstant : IExpressionMember {

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

		#region IAstExpressionVisitable Members

		public void Accept(IAstExpressionVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}

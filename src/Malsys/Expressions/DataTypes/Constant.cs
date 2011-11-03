
namespace Malsys.Expressions {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class Constant : IExpression, IExpressionVisitable, IValue {

		public static readonly Constant NaN = new Constant(double.NaN);

		public static readonly Constant One = new Constant(1);
		public static readonly Constant Zero = new Constant(0);

		public static readonly Constant True = One;
		public static readonly Constant False = Zero;


		public bool IsInfinity { get { return double.IsInfinity(Value); } }

		public readonly double Value;

		public readonly Ast.FloatConstant AstNode;


		public Constant(double value) {
			Value = value;
			AstNode = null;
		}

		public Constant(double value, Ast.FloatConstant astNode) {
			Value = value;
			AstNode = astNode;
		}

		public static implicit operator double(Constant c) {
			return c.Value;
		}

		public override string ToString() {
			return Value.ToStringInvariant();
		}


		#region IExpression Members

		public IValue Evaluate(ArgsStorage args) {
			return this;  // Yay! Immutability rulezz!
		}

		#endregion

		#region IExpressionVisitable Members

		public void Accept(IExpressionVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion

		#region IValue Members

		public bool IsConstant { get { return true; } }
		public bool IsArray { get { return false; } }
		public bool IsNaN { get { return double.IsNaN(Value); } }
		public ExpressionValueType Type { get { return ExpressionValueType.Constant; } }

		#endregion

		#region IComparable<IArithmeticValue> Members

		public int CompareTo(IValue other) {
			if (other.IsConstant) {
				return Value.EpsilonCompareTo(((Constant)other).Value);
			}
			else {
				return -1; // constant is less than array
			}
		}

		#endregion
	}
}

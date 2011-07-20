
namespace Malsys.Expressions {
	public class Constant : IPostfixExpressionMember, IValue {
		public readonly double Value;

		public Constant(double value) {
			Value = value;
		}

		public static implicit operator double(Constant c) {
			return c.Value;
		}

		#region IPostfixExpressionMember Members

		public bool IsConstant { get { return true; } }
		public bool IsVariable { get { return false; } }
		public bool IsEvaluable { get { return false; } }

		#endregion

		#region IArithmeticValue Members

		public bool IsArray { get { return false; } }
		public IArithmeticValueType Type { get { return IArithmeticValueType.Constant; } }

		#endregion

		#region IComparable<IArithmeticValue> Members

		public int CompareTo(IValue other) {
			if (other.IsConstant) {
				return Value.CompareTo(((Constant)other).Value);
			}
			else {
				return -1; // constant is less than array
			}
		}

		#endregion
	}
}

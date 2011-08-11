﻿using System.Globalization;

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


		public Constant(double value) {
			Value = value;
		}

		public static implicit operator double(Constant c) {
			return c.Value;
		}

		public override string ToString() {
			return Value.ToString(CultureInfo.InvariantCulture.NumberFormat);
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
				Constant otherC = (Constant)other;

				if (FloatArithmeticHelper.IsZero(Value - otherC.Value)) {
					return 0;
				}
				else if (Value < otherC.Value) {
					return -1;
				}
				else {
					return 1;
				}
			}
			else {
				return -1; // constant is less than array
			}
		}

		#endregion
	}
}
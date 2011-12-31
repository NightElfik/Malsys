using System;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.SemanticModel {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class Constant : IExpression, IExpressionVisitable, IValue {

		public static readonly Constant NaN = new Constant(double.NaN);

		public static readonly Constant MinusOne = new Constant(-1d);
		public static readonly Constant Zero = new Constant(0d);
		public static readonly Constant One = new Constant(1d);

		public static readonly Constant True = One;
		public static readonly Constant False = Zero;



		public readonly double Value;

		public readonly Ast.FloatConstant AstNode;


		public Constant(double value, Ast.FloatConstant astNode = null) {
			Value = value;
			AstNode = astNode;
		}


		public bool IsZero { get { return Math.Abs(Value) < FloatArithmeticHelper.Epsilon; } }

		public bool IsNaN { get { return double.IsNaN(Value); } }

		public bool IsInfinity { get { return double.IsInfinity(Value); } }

		public bool IsConstant { get { return true; } }

		public bool IsArray { get { return false; } }

		public ExpressionValueType Type { get { return ExpressionValueType.Constant; } }

		public bool IsEmptyExpression { get { return false; } }

		public int RoundedIntValue { get { return (int)Math.Round(Value); } }

		public long RoundedLongValue { get { return (long)Math.Round(Value); } }

		public Position AstPosition { get { return AstNode == null ? Position.Unknown : AstNode.Position; } }


		public static implicit operator double(Constant c) {
			return c.Value;
		}


		public override string ToString() {
			return Value.ToStringInvariant();
		}


		public void Accept(IExpressionVisitor visitor) {
			visitor.Visit(this);
		}


		public int CompareTo(IValue other) {
			if (other.IsConstant) {
				return Value.EpsilonCompareTo(((Constant)other).Value);
			}
			else {
				return -1;  // constant is less than array
			}
		}

	}


	public static class ConstantExtensions {

		public static Constant ToConst(this int i) {
			return new Constant(i);
		}

		public static Constant ToConst(this long i) {
			return new Constant(i);
		}

		public static Constant ToConst(this double d) {
			return new Constant(d);
		}

		public static Constant ToConst(this double d, Ast.FloatConstant astConst) {
			return new Constant(d, astConst);
		}
	}

}

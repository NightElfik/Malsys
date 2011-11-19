using System;
using Malsys.Evaluators;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Compiled.Expressions;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.SemanticModel {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class Constant : IExpression, IExpressionVisitable, IValue {

		public static readonly Constant NaN = new Constant(double.NaN);

		public static readonly Constant One = new Constant(1);
		public static readonly Constant Zero = new Constant(0);

		public static readonly Constant True = One;
		public static readonly Constant False = Zero;


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


		public bool IsInfinity { get { return double.IsInfinity(Value); } }

		public int RoundedIntValue { get { return (int)Math.Round(Value); } }


		public static implicit operator double(Constant c) {
			return c.Value;
		}


		public override string ToString() {
			return Value.ToStringInvariant();
		}

		#region IExpression Members

		public bool IsEmptyExpression { get { return false; } }

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

/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.SemanticModel {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class Constant : IExpression, IValue {

		/// <summary>
		/// Constant with value NaN (not a number).
		/// </summary>
		public static readonly Constant NaN = new Constant(double.NaN);

		/// <summary>
		/// Constant with value -1.
		/// </summary>
		public static readonly Constant MinusOne = new Constant(-1d);

		/// <summary>
		/// Constant with value 0.
		/// </summary>
		public static readonly Constant Zero = new Constant(0d);

		/// <summary>
		/// Constant with value 1.
		/// </summary>
		public static readonly Constant One = new Constant(1d);

		/// <summary>
		/// Constant with value 1 representing true.
		/// </summary>
		public static readonly Constant True = One;
		/// <summary>
		/// Constant with value 0 representing false.
		/// </summary>
		public static readonly Constant False = Zero;



		public readonly double Value;

		public readonly Ast.FloatConstant AstNode;


		public Constant(double value, Ast.FloatConstant astNode = null) {
			Value = value;
			AstNode = astNode;
		}

		public Ast.ConstantFormat ConstantFormat {
			get {
				if (AstNode == null) {
					return Ast.ConstantFormat.Float;
				}
				return AstNode.Format;
			}
		}

		/// <summary>
		/// Returns true if value is not zero (with respect to epsilon).
		/// </summary>
		public bool IsTrue { get { return !(-FloatArithmeticHelper.Epsilon < Value && Value < FloatArithmeticHelper.Epsilon); } }

		/// <summary>
		/// Returns true if value is zero (with respect to epsilon).
		/// </summary>
		public bool IsZero { get { return -FloatArithmeticHelper.Epsilon < Value && Value < FloatArithmeticHelper.Epsilon; } }

		/// <summary>
		/// Returns true if value is NaN (not a number).
		/// </summary>
		public bool IsNaN { get { return double.IsNaN(Value); } }

		/// <summary>
		/// Returns true if value is infinity (positive or negative).
		/// </summary>
		public bool IsInfinity { get { return double.IsInfinity(Value); } }


		public bool IsConstant { get { return true; } }

		public bool IsArray { get { return false; } }

		public ExpressionType ExpressionType { get { return ExpressionType.Constant; } }

		public ExpressionValueType Type { get { return ExpressionValueType.Constant; } }

		public int RoundedIntValue { get { return (int)Math.Round(Value); } }

		public long RoundedLongValue { get { return (long)Math.Round(Value); } }

		public Position AstPosition { get { return AstNode == null ? Position.Unknown : AstNode.Position; } }


		public static implicit operator double(Constant c) {
			return c.Value;
		}


		public override string ToString() {
			return Value.ToStringInvariant();
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

		public static Constant ToConst(this float f) {
			return new Constant(f);
		}

		public static Constant ToConst(this double d) {
			return new Constant(d);
		}

		public static Constant ToConst(this double d, Ast.FloatConstant astConst) {
			return new Constant(d, astConst);
		}
	}

}

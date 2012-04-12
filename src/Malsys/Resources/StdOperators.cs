using System;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Resources {
	[MalsysOpertors]
	public static class StdOperators {

		const int POW_PREC = 100;
		const int POW_PREC_A = 100;

		const int UNARY_PREC = 150;
		const int UNARY_PREC_A = 50;  // 2^-3 = 2^(-3)

		const int MULT_PREC = 200;
		const int MULT_PREC_A = 220;

		const int ADD_PREC = 300;
		const int ADD_PREC_A = 320;

		const int CMP_PREC = 400;
		const int CMP_PREC_A = 420;

		const int EQ_PREC = 500;
		const int EQ_PREC_A = 520;

		const int AND_PREC = 500;
		const int AND_PREC_A = 520;

		const int XOR_PREC = 600;
		const int XOR_PREC_A = 620;

		const int OR_PREC = 700;
		const int OR_PREC_A = 720;


		/// <summary>
		/// Power operator.
		/// </summary>
		/// <remarks>
		/// Right associative -- it is not poped by itself.
		/// </remarks>
		public static readonly OperatorCore Power = new OperatorCore("^", POW_PREC, POW_PREC_A,
			ExpressionValueTypeFlags.Constant, ExpressionValueTypeFlags.Constant,
			(l, r) => Math.Pow((Constant)l, (Constant)r).ToConst());


		/// <summary>
		/// Unary plus operator — the most important operator ;)
		/// </summary>
		public static readonly OperatorCore Plus = new OperatorCore("+", UNARY_PREC, UNARY_PREC_A,
			ExpressionValueTypeFlags.Constant,
			(a) => a);

		/// <summary>
		/// Unary minus operator.
		/// </summary>
		public static readonly OperatorCore Minus = new OperatorCore("-", UNARY_PREC, UNARY_PREC_A,
			ExpressionValueTypeFlags.Constant,
			(a) => (-(Constant)a).ToConst());

		/// <summary>
		/// Unary not operator. Returns zero if applied on non-zero number, returns one if applied on zero.
		/// </summary>
		public static readonly OperatorCore Not = new OperatorCore("!", UNARY_PREC, UNARY_PREC_A,
			ExpressionValueTypeFlags.Constant,
			(a) => a.IsNaN
				? Constant.NaN
				: (FloatArithmeticHelper.IsZero((Constant)a) ? Constant.True : Constant.False));

		/// <summary>
		/// Multiply operator.
		/// </summary>
		public static readonly OperatorCore Multiply = new OperatorCore("*", MULT_PREC, MULT_PREC_A,
			ExpressionValueTypeFlags.Constant, ExpressionValueTypeFlags.Constant,
			(l, r) => ((Constant)l * (Constant)r).ToConst());

		/// <summary>
		/// Division operator.
		/// </summary>
		public static readonly OperatorCore Divide = new OperatorCore("/", MULT_PREC, MULT_PREC_A,
			ExpressionValueTypeFlags.Constant, ExpressionValueTypeFlags.Constant,
			(l, r) => ((Constant)l / (Constant)r).ToConst());

		/// <summary>
		/// Integer division operator. Divides given numbers and returns integer part of the result.
		/// </summary>
		public static readonly OperatorCore IntDivide = new OperatorCore("\\", MULT_PREC, MULT_PREC_A,
			ExpressionValueTypeFlags.Constant, ExpressionValueTypeFlags.Constant,
			(l, r) => Math.Floor((Constant)l / (Constant)r).ToConst());

		/// <summary>
		/// Module operator. Returns remainder of integer division.
		/// </summary>
		public static readonly OperatorCore Modulo = new OperatorCore("%", MULT_PREC, MULT_PREC_A,
			ExpressionValueTypeFlags.Constant, ExpressionValueTypeFlags.Constant,
			(l, r) => ((Constant)l % (Constant)r).ToConst());


		/// <summary>
		/// Add operator.
		/// </summary>
		public static readonly OperatorCore Add = new OperatorCore("+", ADD_PREC, ADD_PREC_A,
			ExpressionValueTypeFlags.Constant, ExpressionValueTypeFlags.Constant,
			(l, r) => ((Constant)l + (Constant)r).ToConst());

		/// <summary>
		/// Subtract operator.
		/// </summary>
		public static readonly OperatorCore Subtract = new OperatorCore("-", ADD_PREC, ADD_PREC_A,
			ExpressionValueTypeFlags.Constant, ExpressionValueTypeFlags.Constant,
			(l, r) => ((Constant)l - (Constant)r).ToConst());


		/// <summary>
		/// Less-than operator. Returns one if left operand is less than right operand, zero otherwise.
		/// </summary>
		public static readonly OperatorCore LessThan = new OperatorCore("<", CMP_PREC, CMP_PREC_A,
			ExpressionValueTypeFlags.Any, ExpressionValueTypeFlags.Any,
			(l, r) => (l.IsNaN || r.IsNaN)
				? Constant.NaN
				: (l.CompareTo(r) < 0 ? Constant.True : Constant.False));

		/// <summary>
		/// Greater-than operator. Returns one if left operand is greater than right operand, zero otherwise.
		/// </summary>
		public static readonly OperatorCore GreaterThan = new OperatorCore(">", CMP_PREC, CMP_PREC_A,
			ExpressionValueTypeFlags.Any, ExpressionValueTypeFlags.Any,
			(l, r) => (l.IsNaN || r.IsNaN)
				? Constant.NaN
				: (l.CompareTo(r) > 0 ? Constant.True : Constant.False));

		/// <summary>
		/// Less-than-or-equal operator. Returns one if left operand is less than or equal right operand, zero otherwise.
		/// </summary>
		public static readonly OperatorCore LessThanOrEqual = new OperatorCore("<=", CMP_PREC, CMP_PREC_A,
			ExpressionValueTypeFlags.Any, ExpressionValueTypeFlags.Any,
			(l, r) => (l.IsNaN || r.IsNaN)
				? Constant.NaN
				: (l.CompareTo(r) <= 0 ? Constant.True : Constant.False));

		/// <summary>
		/// Greater-than-or-equal operator. Returns one if left operand is greater than or equal right operand, zero otherwise.
		/// </summary>
		public static readonly OperatorCore GreaterThanOrEqual = new OperatorCore(">=", CMP_PREC, CMP_PREC_A,
			ExpressionValueTypeFlags.Any, ExpressionValueTypeFlags.Any,
			(l, r) => (l.IsNaN || r.IsNaN)
				? Constant.NaN
				: (l.CompareTo(r) >= 0 ? Constant.True : Constant.False));

		/// <summary>
		/// Equals operator. Returns one if left and right operands are equal, zero otherwise.
		/// </summary>
		public static readonly OperatorCore Equal = new OperatorCore("==", EQ_PREC, EQ_PREC_A,
			ExpressionValueTypeFlags.Any, ExpressionValueTypeFlags.Any,
			(l, r) => (l.IsNaN || r.IsNaN)
				? Constant.NaN
				: (l.CompareTo(r) == 0 ? Constant.True : Constant.False));

		/// <summary>
		/// Equals operator. Returns zero if left and right operands are equal, one otherwise.
		/// </summary>
		public static readonly OperatorCore NotEqual = new OperatorCore("!=", EQ_PREC, EQ_PREC_A,
			ExpressionValueTypeFlags.Any, ExpressionValueTypeFlags.Any,
			(l, r) => (l.IsNaN || r.IsNaN)
				? Constant.NaN
				: (l.CompareTo(r) == 0 ? Constant.False : Constant.True));


		/// <summary>
		/// Logical-and operator. Returns one if both left and right operands non-zero, zero otherwise.
		/// </summary>
		public static readonly OperatorCore And = new OperatorCore("&&", AND_PREC, AND_PREC_A,
			ExpressionValueTypeFlags.Constant, ExpressionValueTypeFlags.Constant,
			(l, r) => (l.IsNaN || r.IsNaN)
				? Constant.NaN
				: ((FloatArithmeticHelper.IsZero((Constant)l) || FloatArithmeticHelper.IsZero((Constant)r)) ? Constant.False : Constant.True));


		/// <summary>
		/// Logical-or operator. Returns one if just one of operands is non-zero, zero otherwise.
		/// </summary>
		public static readonly OperatorCore Xor = new OperatorCore("^^", XOR_PREC, XOR_PREC_A,
			ExpressionValueTypeFlags.Constant, ExpressionValueTypeFlags.Constant,
			(l, r) => (l.IsNaN || r.IsNaN)
				? Constant.NaN
				: ((FloatArithmeticHelper.IsZero((Constant)l) == FloatArithmeticHelper.IsZero((Constant)r)) ? Constant.False : Constant.True));


		/// <summary>
		/// Logical-or operator. Returns one if at least one of operands are non-zero, zero otherwise.
		/// </summary>
		public static readonly OperatorCore Or = new OperatorCore("||", OR_PREC, OR_PREC_A,
			ExpressionValueTypeFlags.Constant, ExpressionValueTypeFlags.Constant,
			(l, r) => (l.IsNaN || r.IsNaN)
				? Constant.NaN
				: ((FloatArithmeticHelper.IsZero((Constant)l) && FloatArithmeticHelper.IsZero((Constant)r)) ? Constant.False : Constant.True));


	}
}

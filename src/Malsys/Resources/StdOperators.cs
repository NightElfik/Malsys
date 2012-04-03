using System;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Resources {
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
		/// Unary plus -- the most important operator ;)
		/// </summary>
		public static readonly OperatorCore Plus = new OperatorCore("+", UNARY_PREC, UNARY_PREC_A,
			ExpressionValueTypeFlags.Constant,
			(a) => a);

		public static readonly OperatorCore Minus = new OperatorCore("-", UNARY_PREC, UNARY_PREC_A,
			ExpressionValueTypeFlags.Constant,
			(a) => (-(Constant)a).ToConst());

		public static readonly OperatorCore Not = new OperatorCore("!", UNARY_PREC, UNARY_PREC_A,
			ExpressionValueTypeFlags.Constant,
			(a) => a.IsNaN
				? Constant.NaN
				: (FloatArithmeticHelper.IsZero((Constant)a) ? Constant.True : Constant.False));


		public static readonly OperatorCore Multiply = new OperatorCore("*", MULT_PREC, MULT_PREC_A,
			ExpressionValueTypeFlags.Constant, ExpressionValueTypeFlags.Constant,
			(l, r) => ((Constant)l * (Constant)r).ToConst());

		public static readonly OperatorCore Divide = new OperatorCore("/", MULT_PREC, MULT_PREC_A,
			ExpressionValueTypeFlags.Constant, ExpressionValueTypeFlags.Constant,
			(l, r) => ((Constant)l / (Constant)r).ToConst());

		public static readonly OperatorCore IntDivide = new OperatorCore("\\", MULT_PREC, MULT_PREC_A,
			ExpressionValueTypeFlags.Constant, ExpressionValueTypeFlags.Constant,
			(l, r) => Math.Floor((Constant)l / (Constant)r).ToConst());

		public static readonly OperatorCore Modulo = new OperatorCore("%", MULT_PREC, MULT_PREC_A,
			ExpressionValueTypeFlags.Constant, ExpressionValueTypeFlags.Constant,
			(l, r) => ((Constant)l % (Constant)r).ToConst());


		public static readonly OperatorCore Add = new OperatorCore("+", ADD_PREC, ADD_PREC_A,
			ExpressionValueTypeFlags.Constant, ExpressionValueTypeFlags.Constant,
			(l, r) => ((Constant)l + (Constant)r).ToConst());

		public static readonly OperatorCore Subtract = new OperatorCore("-", ADD_PREC, ADD_PREC_A,
			ExpressionValueTypeFlags.Constant, ExpressionValueTypeFlags.Constant,
			(l, r) => ((Constant)l - (Constant)r).ToConst());


		public static readonly OperatorCore LessThan = new OperatorCore("<", CMP_PREC, CMP_PREC_A,
			ExpressionValueTypeFlags.Any, ExpressionValueTypeFlags.Any,
			(l, r) => (l.IsNaN || r.IsNaN)
				? Constant.NaN
				: (l.CompareTo(r) < 0 ? Constant.True : Constant.False));

		public static readonly OperatorCore GreaterThan = new OperatorCore(">", CMP_PREC, CMP_PREC_A,
			ExpressionValueTypeFlags.Any, ExpressionValueTypeFlags.Any,
			(l, r) => (l.IsNaN || r.IsNaN)
				? Constant.NaN
				: (l.CompareTo(r) > 0 ? Constant.True : Constant.False));

		public static readonly OperatorCore LessThanOrEqual = new OperatorCore("<=", CMP_PREC, CMP_PREC_A,
			ExpressionValueTypeFlags.Any, ExpressionValueTypeFlags.Any,
			(l, r) => (l.IsNaN || r.IsNaN)
				? Constant.NaN
				: (l.CompareTo(r) <= 0 ? Constant.True : Constant.False));

		public static readonly OperatorCore GreaterThanOrEqual = new OperatorCore(">=", CMP_PREC, CMP_PREC_A,
			ExpressionValueTypeFlags.Any, ExpressionValueTypeFlags.Any,
			(l, r) => (l.IsNaN || r.IsNaN)
				? Constant.NaN
				: (l.CompareTo(r) >= 0 ? Constant.True : Constant.False));


		public static readonly OperatorCore Equal = new OperatorCore("==", EQ_PREC, EQ_PREC_A,
			ExpressionValueTypeFlags.Any, ExpressionValueTypeFlags.Any,
			(l, r) => (l.IsNaN || r.IsNaN)
				? Constant.NaN
				: (l.CompareTo(r) == 0 ? Constant.True : Constant.False));

		public static readonly OperatorCore NotEqual = new OperatorCore("!=", EQ_PREC, EQ_PREC_A,
			ExpressionValueTypeFlags.Any, ExpressionValueTypeFlags.Any,
			(l, r) => (l.IsNaN || r.IsNaN)
				? Constant.NaN
				: (l.CompareTo(r) == 0 ? Constant.False : Constant.True));


		public static readonly OperatorCore And = new OperatorCore("&&", AND_PREC, AND_PREC_A,
			ExpressionValueTypeFlags.Constant, ExpressionValueTypeFlags.Constant,
			(l, r) => (l.IsNaN || r.IsNaN)
				? Constant.NaN
				: ((FloatArithmeticHelper.IsZero((Constant)l) || FloatArithmeticHelper.IsZero((Constant)r)) ? Constant.False : Constant.True));


		public static readonly OperatorCore Xor = new OperatorCore("^^", XOR_PREC, XOR_PREC_A,
			ExpressionValueTypeFlags.Constant, ExpressionValueTypeFlags.Constant,
			(l, r) => (l.IsNaN || r.IsNaN)
				? Constant.NaN
				: ((FloatArithmeticHelper.IsZero((Constant)l) == FloatArithmeticHelper.IsZero((Constant)r)) ? Constant.False : Constant.True));


		public static readonly OperatorCore Or = new OperatorCore("||", OR_PREC, OR_PREC_A,
			ExpressionValueTypeFlags.Constant, ExpressionValueTypeFlags.Constant,
			(l, r) => (l.IsNaN || r.IsNaN)
				? Constant.NaN
				: ((FloatArithmeticHelper.IsZero((Constant)l) && FloatArithmeticHelper.IsZero((Constant)r)) ? Constant.False : Constant.True));


	}
}

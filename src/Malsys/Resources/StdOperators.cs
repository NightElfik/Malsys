using System;
using Malsys.Compilers.Expressions;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Resources {
	public static class StdOperators {


		/// <summary>
		/// Power operator (right associative -- it is not poped by itself).
		/// </summary>
		public static readonly OperatorCore Power = new OperatorCore("^", 3, 3,
			new ExpressionValueTypeFlags[] { ExpressionValueTypeFlags.Constant, ExpressionValueTypeFlags.Constant },
			(a) => {
				return Math.Pow((Constant)a[0], (Constant)a[1]).ToConst();
			});

		/// <summary>
		/// Unary plus -- the most important operator ;)
		/// </summary>
		public static readonly OperatorCore Plus = new OperatorCore("+", 4, 2,
			new ExpressionValueTypeFlags[] { ExpressionValueTypeFlags.Constant },
			(a) => {
				return a[0];
			});
		public static readonly OperatorCore Minus = new OperatorCore("-", 4, 2,
			new ExpressionValueTypeFlags[] { ExpressionValueTypeFlags.Constant },
			(a) => {
				return (-(Constant)a[0]).ToConst();
			});
		public static readonly OperatorCore Not = new OperatorCore("!", 4, 2,
			new ExpressionValueTypeFlags[] { ExpressionValueTypeFlags.Constant },
			(a) => {
				if (a[0].IsNaN) { return Constant.NaN; }
				return FloatArithmeticHelper.IsZero((Constant)a[0]) ? Constant.True : Constant.False;
			});

		public static readonly OperatorCore Multiply = new OperatorCore("*", 5, 6,
			new ExpressionValueTypeFlags[] { ExpressionValueTypeFlags.Constant, ExpressionValueTypeFlags.Constant },
			(a) => {
				return ((Constant)a[0] * (Constant)a[1]).ToConst();
			});
		public static readonly OperatorCore Divide = new OperatorCore("/", 5, 6,
			new ExpressionValueTypeFlags[] { ExpressionValueTypeFlags.Constant, ExpressionValueTypeFlags.Constant },
			(a) => {
				return ((Constant)a[0] / (Constant)a[1]).ToConst();
			});
		public static readonly OperatorCore IntDivide = new OperatorCore("\\", 5, 6,
			new ExpressionValueTypeFlags[] { ExpressionValueTypeFlags.Constant, ExpressionValueTypeFlags.Constant },
			(a) => {
				return Math.Floor((Constant)a[0] / (Constant)a[1]).ToConst();
			});
		public static readonly OperatorCore Modulo = new OperatorCore("%", 5, 6,
			new ExpressionValueTypeFlags[] { ExpressionValueTypeFlags.Constant, ExpressionValueTypeFlags.Constant },
			(a) => {
				return ((Constant)a[0] % (Constant)a[1]).ToConst();
			});

		public static readonly OperatorCore Add = new OperatorCore("+", 7, 8,
			new ExpressionValueTypeFlags[] { ExpressionValueTypeFlags.Constant, ExpressionValueTypeFlags.Constant },
			(a) => {
				return ((Constant)a[0] + (Constant)a[1]).ToConst();
			});
		public static readonly OperatorCore Subtract = new OperatorCore("-", 7, 8,
			new ExpressionValueTypeFlags[] { ExpressionValueTypeFlags.Constant, ExpressionValueTypeFlags.Constant },
			(a) => {
				return ((Constant)a[0] - (Constant)a[1]).ToConst();
			});

		public static readonly OperatorCore LessThan = new OperatorCore("<", 9, 10,
			new ExpressionValueTypeFlags[] { ExpressionValueTypeFlags.Any, ExpressionValueTypeFlags.Any },
			(a) => {
				if (a[0].IsNaN || a[1].IsNaN) { return Constant.NaN; }
				return a[0].CompareTo(a[1]) < 0 ? Constant.True : Constant.False;
			});
		public static readonly OperatorCore GreaterThan = new OperatorCore(">", 9, 10,
			new ExpressionValueTypeFlags[] { ExpressionValueTypeFlags.Any, ExpressionValueTypeFlags.Any },
			(a) => {
				if (a[0].IsNaN || a[1].IsNaN) { return Constant.NaN; }
				return a[0].CompareTo(a[1]) > 0 ? Constant.True : Constant.False;
			});
		public static readonly OperatorCore LessThanOrEqual = new OperatorCore("<=", 9, 10,
			new ExpressionValueTypeFlags[] { ExpressionValueTypeFlags.Any, ExpressionValueTypeFlags.Any },
			(a) => {
				if (a[0].IsNaN || a[1].IsNaN) { return Constant.NaN; }
				return a[0].CompareTo(a[1]) <= 0 ? Constant.True : Constant.False;
			});
		public static readonly OperatorCore GreaterThanOrEqual = new OperatorCore(">=", 9, 10,
			new ExpressionValueTypeFlags[] { ExpressionValueTypeFlags.Any, ExpressionValueTypeFlags.Any },
			(a) => {
				if (a[0].IsNaN || a[1].IsNaN) { return Constant.NaN; }
				return a[0].CompareTo(a[1]) >= 0 ? Constant.True : Constant.False;
			});

		public static readonly OperatorCore Equal = new OperatorCore("==", 11, 12,
			new ExpressionValueTypeFlags[] { ExpressionValueTypeFlags.Any, ExpressionValueTypeFlags.Any },
			(a) => {
				if (a[0].IsNaN || a[1].IsNaN) { return Constant.NaN; }
				return a[0].CompareTo(a[1]) == 0 ? Constant.True : Constant.False;
			});

		public static readonly OperatorCore And = new OperatorCore("&&", 13, 14,
			new ExpressionValueTypeFlags[] { ExpressionValueTypeFlags.Constant, ExpressionValueTypeFlags.Constant },
			(a) => {
				if (a[0].IsNaN || a[1].IsNaN) { return Constant.NaN; }
				return (FloatArithmeticHelper.IsZero((Constant)a[0]) || FloatArithmeticHelper.IsZero((Constant)a[1])) ? Constant.False : Constant.True;
			});

		public static readonly OperatorCore Xor = new OperatorCore("^^", 15, 16,
			new ExpressionValueTypeFlags[] { ExpressionValueTypeFlags.Constant, ExpressionValueTypeFlags.Constant },
			(a) => {
				if (a[0].IsNaN || a[1].IsNaN) { return Constant.NaN; }
				return (FloatArithmeticHelper.IsZero((Constant)a[0]) == FloatArithmeticHelper.IsZero((Constant)a[1])) ? Constant.False : Constant.True;
			});

		public static readonly OperatorCore Or = new OperatorCore("||", 17, 18,
			new ExpressionValueTypeFlags[] { ExpressionValueTypeFlags.Constant, ExpressionValueTypeFlags.Constant },
			(a) => {
				if (a[0].IsNaN || a[1].IsNaN) { return Constant.NaN; }
				return (FloatArithmeticHelper.IsZero((Constant)a[0]) && FloatArithmeticHelper.IsZero((Constant)a[1])) ? Constant.False : Constant.True;
			});


	}
}

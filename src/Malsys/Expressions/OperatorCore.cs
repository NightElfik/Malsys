using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Key = System.Tuple<string, int>;

namespace Malsys.Expressions {
	class OperatorCore {
		#region Static members

		#region Operator definitions

		/// <summary>
		/// Power operator (right associative -- it is not poped by itself).
		/// </summary>
		public static readonly OperatorCore Power = new OperatorCore("^", 3, 3,
			new ExpressionValueType[] { ExpressionValueType.Constant, ExpressionValueType.Constant },
			(a) => {
				return Math.Pow((Constant)a[0], (Constant)a[1]).ToConst();
			});

		/// <summary>
		/// Unary plus -- the most important operator ;)
		/// </summary>
		public static readonly OperatorCore Plus = new OperatorCore("+", 4, 2,
			new ExpressionValueType[] { ExpressionValueType.Constant },
			(a) => {
				return a[0];
			});
		public static readonly OperatorCore Minus = new OperatorCore("-", 4, 2,
			new ExpressionValueType[] { ExpressionValueType.Constant },
			(a) => {
				return (-(Constant)a[0]).ToConst();
			});
		public static readonly OperatorCore Not = new OperatorCore("!", 4, 2,
			new ExpressionValueType[] { ExpressionValueType.Constant },
			(a) => {
				return (FloatArithmeticHelper.IsZero((Constant)a[0]) ? 1.0 : 0.0).ToConst();
			});

		public static readonly OperatorCore Multiply = new OperatorCore("*", 5, 6,
			new ExpressionValueType[] { ExpressionValueType.Constant, ExpressionValueType.Constant },
			(a) => {
				return ((Constant)a[0] * (Constant)a[1]).ToConst();
			});
		public static readonly OperatorCore Divide = new OperatorCore("/", 5, 6,
			new ExpressionValueType[] { ExpressionValueType.Constant, ExpressionValueType.Constant },
			(a) => {
				return ((Constant)a[0] / (Constant)a[1]).ToConst();
			});
		public static readonly OperatorCore IntDivide = new OperatorCore("\\", 5, 6,
			new ExpressionValueType[] { ExpressionValueType.Constant, ExpressionValueType.Constant },
			(a) => {
				return Math.Floor((Constant)a[0] / (Constant)a[1]).ToConst();
			});
		public static readonly OperatorCore Modulo = new OperatorCore("%", 5, 6,
			new ExpressionValueType[] { ExpressionValueType.Constant, ExpressionValueType.Constant },
			(a) => {
				return ((Constant)a[0] % (Constant)a[1]).ToConst();
			});

		public static readonly OperatorCore Add = new OperatorCore("+", 7, 8,
			new ExpressionValueType[] { ExpressionValueType.Constant, ExpressionValueType.Constant },
			(a) => {
				return ((Constant)a[0] + (Constant)a[1]).ToConst();
			});
		public static readonly OperatorCore Subtract = new OperatorCore("-", 7, 8,
			new ExpressionValueType[] { ExpressionValueType.Constant, ExpressionValueType.Constant },
			(a) => {
				return ((Constant)a[0] - (Constant)a[1]).ToConst();
			});

		public static readonly OperatorCore LessThan = new OperatorCore("<", 9, 10,
			new ExpressionValueType[] { ExpressionValueType.Any, ExpressionValueType.Any },
			(a) => {
				return (a[0].CompareTo(a[1]) < 0 ? 1.0 : 0.0).ToConst();
			});
		public static readonly OperatorCore GreaterThan = new OperatorCore(">", 9, 10,
			new ExpressionValueType[] { ExpressionValueType.Any, ExpressionValueType.Any },
			(a) => {
				return (a[0].CompareTo(a[1]) > 0 ? 1.0 : 0.0).ToConst();
			});
		public static readonly OperatorCore LessThanOrEqual = new OperatorCore("<=", 9, 10,
			new ExpressionValueType[] { ExpressionValueType.Any, ExpressionValueType.Any },
			(a) => {
				return (a[0].CompareTo(a[1]) <= 0 ? 1.0 : 0.0).ToConst();
			});
		public static readonly OperatorCore GreaterThanOrEqual = new OperatorCore(">=", 9, 10,
			new ExpressionValueType[] { ExpressionValueType.Any, ExpressionValueType.Any },
			(a) => {
				return (a[0].CompareTo(a[1]) >= 0 ? 1.0 : 0.0).ToConst();
			});

		public static readonly OperatorCore Equal = new OperatorCore("==", 11, 12,
			new ExpressionValueType[] { ExpressionValueType.Any, ExpressionValueType.Any },
			(a) => {
				return (a[0].CompareTo(a[1]) == 0 ? 1.0 : 0.0).ToConst();
			});

		public static readonly OperatorCore And = new OperatorCore("&&", 13, 14,
			new ExpressionValueType[] { ExpressionValueType.Constant, ExpressionValueType.Constant },
			(a) => {
				return ((FloatArithmeticHelper.IsZero((Constant)a[0]) || FloatArithmeticHelper.IsZero((Constant)a[1])) ? 0.0 : 1.0).ToConst();
			});

		public static readonly OperatorCore Xor = new OperatorCore("^^", 15, 16,
			new ExpressionValueType[] { ExpressionValueType.Constant, ExpressionValueType.Constant },
			(a) => {
				return ((FloatArithmeticHelper.IsZero((Constant)a[0]) == FloatArithmeticHelper.IsZero((Constant)a[1])) ? 0.0 : 1.0).ToConst();
			});

		public static readonly OperatorCore Or = new OperatorCore("||", 17, 18,
			new ExpressionValueType[] { ExpressionValueType.Constant, ExpressionValueType.Constant },
			(a) => {
				return ((FloatArithmeticHelper.IsZero((Constant)a[0]) && FloatArithmeticHelper.IsZero((Constant)a[1])) ? 0.0 : 1.0).ToConst();
			});

		#endregion


		static Dictionary<Key, OperatorCore> opCache;

		/// <summary>
		/// Builds operators cache from definitions in this class.
		/// </summary>
		static OperatorCore() {
			opCache = new Dictionary<Key, OperatorCore>();

			foreach (FieldInfo fi in typeof(OperatorCore).GetFields(BindingFlags.Public | BindingFlags.Static)) {
				if (!fi.FieldType.Equals(typeof(OperatorCore))) {
					continue;
				}

				OperatorCore op = (OperatorCore)fi.GetValue(null);
				Key key = new Key(op.Syntax.ToLowerInvariant(), op.Arity);

				Debug.Assert(!opCache.ContainsKey(key),
					"Syntax-arity pair `{0}` [{1}] of known operator is not unique.".Fmt(key.Item1, op.Arity));

				opCache.Add(key, op);
			}
		}

		/// <summary>
		/// Tries to get operator with given arity and syntax equal to given string.
		/// </summary>
		public static bool TryGet(string syntax, byte arity, out OperatorCore result) {
			return opCache.TryGetValue(new Key(syntax, arity), out result);
		}

		#endregion

		public readonly string Syntax;
		/// <summary>
		/// Arity -- how many arguments operator needs.
		/// Only unary and binary operators are supported.
		/// </summary>
		public readonly int Arity;

		/// <summary>
		/// Normal precedence.
		/// </summary>
		/// <remarks>
		/// If precedence and active precedence are the same, operator is right associative.
		/// If active precedence is higher, operator is left associative.
		/// </remarks>
		public readonly byte Precedence;
		/// <summary>
		/// Precedence while I am holding this operator and deciding weather i push it on stack or pop some others before push.
		/// Idea by Martin Mareš :)
		/// </summary>
		public readonly byte ActivePrecedence;

		/// <summary>
		/// Types of parameters of operator.
		/// Length of the array is equal to operator's arity.
		/// </summary>
		public readonly ImmutableList<ExpressionValueType> ParamsTypes;

		public readonly EvalDelegate EvalFunction;


		private OperatorCore(string syntax, byte prec, byte activePrec, ExpressionValueType[] paramsTypes, EvalDelegate evalFunc) {
			Syntax = syntax;
			Arity = paramsTypes.Length;
			Debug.Assert(Arity == 1 || Arity == 2, "Only unary and binary operators are supported.");
			EvalFunction = evalFunc;
			Precedence = prec;
			ParamsTypes = new ImmutableList<ExpressionValueType>(paramsTypes, true);
			ActivePrecedence = activePrec;
		}
	}
}

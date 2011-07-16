using System;
using System.Reflection;
using System.Collections.Generic;

namespace Malsys.Expressions {
	/// <summary>
	/// Immutable arithmetic operator.
	/// </summary>
	public class ArithmeticOperator : ArithmeticFunction {
		#region Static members

		#region Operator definitions

		public static readonly ArithmeticOperator Sqrt = new ArithmeticOperator(CharHelper.Sqrt.ToString(), 1, 2, 3, (a) => { return Math.Sqrt(a[0]); });

		/// <summary>
		/// Power operator (right associative -- it is not poped by itself).
		/// </summary>
		public static readonly ArithmeticOperator Power = new ArithmeticOperator("^", 2, 3, 3, (a) => { return Math.Pow(a[0], a[1]); });

		/// <summary>
		/// Unary plus -- the most important operator ;)
		/// </summary>
		public static readonly ArithmeticOperator Plus = new ArithmeticOperator("+", 1, 4, 2,
			(a) => { return a[0]; });
		public static readonly ArithmeticOperator Minus = new ArithmeticOperator("-", 1, 4, 2, 
			(a) => { return -a[0]; });
		public static readonly ArithmeticOperator Not = new ArithmeticOperator("!", 1, 4, 2,
			(a) => { return FloatArithmeticHelper.IsZero(a[0]) ? 1.0 : 0.0; });

		public static readonly ArithmeticOperator Multiply = new ArithmeticOperator("*", 2, 5, 6,
			(a) => { return a[0] * a[1]; });
		public static readonly ArithmeticOperator Divide = new ArithmeticOperator("/", 2, 5, 6,
			(a) => { return a[0] / a[1]; });
		public static readonly ArithmeticOperator IntDivide = new ArithmeticOperator("\\", 2, 5, 6,
			(a) => { return (double)((long)a[0] / (long)a[1]); });
		public static readonly ArithmeticOperator Modulo = new ArithmeticOperator("%", 2, 5, 6,
			(a) => { return a[0] % a[1]; });

		public static readonly ArithmeticOperator Add = new ArithmeticOperator("+", 2, 7, 8,
			(a) => { return a[0] + a[1]; });
		public static readonly ArithmeticOperator Subtract = new ArithmeticOperator("-", 2, 7, 8,
			(a) => { return a[0] - a[1]; });

		public static readonly ArithmeticOperator LessThan = new ArithmeticOperator("<", 2, 9, 10,
			(a) => { return a[0] < a[1] ? 1.0 : 0.0; });
		public static readonly ArithmeticOperator GreaterThan = new ArithmeticOperator(">", 2, 9, 10,
			(a) => { return a[0] > a[1] ? 1.0 : 0.0; });
		public static readonly ArithmeticOperator LessThanOrEqual = new ArithmeticOperator("<=", 2, 9, 10,
			(a) => { return a[0] <= a[1] ? 1.0 : 0.0; });
		public static readonly ArithmeticOperator GreaterThanOrEqual = new ArithmeticOperator(">=", 2, 9, 10,
			(a) => { return a[0] >= a[1] ? 1.0 : 0.0; });

		public static readonly ArithmeticOperator Equal = new ArithmeticOperator("==", 2, 11, 12,
			(a) => { return FloatArithmeticHelper.IsZero(a[0] - a[1]) ? 1.0 : 0.0; });
		
		public static readonly ArithmeticOperator And = new ArithmeticOperator("&&", 2, 13, 14,
			(a) => { return (FloatArithmeticHelper.IsZero(a[0]) || FloatArithmeticHelper.IsZero(a[1])) ? 0.0 : 1.0; });

		public static readonly ArithmeticOperator Xor = new ArithmeticOperator("^^", 2, 13, 14,
			(a) => { return (FloatArithmeticHelper.IsZero(a[0]) == FloatArithmeticHelper.IsZero(a[1])) ? 0.0 : 1.0; });

		public static readonly ArithmeticOperator Or = new ArithmeticOperator("||", 2, 17, 18,
			(a) => { return (FloatArithmeticHelper.IsZero(a[0]) && FloatArithmeticHelper.IsZero(a[1])) ? 0.0 : 1.0; });
		
		#endregion

		static Dictionary<string, ArithmeticOperator>[] opCache; 

		static ArithmeticOperator() {
			opCache = new Dictionary<string, ArithmeticOperator>[2];
			for (int i = 0; i < opCache.Length; i++) {
				opCache[i] = new Dictionary<string, ArithmeticOperator>();
			}

			foreach (FieldInfo fi in typeof(ArithmeticOperator).GetFields(BindingFlags.Public | BindingFlags.Static)) {
				if (!fi.FieldType.Equals(typeof(ArithmeticOperator))) {
					continue;
				}

				ArithmeticOperator op = (ArithmeticOperator)fi.GetValue(null);
#if DEBUG
				if (op.Arity == 0 || op.Arity > opCache.Length) {
					throw new InvalidOperationException("Failed to initialize operator cahce. Invalid operator `{0}`.".Fmt(op.ToString()));
				}
#endif
				opCache[op.Arity - 1].Add(op.Syntax, op);
			}
		}

		/// <summary>
		/// Tries to parse given string and arity as operator.
		/// </summary>
		public static bool TryParse(string str, byte arity, out ArithmeticOperator result) {
			if (arity == 0 || arity > opCache.Length) {
				result = null;
				return false;
			}

			return opCache[arity].TryGetValue(str, out result);
		}

		#endregion

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

		public ArithmeticOperator(string syntax, byte arity, byte prec, byte activePrec, EvaluateDelegate evalFunc)
			: base(syntax, arity, evalFunc) {

			Precedence = prec;
			ActivePrecedence = activePrec;
		}
	}
}

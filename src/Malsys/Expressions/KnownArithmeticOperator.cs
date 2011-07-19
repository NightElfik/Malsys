using System;
using System.Collections.Generic;
using System.Reflection;
using Key = System.Tuple<string, byte>;
using System.Diagnostics;

namespace Malsys.Expressions {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class KnownArithmeticOperator : IEvaluable {
		#region Static members

		#region Operator definitions

		/// <summary>
		/// Power operator (right associative -- it is not poped by itself).
		/// </summary>
		public static readonly KnownArithmeticOperator Power = new KnownArithmeticOperator("^", 2, 3, 3,
			(a) => { return Math.Pow(a[0], a[1]); });

		/// <summary>
		/// Unary plus -- the most important operator ;)
		/// </summary>
		public static readonly KnownArithmeticOperator Plus = new KnownArithmeticOperator("+", 1, 4, 2,
			(a) => { return a[0]; });
		public static readonly KnownArithmeticOperator Minus = new KnownArithmeticOperator("-", 1, 4, 2, 
			(a) => { return -a[0]; });
		public static readonly KnownArithmeticOperator Not = new KnownArithmeticOperator("!", 1, 4, 2,
			(a) => { return FloatArithmeticHelper.IsZero(a[0]) ? 1.0 : 0.0; });

		public static readonly KnownArithmeticOperator Multiply = new KnownArithmeticOperator("*", 2, 5, 6,
			(a) => { return a[0] * a[1]; });
		public static readonly KnownArithmeticOperator Divide = new KnownArithmeticOperator("/", 2, 5, 6,
			(a) => { return a[0] / a[1]; });
		public static readonly KnownArithmeticOperator IntDivide = new KnownArithmeticOperator("\\", 2, 5, 6,
			(a) => { return (double)((long)a[0] / (long)a[1]); });
		public static readonly KnownArithmeticOperator Modulo = new KnownArithmeticOperator("%", 2, 5, 6,
			(a) => { return a[0] % a[1]; });

		public static readonly KnownArithmeticOperator Add = new KnownArithmeticOperator("+", 2, 7, 8,
			(a) => { return a[0] + a[1]; });
		public static readonly KnownArithmeticOperator Subtract = new KnownArithmeticOperator("-", 2, 7, 8,
			(a) => { return a[0] - a[1]; });

		public static readonly KnownArithmeticOperator LessThan = new KnownArithmeticOperator("<", 2, 9, 10,
			(a) => { return a[0] < a[1] ? 1.0 : 0.0; });
		public static readonly KnownArithmeticOperator GreaterThan = new KnownArithmeticOperator(">", 2, 9, 10,
			(a) => { return a[0] > a[1] ? 1.0 : 0.0; });
		public static readonly KnownArithmeticOperator LessThanOrEqual = new KnownArithmeticOperator("<=", 2, 9, 10,
			(a) => { return a[0] <= a[1] ? 1.0 : 0.0; });
		public static readonly KnownArithmeticOperator GreaterThanOrEqual = new KnownArithmeticOperator(">=", 2, 9, 10,
			(a) => { return a[0] >= a[1] ? 1.0 : 0.0; });

		public static readonly KnownArithmeticOperator Equal = new KnownArithmeticOperator("==", 2, 11, 12,
			(a) => { return FloatArithmeticHelper.IsZero(a[0] - a[1]) ? 1.0 : 0.0; });
		
		public static readonly KnownArithmeticOperator And = new KnownArithmeticOperator("&&", 2, 13, 14,
			(a) => { return (FloatArithmeticHelper.IsZero(a[0]) || FloatArithmeticHelper.IsZero(a[1])) ? 0.0 : 1.0; });

		public static readonly KnownArithmeticOperator Xor = new KnownArithmeticOperator("^^", 2, 13, 14,
			(a) => { return (FloatArithmeticHelper.IsZero(a[0]) == FloatArithmeticHelper.IsZero(a[1])) ? 0.0 : 1.0; });

		public static readonly KnownArithmeticOperator Or = new KnownArithmeticOperator("||", 2, 17, 18,
			(a) => { return (FloatArithmeticHelper.IsZero(a[0]) && FloatArithmeticHelper.IsZero(a[1])) ? 0.0 : 1.0; });
		
		#endregion


		static Dictionary<Key, KnownArithmeticOperator> opCache; 

		/// <summary>
		/// Builds operators cache.
		/// </summary>
		static KnownArithmeticOperator() {
			opCache = new Dictionary<Key, KnownArithmeticOperator>();

			foreach (FieldInfo fi in typeof(KnownArithmeticOperator).GetFields(BindingFlags.Public | BindingFlags.Static)) {
				if (!fi.FieldType.Equals(typeof(KnownArithmeticOperator))) {
					continue;
				}

				KnownArithmeticOperator op = (KnownArithmeticOperator)fi.GetValue(null);

				Debug.Assert(!opCache.ContainsKey(new Key(op.Syntax, op.Arity)),
					"Syntax-arity pair `{0}`{1}` of known operator is not unique.".Fmt(op.Syntax, op.Arity));

				opCache.Add(new Key(op.Syntax, op.Arity), op);
			}
		}

		/// <summary>
		/// Tries to parse given string as operator with given arity.
		/// </summary>
		public static bool TryParse(string syntax, byte arity, out KnownArithmeticOperator result) {
			return opCache.TryGetValue(new Key(syntax, arity), out result);
		}

		#endregion

		public readonly string Syntax;
		public readonly byte Arity;
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

		private EvaluateDelegate evalFunction;


		private KnownArithmeticOperator(string syntax, byte arity, byte prec, byte activePrec, EvaluateDelegate evalFunc){
			Syntax = syntax;
			Arity = arity;
			evalFunction = evalFunc;
			Precedence = prec;
			ActivePrecedence = activePrec;
		}

		#region IEvaluable Members

		byte IEvaluable.Arity { get { return Arity; } }

		public double Evaluate(params double[] args) {
#if DEBUG
			if (args.Length != Arity) {
				throw new ArgumentException("Failed to evaluate operator `{0}'{1}` with {2} argument(s).".Fmt(Syntax, Arity, args.Length));
			}
#endif
			return evalFunction.Invoke(args);
		}

		#endregion
	}
}

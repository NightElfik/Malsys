using System;
using System.Collections.Generic;
using System.Reflection;
using Key = System.Tuple<string, byte>;
using System.Diagnostics;

namespace Malsys.Expressions {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class KnownOperator : IEvaluable {
		#region Static members

		#region Operator definitions

		/// <summary>
		/// Power operator (right associative -- it is not poped by itself).
		/// </summary>
		public static readonly KnownOperator Power = new KnownOperator("^", 2, 3, 3,
			(a) => {ensureParams(IArithmeticValueType.Constant, IArithmeticValueType.Constant, a);
				return Math.Pow((Constant)a[0], (Constant)a[1]).ToConst();
			});

		/// <summary>
		/// Unary plus -- the most important operator ;)
		/// </summary>
		public static readonly KnownOperator Plus = new KnownOperator("+", 1, 4, 2,
			(a) => {
				ensureParams(IArithmeticValueType.Constant, a);
				return a[0];
			});
		public static readonly KnownOperator Minus = new KnownOperator("-", 1, 4, 2,
			(a) => {
				ensureParams(IArithmeticValueType.Constant, a);
				return (-(Constant)a[0]).ToConst();
			});
		public static readonly KnownOperator Not = new KnownOperator("!", 1, 4, 2,
			(a) => {
				ensureParams(IArithmeticValueType.Constant, a);
				return (FloatArithmeticHelper.IsZero((Constant)a[0]) ? 1.0 : 0.0).ToConst();
			});

		public static readonly KnownOperator Multiply = new KnownOperator("*", 2, 5, 6,
			(a) => {
				ensureParams(IArithmeticValueType.Constant, IArithmeticValueType.Constant, a);
				return ((Constant)a[0] * (Constant)a[1]).ToConst();
			});
		public static readonly KnownOperator Divide = new KnownOperator("/", 2, 5, 6,
			(a) => {
				ensureParams(IArithmeticValueType.Constant, IArithmeticValueType.Constant, a);
				return ((Constant)a[0] / (Constant)a[1]).ToConst();
			});
		public static readonly KnownOperator IntDivide = new KnownOperator("\\", 2, 5, 6,
			(a) => {
				ensureParams(IArithmeticValueType.Constant, IArithmeticValueType.Constant, a);
				return ((double)((long)(((Constant)a[0]).Value) / (long)(((Constant)a[1]).Value))).ToConst();
			});
		public static readonly KnownOperator Modulo = new KnownOperator("%", 2, 5, 6,
			(a) => {
				ensureParams(IArithmeticValueType.Constant, IArithmeticValueType.Constant, a);
				return ((Constant)a[0] % (Constant)a[1]).ToConst();
			});

		public static readonly KnownOperator Add = new KnownOperator("+", 2, 7, 8,
			(a) => {
				ensureParams(IArithmeticValueType.Constant, IArithmeticValueType.Constant, a);
				return ((Constant)a[0] + (Constant)a[1]).ToConst();
			});
		public static readonly KnownOperator Subtract = new KnownOperator("-", 2, 7, 8,
			(a) => {
				ensureParams(IArithmeticValueType.Constant, IArithmeticValueType.Constant, a);
				return ((Constant)a[0] - (Constant)a[1]).ToConst();
			});

		public static readonly KnownOperator LessThan = new KnownOperator("<", 2, 9, 10,
			(a) => {
				ensureParams(IArithmeticValueType.Constant, IArithmeticValueType.Constant, a);
				return ((Constant)a[0] < (Constant)a[1] ? 1.0 : 0.0).ToConst();
			});
		public static readonly KnownOperator GreaterThan = new KnownOperator(">", 2, 9, 10,
			(a) => {
				ensureParams(IArithmeticValueType.Constant, IArithmeticValueType.Constant, a);
				return ((Constant)a[0] > (Constant)a[1] ? 1.0 : 0.0).ToConst();
			});
		public static readonly KnownOperator LessThanOrEqual = new KnownOperator("<=", 2, 9, 10,
			(a) => {
				ensureParams(IArithmeticValueType.Constant, IArithmeticValueType.Constant, a);
				return ((Constant)a[0] <= (Constant)a[1] ? 1.0 : 0.0).ToConst();
			});
		public static readonly KnownOperator GreaterThanOrEqual = new KnownOperator(">=", 2, 9, 10,
			(a) => {
				ensureParams(IArithmeticValueType.Constant, IArithmeticValueType.Constant, a);
				return ((Constant)a[0] >= (Constant)a[1] ? 1.0 : 0.0).ToConst();
			});

		public static readonly KnownOperator Equal = new KnownOperator("==", 2, 11, 12,
			(a) => {
				ensureParams(IArithmeticValueType.Constant, IArithmeticValueType.Constant, a);
				return (FloatArithmeticHelper.IsZero((Constant)a[0] - (Constant)a[1]) ? 1.0 : 0.0).ToConst();
			});

		public static readonly KnownOperator And = new KnownOperator("&&", 2, 13, 14,
			(a) => {
				ensureParams(IArithmeticValueType.Constant, IArithmeticValueType.Constant, a);
				return ((FloatArithmeticHelper.IsZero((Constant)a[0]) || FloatArithmeticHelper.IsZero((Constant)a[1])) ? 0.0 : 1.0).ToConst();
			});

		public static readonly KnownOperator Xor = new KnownOperator("^^", 2, 13, 14,
			(a) => {
				ensureParams(IArithmeticValueType.Constant, IArithmeticValueType.Constant, a);
				return ((FloatArithmeticHelper.IsZero((Constant)a[0]) == FloatArithmeticHelper.IsZero((Constant)a[1])) ? 0.0 : 1.0).ToConst();
			});

		public static readonly KnownOperator Or = new KnownOperator("||", 2, 17, 18,
			(a) => {
				ensureParams(IArithmeticValueType.Constant, IArithmeticValueType.Constant, a);
				return ((FloatArithmeticHelper.IsZero((Constant)a[0]) && FloatArithmeticHelper.IsZero((Constant)a[1])) ? 0.0 : 1.0).ToConst();
			});

		#endregion


		static Dictionary<Key, KnownOperator> opCache;

		/// <summary>
		/// Builds operators cache from definitions in this class.
		/// </summary>
		static KnownOperator() {
			opCache = new Dictionary<Key, KnownOperator>();

			foreach (FieldInfo fi in typeof(KnownOperator).GetFields(BindingFlags.Public | BindingFlags.Static)) {
				if (!fi.FieldType.Equals(typeof(KnownOperator))) {
					continue;
				}

				KnownOperator op = (KnownOperator)fi.GetValue(null);
				Key key = new Key(op.Syntax.ToLowerInvariant(), op.Arity);

				Debug.Assert(!opCache.ContainsKey(key),
					"Syntax-arity pair `{0}`{1}` of known operator is not unique.".Fmt(key.Item1, op.Arity));

				opCache.Add(key, op);
			}
		}

		/// <summary>
		/// Tries to parse given string as operator with given arity.
		/// </summary>
		public static bool TryParse(string syntax, byte arity, out KnownOperator result) {
			return opCache.TryGetValue(new Key(syntax, arity), out result);
		}

		/// <summary>
		/// Ensures parameters of unary operator and throws exception if they dont match.
		/// </summary>
		private static void ensureParams(IArithmeticValueType desiredType, ArgsStorage args) {
			if (args[0].Type != desiredType) {
				throw new EvalException("As operand was excpected {0}, but it is {1}.".Fmt(
					desiredType == IArithmeticValueType.Constant ? "value" : "array",
					args[0].Type == IArithmeticValueType.Constant ? "value" : "array"));
			}
		}

		/// <summary>
		/// Ensures parameters of binary operator and throws exception if they dont match.
		/// </summary>
		private static void ensureParams(IArithmeticValueType desiredType0, IArithmeticValueType desiredType1, ArgsStorage args) {
			if (args[0].Type != desiredType0) {
				throw new EvalException("As left operand was excpected {0}, but it is {1}.".Fmt(
					desiredType0 == IArithmeticValueType.Constant ? "value" : "array",
					args[0].Type == IArithmeticValueType.Constant ? "value" : "array"));
			}
			if (args[1].Type != desiredType1) {
				throw new EvalException("As right operand was excpected {0}, but it is {1}.".Fmt(
					desiredType1 == IArithmeticValueType.Constant ? "value" : "array",
					args[1].Type == IArithmeticValueType.Constant ? "value" : "array"));
			}
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


		private KnownOperator(string syntax, byte arity, byte prec, byte activePrec, EvaluateDelegate evalFunc) {
			Syntax = syntax;
			Arity = arity;
			evalFunction = evalFunc;
			Precedence = prec;
			ActivePrecedence = activePrec;
		}

		#region IEvaluable Members

		int IEvaluable.Arity { get { return Arity; } }
		public string Name { get { return "operator"; } }
		string IEvaluable.Syntax { get { return Syntax; } }

		public IValue Evaluate(ArgsStorage args) {
			if (args.ArgsCount != Arity) {
				throw new EvalException("Failed to evaluate operator `{0}` with {1} argument(s), it needs {2}.".Fmt(Syntax, args.ArgsCount, Arity));
			}

			try {
				return evalFunction.Invoke(args);
			}
			catch (EvalException ex) {
				throw new EvalException("Failed to evaluate operator `{0}`. {1}".Fmt(Syntax), ex);
			}
		}

		#endregion

		#region IPostfixExpressionMember Members

		public bool IsConstant { get { return false; } }
		public bool IsArray { get { return false; } }
		public bool IsVariable { get { return false; } }
		public bool IsEvaluable { get { return true; } }
		public bool IsUnknownFunction { get { return false; } }

		#endregion

		#region IEvaluable Members



		#endregion
	}
}

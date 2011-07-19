using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Key = System.Tuple<string, byte>;

namespace Malsys.Expressions {
	public delegate double EvaluateDelegate(double[] parameters);

	/// <summary>
	/// Immutable.
	/// </summary>
	public class KnownArithmeticFunction : IEvaluable {
		#region Static members

		public const byte AnyArity = byte.MaxValue;

		#region Known functions definitions

		#region Abs, sign, round, floor, ceiling, min, max

		public static readonly KnownArithmeticFunction Abs = new KnownArithmeticFunction("abs", 1,
			(a) => { return Math.Abs(a[0]); });

		public static readonly KnownArithmeticFunction Sign = new KnownArithmeticFunction("sign", 1,
			(a) => { return Math.Sign(a[0]); });

		public static readonly KnownArithmeticFunction Round = new KnownArithmeticFunction("round", 1,
			(a) => { return Math.Round(a[0]); });

		public static readonly KnownArithmeticFunction Floor = new KnownArithmeticFunction("floor", 1,
			(a) => { return Math.Floor(a[0]); });

		public static readonly KnownArithmeticFunction Ceiling = new KnownArithmeticFunction("ceil", 1,
			(a) => { return Math.Ceiling(a[0]); });

		public static readonly KnownArithmeticFunction Min = new KnownArithmeticFunction("min", AnyArity,
			(a) => { return a.Min(); });

		public static readonly KnownArithmeticFunction Max = new KnownArithmeticFunction("max", AnyArity,
			(a) => { return a.Max(); });

		#endregion

		#region Sqrt, log, log10

		public static readonly KnownArithmeticFunction Sqrt = new KnownArithmeticFunction("sqrt", 1,
			(a) => { return Math.Sqrt(a[0]); });

		public static readonly KnownArithmeticFunction Log = new KnownArithmeticFunction("log", 1,
			(a) => { return Math.Log(a[0]); });

		public static readonly KnownArithmeticFunction LogBase = new KnownArithmeticFunction("log", 2,
			(a) => { return Math.Log(a[0], a[1]); });

		public static readonly KnownArithmeticFunction Log10 = new KnownArithmeticFunction("log10", 1,
			(a) => { return Math.Log10(a[0]); });

		#endregion

		#region Sin, cos, tan, asin, acos, atan, atan2

		public static readonly KnownArithmeticFunction Sin = new KnownArithmeticFunction("sin", 1,
			(a) => { return Math.Sin(a[0]); });

		public static readonly KnownArithmeticFunction Cos = new KnownArithmeticFunction("cos", 1,
			(a) => { return Math.Cos(a[0]); });

		public static readonly KnownArithmeticFunction Tan = new KnownArithmeticFunction("tan", 1,
			(a) => { return Math.Tan(a[0]); });

		public static readonly KnownArithmeticFunction Asin = new KnownArithmeticFunction("asin", 1,
			(a) => { return Math.Asin(a[0]); });

		public static readonly KnownArithmeticFunction Acos = new KnownArithmeticFunction("acos", 1,
			(a) => { return Math.Acos(a[0]); });

		public static readonly KnownArithmeticFunction Atan = new KnownArithmeticFunction("atan", 1,
			(a) => { return Math.Atan(a[0]); });

		public static readonly KnownArithmeticFunction Atan2 = new KnownArithmeticFunction("cos", 2,
			(a) => { return Math.Atan2(a[0], a[1]); });

		#endregion

		#endregion


		static Dictionary<Key, KnownArithmeticFunction> funCache;

		/// <summary>
		/// Builds known functions cache.
		/// </summary>
		static KnownArithmeticFunction() {
			funCache = new Dictionary<Key, KnownArithmeticFunction>();

			foreach (FieldInfo fi in typeof(KnownArithmeticFunction).GetFields(BindingFlags.Public | BindingFlags.Static)) {
				if (!fi.FieldType.Equals(typeof(KnownArithmeticFunction))) {
					continue;
				}

				KnownArithmeticFunction fun = (KnownArithmeticFunction)fi.GetValue(null);

				funCache.Add(new Key(fun.Name, fun.Arity), fun);
			}
		}

		/// <summary>
		/// Tries to parse given string as function with given arity. Returned function have allways desired syntax and arity.
		/// </summary>
		public static bool TryGet(string syntax, byte arity, out KnownArithmeticFunction result) {
			Debug.Assert(arity != AnyArity, "Querry for function have to be with concrete arity (not any arity).");

			if (funCache.TryGetValue(new Key(syntax, arity), out result)) {
				return true;
			}
			else if (funCache.TryGetValue(new Key(syntax, AnyArity), out result)) {
				// set arity to desired number
				result = new KnownArithmeticFunction(result.Name, arity, result.evalFunction);
				return true;
			}
			else {
				return false;
			}
		}

		#endregion

		public readonly string Name;
		public readonly byte Arity;

		private EvaluateDelegate evalFunction;

		public KnownArithmeticFunction(string name, byte arity, EvaluateDelegate evalFunc) {
			Name = name;
			Arity = arity;
			evalFunction = evalFunc;
		}

		#region IEvaluable Members

		byte IEvaluable.Arity { get { return Arity; } }

		public double Evaluate(params double[] values) {
#if DEBUG
			if (Arity != AnyArity && values.Length != Arity) {
				throw new ArgumentException("Failed to evaluate function `{0}'{1}` with {2} argument(s).".Fmt(Name, Arity, values.Length));
			}
#endif
			return evalFunction.Invoke(values);
		}

		#endregion
	}
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Key = System.Tuple<string, byte>;

namespace Malsys.Expressions {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class KnownFunction : IEvaluable, IPostfixExpressionMember {
		#region Static members

		public const byte AnyArity = byte.MaxValue;

		#region Known functions definitions

		#region Abs, sign, round, floor, ceiling, min, max

		public static readonly KnownFunction Abs = new KnownFunction("abs", 1,
			(a) => {
				ensureParam(0, IArithmeticValueType.Constant, a);
				return Math.Abs((Constant)a[0]).ToConst();
			});

		public static readonly KnownFunction Sign = new KnownFunction("sign", 1,
			(a) => {
				ensureParam(0, IArithmeticValueType.Constant, a);
				return Math.Sign((Constant)a[0]).ToConst();
			});

		public static readonly KnownFunction Round = new KnownFunction("round", 1,
			(a) => {
				ensureParam(0, IArithmeticValueType.Constant, a);
				return Math.Round((Constant)a[0]).ToConst();
			});

		public static readonly KnownFunction Floor = new KnownFunction("floor", 1,
			(a) => {
				ensureParam(0, IArithmeticValueType.Constant, a);
				return Math.Floor((Constant)a[0]).ToConst();
			});

		public static readonly KnownFunction Ceiling = new KnownFunction("ceil", 1,
			(a) => {
				ensureParam(0, IArithmeticValueType.Constant, a);
				return Math.Ceiling((Constant)a[0]).ToConst();
			});

		public static readonly KnownFunction Min = new KnownFunction("min", AnyArity,
			(a) => { return a.Min(); });

		public static readonly KnownFunction Max = new KnownFunction("max", AnyArity,
			(a) => { return a.Max(); });

		#endregion

		#region Sqrt, log, log10, sum, product, average

		public static readonly KnownFunction Sqrt = new KnownFunction("sqrt", 1,
			(a) => {
				ensureParam(0, IArithmeticValueType.Constant, a);
				return Math.Sqrt((Constant)a[0]).ToConst();
			});

		public static readonly KnownFunction Log = new KnownFunction("log", 1,
			(a) => {
				ensureParam(0, IArithmeticValueType.Constant, a);
				return Math.Log((Constant)a[0]).ToConst();
			});

		public static readonly KnownFunction LogBase = new KnownFunction("log", 2,
			(a) => {
				ensureParam(0, IArithmeticValueType.Constant, a);
				ensureParam(1, IArithmeticValueType.Constant, a);
				return Math.Log((Constant)a[0], (Constant)a[1]).ToConst();
			});

		public static readonly KnownFunction Log10 = new KnownFunction("log10", 1,
			(a) => {
				ensureParam(0, IArithmeticValueType.Constant, a);
				return Math.Log10((Constant)a[0]).ToConst();
			});

		public static readonly KnownFunction Sum = new KnownFunction("sum", AnyArity,
			(a) => {
				ensureParam(-1, IArithmeticValueType.Constant, a);
				return a.Aggregate(0.0, (acc, val) => acc + (Constant)val).ToConst();
			});

		public static readonly KnownFunction Product = new KnownFunction("prod", AnyArity,
			(a) => {
				ensureParam(-1, IArithmeticValueType.Constant, a);
				return a.Aggregate(1.0, (acc, val) => acc * (Constant)val).ToConst();
			});

		public static readonly KnownFunction Average = new KnownFunction("avg", AnyArity,
			(a) => {
				ensureParam(-1, IArithmeticValueType.Constant, a);
				return (a.Aggregate(0.0, (acc, val) => acc + (Constant)val) / a.ArgsCount).ToConst();
			});

		#endregion

		#region Sin, cos, tan, asin, acos, atan, atan2

		public static readonly KnownFunction Sin = new KnownFunction("sin", 1,
			(a) => {
				ensureParam(0, IArithmeticValueType.Constant, a);
				return Math.Sin((Constant)a[0]).ToConst();
			});

		public static readonly KnownFunction Cos = new KnownFunction("cos", 1,
			(a) => {
				ensureParam(0, IArithmeticValueType.Constant, a);
				return Math.Cos((Constant)a[0]).ToConst();
			});

		public static readonly KnownFunction Tan = new KnownFunction("tan", 1,
			(a) => {
				ensureParam(0, IArithmeticValueType.Constant, a);
				return Math.Tan((Constant)a[0]).ToConst();
			});

		public static readonly KnownFunction Asin = new KnownFunction("asin", 1,
			(a) => {
				ensureParam(0, IArithmeticValueType.Constant, a);
				return Math.Asin((Constant)a[0]).ToConst();
			});

		public static readonly KnownFunction Acos = new KnownFunction("acos", 1,
			(a) => {
				ensureParam(0, IArithmeticValueType.Constant, a);
				return Math.Acos((Constant)a[0]).ToConst();
			});

		public static readonly KnownFunction Atan = new KnownFunction("atan", 1,
			(a) => {
				ensureParam(0, IArithmeticValueType.Constant, a);
				return Math.Atan((Constant)a[0]).ToConst();
			});

		public static readonly KnownFunction Atan2 = new KnownFunction("atan2", 2,
			(a) => {
				ensureParam(0, IArithmeticValueType.Constant, a);
				ensureParam(1, IArithmeticValueType.Constant, a);
				return Math.Atan2((Constant)a[0], (Constant)a[1]).ToConst();
			});

		#endregion

		#region If, length

		public static readonly KnownFunction If = new KnownFunction("if", 3,
			(a) => {
				ensureParam(0, IArithmeticValueType.Constant, a);
				if (FloatArithmeticHelper.IsZero((Constant)a[0])) {
					return a[2];  // 0 (false)
				}
				else {
					return a[1]; // non-0 (true)
				}
			});

		public static readonly KnownFunction Length = new KnownFunction("length", 1,
			(a) => {
				if (a[0].IsConstant) {
					return 0.0.ToConst();
				}
				else {
					return ((ValuesArray)a[0]).Length.ToConst();
				}
			});

		#endregion

		#endregion


		static Dictionary<Key, KnownFunction> funCache;

		/// <summary>
		/// Builds known functions cache from definitions in this class.
		/// </summary>
		static KnownFunction() {
			funCache = new Dictionary<Key, KnownFunction>();

			foreach (FieldInfo fi in typeof(KnownFunction).GetFields(BindingFlags.Public | BindingFlags.Static)) {
				if (!fi.FieldType.Equals(typeof(KnownFunction))) {
					continue;
				}

				KnownFunction fun = (KnownFunction)fi.GetValue(null);
				Key key = new Key(fun.Name.ToLowerInvariant(), fun.Arity);

				Debug.Assert(!funCache.ContainsKey(key),
					"Name-arity pair `{0}`{1}` of known function is not unique.".Fmt(key.Item1, fun.Arity));

				funCache.Add(key, fun);
			}
		}

		/// <summary>
		/// Tries to parse given string as function with given arity. Returned function have allways desired syntax and arity.
		/// </summary>
		public static bool TryGet(string syntax, byte arity, out KnownFunction result) {
			Debug.Assert(arity != AnyArity, "Querry for function have to be with concrete arity (not any arity).");

			if (funCache.TryGetValue(new Key(syntax, arity), out result)) {
				return true;
			}
			else if (funCache.TryGetValue(new Key(syntax, AnyArity), out result)) {
				// set arity to desired number
				result = new KnownFunction(result.Name, arity, result.evalFunction);
				return true;
			}
			else {
				return false;
			}
		}

		/// <summary>
		/// Ensures parameter from given array at given index to be given type and throws exception if it is not.
		/// </summary>
		/// <param name="argNum">Zero-based argument number, if it is negative, checks all arguments in given array.</param>
		private static void ensureParam(int argNum, IArithmeticValueType desiredType, ArgsStorage args) {
			if (argNum < 0) {
				for (int i = 0; i < args.ArgsCount; i++) {
					ensureParam(i, desiredType, args);
				}
			}
			else {
				if (args[argNum].Type != desiredType) {
					throw new EvalException("As {0}. argument was excpected {1}, but it is {2}.".Fmt(
						argNum + 1,
						desiredType == IArithmeticValueType.Constant ? "value" : "array",
						args[argNum].Type == IArithmeticValueType.Constant ? "value" : "array"));
				}
			}
		}

		#endregion

		public readonly string Name;
		public readonly byte Arity;

		private EvaluateDelegate evalFunction;

		public KnownFunction(string name, byte arity, EvaluateDelegate evalFunc) {
			Name = name;
			Arity = arity;
			evalFunction = evalFunc;
		}

		#region IEvaluable Members

		int IEvaluable.Arity { get { return Arity; } }
		string IEvaluable.Name { get { return "function"; } }
		public string Syntax { get { return Name; } }

		public IValue Evaluate(ArgsStorage args) {
			if (args.ArgsCount != Arity) {
				throw new EvalException("Failed to evaluate function `{0}` with {1} argument(s), it needs {2}.".Fmt(Name, args.ArgsCount, Arity));
			}

			try {
				return evalFunction.Invoke(args);
			}
			catch (EvalException ex) {
				throw new EvalException("Failed to evaluate function `{0}`. {1}".Fmt(Name), ex);
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
	}
}

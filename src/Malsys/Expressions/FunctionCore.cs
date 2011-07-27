using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Key = System.Tuple<string, int>;

namespace Malsys.Expressions {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class FunctionCore {
		#region Static members

		public const int AnyParamsCount = int.MaxValue;


		#region Known functions definitions

		#region Abs, sign, round, floor, ceiling, min, max

		public static readonly FunctionCore Abs = new FunctionCore("abs", 1,
			new ExpressionValueType[] { ExpressionValueType.Constant },
			(a) => {
				return Math.Abs((Constant)a[0]).ToConst();
			});

		public static readonly FunctionCore Sign = new FunctionCore("sign", 1,
			new ExpressionValueType[] { ExpressionValueType.Constant },
			(a) => {
				return Math.Sign((Constant)a[0]).ToConst();
			});

		public static readonly FunctionCore Round = new FunctionCore("round", 1,
			new ExpressionValueType[] { ExpressionValueType.Constant },
			(a) => {
				return Math.Round((Constant)a[0]).ToConst();
			});

		public static readonly FunctionCore Floor = new FunctionCore("floor", 1,
			new ExpressionValueType[] { ExpressionValueType.Constant },
			(a) => {
				return Math.Floor((Constant)a[0]).ToConst();
			});

		public static readonly FunctionCore Ceiling = new FunctionCore("ceiling", 1,
			new ExpressionValueType[] { ExpressionValueType.Constant },
			(a) => {
				return Math.Ceiling((Constant)a[0]).ToConst();
			});

		public static readonly FunctionCore Ceil = Ceiling.ChangeName("ceil");

		public static readonly FunctionCore Min = new FunctionCore("min", AnyParamsCount,
			new ExpressionValueType[] { ExpressionValueType.Any },
			(a) => { return a.Min(); });

		public static readonly FunctionCore Max = new FunctionCore("max", AnyParamsCount,
			new ExpressionValueType[] { ExpressionValueType.Any },
			(a) => { return a.Max(); });

		#endregion

		#region Sqrt, factorial, log, log10, sum, product, average

		public static readonly FunctionCore Sqrt = new FunctionCore("sqrt", 1,
			new ExpressionValueType[] { ExpressionValueType.Constant },
			(a) => {
				return Math.Sqrt((Constant)a[0]).ToConst();
			});

		public static readonly FunctionCore Factorial = new FunctionCore("factorial", 1,
			new ExpressionValueType[] { ExpressionValueType.Constant },
			(a) => {
				double result = 1.0;
				double max = (Constant)a[0];
				for (int i = 2; i <= max; i++) { result *= i; }
				return result.ToConst();
			});

		public static readonly FunctionCore Fact = Factorial.ChangeName("fact");

		public static readonly FunctionCore Log = new FunctionCore("log", 1,
			new ExpressionValueType[] { ExpressionValueType.Constant },
			(a) => {
				return Math.Log((Constant)a[0]).ToConst();
			});

		public static readonly FunctionCore LogBase = new FunctionCore("log", 2,
			new ExpressionValueType[] { ExpressionValueType.Constant },
			(a) => {
				return Math.Log((Constant)a[0], (Constant)a[1]).ToConst();
			});

		public static readonly FunctionCore Log10 = new FunctionCore("log10", 1,
			new ExpressionValueType[] { ExpressionValueType.Constant },
			(a) => {
				return Math.Log10((Constant)a[0]).ToConst();
			});

		public static readonly FunctionCore Sum = new FunctionCore("sum", AnyParamsCount,
			new ExpressionValueType[] { ExpressionValueType.Constant },
			(a) => {
				return a.Aggregate(0.0, (acc, val) => acc + (Constant)val).ToConst();
			});

		public static readonly FunctionCore Product = new FunctionCore("product", AnyParamsCount,
			new ExpressionValueType[] { ExpressionValueType.Constant },
			(a) => {
				return a.Aggregate(1.0, (acc, val) => acc * (Constant)val).ToConst();
			});

		public static readonly FunctionCore Prod = Product.ChangeName("prod");

		public static readonly FunctionCore Average = new FunctionCore("average", AnyParamsCount,
			new ExpressionValueType[] { ExpressionValueType.Constant },
			(a) => {
				return (a.Aggregate(0.0, (acc, val) => acc + (Constant)val) / a.ArgsCount).ToConst();
			});

		public static readonly FunctionCore Avg = Average.ChangeName("avg");

		#endregion

		#region Deg2rad, rad2deg, sin, cos, tan, asin, acos, atan, atan2

		public static readonly FunctionCore Deg2Rad = new FunctionCore("deg2rad", 1,
			new ExpressionValueType[] { ExpressionValueType.Constant },
			(a) => {
				return ((Constant)a[0] * (Math.PI / 180)).ToConst();
			});

		public static readonly FunctionCore Rad2Deg = new FunctionCore("rad2deg", 1,
			new ExpressionValueType[] { ExpressionValueType.Constant },
			(a) => {
				return ((Constant)a[0] * (180 / Math.PI)).ToConst();
			});

		public static readonly FunctionCore Sin = new FunctionCore("sin", 1,
			new ExpressionValueType[] { ExpressionValueType.Constant },
			(a) => {
				return Math.Sin((Constant)a[0]).ToConst();
			});

		public static readonly FunctionCore Cos = new FunctionCore("cos", 1,
			new ExpressionValueType[] { ExpressionValueType.Constant },
			(a) => {
				return Math.Cos((Constant)a[0]).ToConst();
			});

		public static readonly FunctionCore Tan = new FunctionCore("tan", 1,
			new ExpressionValueType[] { ExpressionValueType.Constant },
			(a) => {
				return Math.Tan((Constant)a[0]).ToConst();
			});

		public static readonly FunctionCore Asin = new FunctionCore("asin", 1,
			new ExpressionValueType[] { ExpressionValueType.Constant },
			(a) => {
				return Math.Asin((Constant)a[0]).ToConst();
			});

		public static readonly FunctionCore Acos = new FunctionCore("acos", 1,
			new ExpressionValueType[] { ExpressionValueType.Constant },
			(a) => {
				return Math.Acos((Constant)a[0]).ToConst();
			});

		public static readonly FunctionCore Atan = new FunctionCore("atan", 1,
			new ExpressionValueType[] { ExpressionValueType.Constant },
			(a) => {
				return Math.Atan((Constant)a[0]).ToConst();
			});

		public static readonly FunctionCore Atan2 = new FunctionCore("atan2", 2,
			new ExpressionValueType[] { ExpressionValueType.Constant },
			(a) => {
				return Math.Atan2((Constant)a[0], (Constant)a[1]).ToConst();
			});

		#endregion

		#region If, length, isNan, isInfinity

		public static readonly FunctionCore If = new FunctionCore("if", 3,
			new ExpressionValueType[] { ExpressionValueType.Constant, ExpressionValueType.Any, ExpressionValueType.Any },
			(a) => {
				if (FloatArithmeticHelper.IsZero((Constant)a[0])) {
					return a[2];  // 0 (false)
				}
				else {
					return a[1]; // non-0 (true)
				}
			});

		public static readonly FunctionCore Length = new FunctionCore("length", 1,
			new ExpressionValueType[] { ExpressionValueType.Any },
			(a) => {
				if (a[0].IsConstant) {
					return 0.0.ToConst();
				}
				else {
					return ((ValuesArray)a[0]).Length.ToConst();
				}
			});

		public static readonly FunctionCore Len = Length.ChangeName("len");

		public static readonly FunctionCore IsNan = new FunctionCore("isNan", 1,
			new ExpressionValueType[] { ExpressionValueType.Constant },
			(a) => {
				return (double.IsNaN((Constant)a[0]) ? 1.0 : 0.0).ToConst();
			});

		public static readonly FunctionCore IsInfinity = new FunctionCore("isInfinity", 1,
			new ExpressionValueType[] { ExpressionValueType.Constant },
			(a) => {
				return (double.IsInfinity((Constant)a[0]) ? 1.0 : 0.0).ToConst();
			});

		public static readonly FunctionCore IsInfty = IsInfinity.ChangeName("isInfty");

		#endregion

		#endregion


		static Dictionary<Key, FunctionCore> funCache;

		/// <summary>
		/// Builds known functions cache from definitions in this class.
		/// </summary>
		static FunctionCore() {
			funCache = new Dictionary<Key, FunctionCore>();

			foreach (FieldInfo fi in typeof(FunctionCore).GetFields(BindingFlags.Public | BindingFlags.Static)) {
				if (!fi.FieldType.Equals(typeof(FunctionCore))) {
					continue;
				}

				FunctionCore fun = (FunctionCore)fi.GetValue(null);
				Key key = new Key(fun.Name.ToLowerInvariant(), fun.ParametersCount);

				Debug.Assert(!funCache.ContainsKey(key),
					"Name-arity pair `{0}` [{1}] of known function is not unique.".Fmt(key.Item1, fun.ParametersCount));

				funCache.Add(key, fun);
			}
		}

		/// <summary>
		/// Tries to get function with given arity and syntax equal to given string.
		/// Returned function have allways desired syntax and arity.
		/// </summary>
		public static bool TryGet(string syntax, int paramsCount, out FunctionCore result) {
			Debug.Assert(paramsCount != AnyParamsCount, "Querry for function have to be with concrete params count (not any params count).");

			if (funCache.TryGetValue(new Key(syntax, paramsCount), out result)) {
				return true;
			}
			else if (funCache.TryGetValue(new Key(syntax, AnyParamsCount), out result)) {
				// set arity to desired number
				result = new FunctionCore(result.Name, paramsCount, result.ParamsTypes, result.EvalFunction);
				return true;
			}
			else {
				return false;
			}
		}

		#endregion


		public readonly string Name;
		/// <summary>
		/// If function have params count equal to <c>AnyParamsCount</c> constant, any number of arguments can be supplied.
		/// </summary>
		public readonly int ParametersCount;

		/// <summary>
		/// Types of parameters of function.
		/// Length of the array do not have to be same length as function's arity.
		/// If function have more params than length of this array, modulo is used to cycle in it.
		/// </summary>
		public readonly ImmutableList<ExpressionValueType> ParamsTypes;

		public readonly EvalDelegate EvalFunction;


		private FunctionCore(string name, int paramsCount, ExpressionValueType[] paramsTypes, EvalDelegate evalFunc) {
			Name = name;
			ParametersCount = paramsCount;
			ParamsTypes = new ImmutableList<ExpressionValueType>(paramsTypes, true);
			EvalFunction = evalFunc;
		}

		private FunctionCore(string name, int paramsCount, ImmutableList<ExpressionValueType> paramsTypes, EvalDelegate evalFunc) {
			Name = name;
			ParametersCount = paramsCount;
			ParamsTypes = paramsTypes;
			EvalFunction = evalFunc;
		}

		internal FunctionCore ChangeName(string newName) {
			return new FunctionCore(newName, ParametersCount, ParamsTypes, EvalFunction);
		}
	}
}

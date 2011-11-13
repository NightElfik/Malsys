using System;
using System.Diagnostics.Contracts;
using System.Linq;
using Malsys.SemanticModel.Evaluated;
using Malsys.SemanticModel;
using Malsys.Evaluators;

namespace Malsys.Compilers.Expressions {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class FunctionCore {

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
				if (a[0].IsNaN) { return Constant.NaN; }
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

		public static readonly FunctionCore Ceil = Ceiling.ChangeNameTo("ceil");

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
				if (a[0].IsNaN) { return Constant.NaN; }
				double result = 1.0;
				double max = (Constant)a[0];
				for (int i = 2; i <= max; i++) {
					result *= i;
				}
				return result.ToConst();
			});

		public static readonly FunctionCore Fact = Factorial.ChangeNameTo("fact");

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

		public static readonly FunctionCore Prod = Product.ChangeNameTo("prod");

		public static readonly FunctionCore Average = new FunctionCore("average", AnyParamsCount,
			new ExpressionValueType[] { ExpressionValueType.Constant },
			(a) => {
				return (a.Aggregate(0.0, (acc, val) => acc + (Constant)val) / a.ArgsCount).ToConst();
			});

		public static readonly FunctionCore Avg = Average.ChangeNameTo("avg");

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

		#region Length, isNan, isInfinity

		public static readonly FunctionCore Length = new FunctionCore("length", 1,
			new ExpressionValueType[] { ExpressionValueType.Any },
			(a) => {
				if (a[0].IsConstant) {
					return Constant.Zero;
				}
				else {
					return ((ValuesArray)a[0]).Length.ToConst();
				}
			});

		public static readonly FunctionCore Len = Length.ChangeNameTo("len");

		public static readonly FunctionCore IsNan = new FunctionCore("isNan", 1,
			new ExpressionValueType[] { ExpressionValueType.Any },
			(a) => {
				return a[0].IsNaN ? Constant.True : Constant.False;
			});

		public static readonly FunctionCore IsInfinity = new FunctionCore("isInfinity", 1,
			new ExpressionValueType[] { ExpressionValueType.Any },
			(a) => {
				if (a[0].IsConstant) {
					return ((Constant)a[0]).IsInfinity ? Constant.True : Constant.False;
				}
				else {
					return Constant.False;
				}
			});

		public static readonly FunctionCore IsInfty = IsInfinity.ChangeNameTo("isInfty");

		#endregion

		#region If

		/// <summary>
		/// This is dummy function for IF with no body. Implementation have to be in expression evaluator.
		/// </summary>
		/// <remarks>
		/// Function IF can not be directly written like others functions, because its 2nd and 3rd arguments can not be
		/// evaluated till decision fom 1st is made. Recursion is the problem. If recursive call is in 2nd or 3rd
		/// argument and it is tried to evaluate, stack overflow excetion will obviously occur.
		/// </remarks>
		public static readonly FunctionCore If = new FunctionCore("if", 3,
			new ExpressionValueType[] { ExpressionValueType.Constant, ExpressionValueType.Any, ExpressionValueType.Any },
			null);

		#endregion

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

		public readonly Func<ArgsStorage, IValue> EvalFunction;


		private FunctionCore(string name, int paramsCount, ExpressionValueType[] paramsTypes, Func<ArgsStorage, IValue> evalFunc)
			: this(name, paramsCount, new ImmutableList<ExpressionValueType>(paramsTypes, true), evalFunc) {
		}

		public FunctionCore(string name, int paramsCount, ImmutableList<ExpressionValueType> paramsTypes, Func<ArgsStorage, IValue> evalFunc) {

			Contract.Requires<ArgumentNullException>(name != null);
			Contract.Requires<ArgumentOutOfRangeException>(paramsCount >= 0);
			Contract.Requires<ArgumentException>(paramsTypes != null);
			Contract.Requires<ArgumentNullException>(evalFunc != null);

			Name = name;
			ParametersCount = paramsCount;
			ParamsTypes = paramsTypes;
			EvalFunction = evalFunc;
		}


		public FunctionCore ChangeNameTo(string newName) {
			return new FunctionCore(newName, ParametersCount, ParamsTypes, EvalFunction);
		}
	}
}

using System;
using System.Linq;
using Malsys.Compilers.Expressions;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Resources {
	public static class StdFunctions {


		#region Mathematical functions -- sqrt, factorial, log, log10

		public const string mathFunsGroup = "Mathematical functions";

		public static readonly FunctionCore Sqrt = new FunctionCore("sqrt",
			mathFunsGroup,
			"Returns the square root of a given number.",
			1,
			new ExpressionValueTypeFlags[] { ExpressionValueTypeFlags.Constant },
			(a) => {
				return Math.Sqrt((Constant)a[0]).ToConst();
			});

		public static readonly FunctionCore Factorial = new FunctionCore("factorial",
			mathFunsGroup,
			"Returns the product of all positive integers less than or equal to given number.",
			1,
			new ExpressionValueTypeFlags[] { ExpressionValueTypeFlags.Constant },
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

		public static readonly FunctionCore Log = new FunctionCore("log",
			mathFunsGroup,
			"Returns the natural (base e) logarithm of a given number.",
			1,
			new ExpressionValueTypeFlags[] { ExpressionValueTypeFlags.Constant },
			(a) => {
				return Math.Log((Constant)a[0]).ToConst();
			});

		public static readonly FunctionCore LogBase = new FunctionCore("log",
			mathFunsGroup,
			" Returns the logarithm of a given number in a given base.",
			2,
			new ExpressionValueTypeFlags[] { ExpressionValueTypeFlags.Constant, ExpressionValueTypeFlags.Constant },
			(a) => {
				return Math.Log((Constant)a[0], (Constant)a[1]).ToConst();
			});

		public static readonly FunctionCore Log10 = new FunctionCore("log10",
			mathFunsGroup,
			"Returns the base 10 logarithm of a given number.",
			1,
			new ExpressionValueTypeFlags[] { ExpressionValueTypeFlags.Constant },
			(a) => {
				return Math.Log10((Constant)a[0]).ToConst();
			});

		#endregion


		#region Rounding functions -- round, floor, ceiling

		public const string roundFunsGroup = "Rounding functions";

		public static readonly FunctionCore Round = new FunctionCore("round",
			roundFunsGroup,
			"Rounds given number to the nearest integral value. If the fractional component of a is halfway between two integers, "
				+ "one of which is even and the other odd, then the even number is returned.",
			1,
			new ExpressionValueTypeFlags[] { ExpressionValueTypeFlags.Constant },
			(a) => {
				return Math.Round((Constant)a[0]).ToConst();
			});

		public static readonly FunctionCore Floor = new FunctionCore("floor",
			roundFunsGroup,
			"Returns the largest integer less than or equal to the given number.",
			1,
			new ExpressionValueTypeFlags[] { ExpressionValueTypeFlags.Constant },
			(a) => {
				return Math.Floor((Constant)a[0]).ToConst();
			});

		public static readonly FunctionCore Ceiling = new FunctionCore("ceiling",
			roundFunsGroup,
			"Returns the smallest integer greater than or equal to the given number.",
			1,
			new ExpressionValueTypeFlags[] { ExpressionValueTypeFlags.Constant },
			(a) => {
				return Math.Ceiling((Constant)a[0]).ToConst();
			});

		public static readonly FunctionCore Ceil = Ceiling.ChangeNameTo("ceil");

		#endregion


		#region Multi-values functions -- min, max, sum, product, average

		public const string multiFunsGroup = "Multi-values functions";

		public static readonly FunctionCore Min = new FunctionCore("min",
			multiFunsGroup,
			"Returns the minimum from all given arguments. Returns NaN (not a number) if no argument is given.",
			FunctionCore.AnyParamsCount,
			new ExpressionValueTypeFlags[] { ExpressionValueTypeFlags.Any },
			(a) => {
				if (a.ArgsCount == 0) {
					return Constant.NaN;
				}
				else {
					return a.Min();
				}
			});

		public static readonly FunctionCore Max = new FunctionCore("max",
			multiFunsGroup,
			"Returns the average from all given arguments. Returns NaN (not a number) if no argument is given.",
			FunctionCore.AnyParamsCount,
			new ExpressionValueTypeFlags[] { ExpressionValueTypeFlags.Any },
			(a) => {
				if (a.ArgsCount == 0) {
					return Constant.NaN;
				}
				else {
					return a.Max();
				}
			});

		public static readonly FunctionCore Sum = new FunctionCore("sum",
			multiFunsGroup,
			"Returns the sum of all given arguments. Returns 0 if no argument is given.",
			FunctionCore.AnyParamsCount,
			new ExpressionValueTypeFlags[] { ExpressionValueTypeFlags.Constant },
			(a) => {
				return a.Aggregate(0.0, (acc, val) => acc + (Constant)val).ToConst();
			});

		public static readonly FunctionCore Product = new FunctionCore("product",
			multiFunsGroup,
			"Returns the product of all given arguments. Returns 1 if no argument is given.",
			FunctionCore.AnyParamsCount,
			new ExpressionValueTypeFlags[] { ExpressionValueTypeFlags.Constant },
			(a) => {
				return a.Aggregate(1.0, (acc, val) => acc * (Constant)val).ToConst();
			});

		public static readonly FunctionCore Prod = Product.ChangeNameTo("prod");

		public static readonly FunctionCore Average = new FunctionCore("average",
			multiFunsGroup,
			"Returns the average of all given arguments. Returns NaN (not a number) if no argument is given.",
			FunctionCore.AnyParamsCount,
			new ExpressionValueTypeFlags[] { ExpressionValueTypeFlags.Constant },
			(a) => {
				if (a.ArgsCount == 0) {
					return Constant.NaN;
				}
				else {
					return (a.Aggregate(0.0, (acc, val) => acc + (Constant)val) / a.ArgsCount).ToConst();
				}
			});

		public static readonly FunctionCore Avg = Average.ChangeNameTo("avg");


		#endregion


		#region Trigonometric functions -- sin, cos, tan, asin, acos, atan, atan2

		private const string trigFunsGroup = "Trigonometric functions";

		public static readonly FunctionCore Sin = new FunctionCore("sin",
			trigFunsGroup,
			"Returns the sine of the specified angle in radians.",
			1,
			new ExpressionValueTypeFlags[] { ExpressionValueTypeFlags.Constant },
			(a) => {
				return Math.Sin((Constant)a[0]).ToConst();
			});

		public static readonly FunctionCore Cos = new FunctionCore("cos",
			trigFunsGroup,
			"Returns the cosine of the specified angle in radians.",
			1,
			new ExpressionValueTypeFlags[] { ExpressionValueTypeFlags.Constant },
			(a) => {
				return Math.Cos((Constant)a[0]).ToConst();
			});

		public static readonly FunctionCore Tan = new FunctionCore("tan",
			trigFunsGroup,
			"Returns the tangent of the specified angle in radians.",
			1,
			new ExpressionValueTypeFlags[] { ExpressionValueTypeFlags.Constant },
			(a) => {
				return Math.Tan((Constant)a[0]).ToConst();
			});

		public static readonly FunctionCore Asin = new FunctionCore("asin",
			trigFunsGroup,
			"Returns the angle in radians whose sine is the given number.",
			1,
			new ExpressionValueTypeFlags[] { ExpressionValueTypeFlags.Constant },
			(a) => {
				return Math.Asin((Constant)a[0]).ToConst();
			});

		public static readonly FunctionCore Acos = new FunctionCore("acos",
			trigFunsGroup,
			"Returns the angle in radians whose cosine is the given number.",
			1,
			new ExpressionValueTypeFlags[] { ExpressionValueTypeFlags.Constant },
			(a) => {
				return Math.Acos((Constant)a[0]).ToConst();
			});

		public static readonly FunctionCore Atan = new FunctionCore("atan",
			trigFunsGroup,
			"Returns the angle in radians whose tangent is the given number.",
			1,
			new ExpressionValueTypeFlags[] { ExpressionValueTypeFlags.Constant },
			(a) => {
				return Math.Atan((Constant)a[0]).ToConst();
			});

		public static readonly FunctionCore Atan2 = new FunctionCore("atan2",
			trigFunsGroup,
			"Returns the angle in radians whose tangent is the quotient of two given numbers.",
			2,
			new ExpressionValueTypeFlags[] { ExpressionValueTypeFlags.Constant, ExpressionValueTypeFlags.Constant },
			(a) => {
				return Math.Atan2((Constant)a[0], (Constant)a[1]).ToConst();
			});

		#endregion


		#region Array functions -- length, min, max, sum, product, average

		private const string arrFunsGroup = "Array functions";

		public static readonly FunctionCore Length = new FunctionCore("length",
			arrFunsGroup,
			"Returns a value that represents the total number of elements in given array. Returns -1 if argument is not an array.",
			1,
			new ExpressionValueTypeFlags[] { ExpressionValueTypeFlags.Any },
			(a) => {
				if (a[0].IsArray) {
					return ((ValuesArray)a[0]).Length.ToConst();
				}
				else {
					return Constant.MinusOne;
				}
			});

		public static readonly FunctionCore MinArr = new FunctionCore("min",
			arrFunsGroup,
			"Returns the minimum value from given array.",
			1,
			new ExpressionValueTypeFlags[] { ExpressionValueTypeFlags.Array },
			(a) => { return ((ValuesArray)a[0]).Min(); });

		public static readonly FunctionCore MaxArr = new FunctionCore("max",
			arrFunsGroup,
			"Returns the maximum value from given array",
			1,
			new ExpressionValueTypeFlags[] { ExpressionValueTypeFlags.Array },
			(a) => { return ((ValuesArray)a[0]).Max(); });


		public static readonly FunctionCore Len = Length.ChangeNameTo("len");

		public static readonly FunctionCore SumArr = new FunctionCore("sum",
			arrFunsGroup,
			"Returns the sum of all numbers in given array. Sum of an empty array is 0.",
			1,
			new ExpressionValueTypeFlags[] { ExpressionValueTypeFlags.Array },
			(a) => {
				return ((ValuesArray)a[0]).Aggregate(0.0, (acc, val) => acc + (Constant)val).ToConst();
			});

		public static readonly FunctionCore ProductArr = new FunctionCore("product",
			arrFunsGroup,
			"Returns the product of all numbers in given array. Product of an empty array is 1.",
			1,
			new ExpressionValueTypeFlags[] { ExpressionValueTypeFlags.Array },
			(a) => {
				return ((ValuesArray)a[0]).Aggregate(1.0, (acc, val) => acc * (Constant)val).ToConst();
			});

		public static readonly FunctionCore ProdArr = ProductArr.ChangeNameTo("prod");

		public static readonly FunctionCore AverageArr = new FunctionCore("average",
			arrFunsGroup,
			"Returns the average value from all numbers in given array.",
			1,
			new ExpressionValueTypeFlags[] { ExpressionValueTypeFlags.Array },
			(a) => {
				var arr = ((ValuesArray)a[0]);
				return (arr.Aggregate(0.0, (acc, val) => acc + (Constant)val) / arr.Length).ToConst();
			});

		public static readonly FunctionCore AvgArr = AverageArr.ChangeNameTo("avg");

		#endregion


		#region Special functions -- isNan, isInfinity, if

		private const string specFunsGroup = "Special functions";

		public static readonly FunctionCore IsNan = new FunctionCore("isNan",
			specFunsGroup,
			"Returns a value indicating whether the given number is NaN (not a number).",
			1,
			new ExpressionValueTypeFlags[] { ExpressionValueTypeFlags.Any },
			(a) => {
				return a[0].IsNaN ? Constant.True : Constant.False;
			});

		public static readonly FunctionCore IsInfinity = new FunctionCore("isInfinity",
			specFunsGroup,
			"Returns a value indicating whether the specified number is negative or positive infinity.",
			1,
			new ExpressionValueTypeFlags[] { ExpressionValueTypeFlags.Any },
			(a) => {
				if (a[0].IsConstant) {
					return ((Constant)a[0]).IsInfinity ? Constant.True : Constant.False;
				}
				else {
					return Constant.False;
				}
			});

		public static readonly FunctionCore IsInfty = IsInfinity.ChangeNameTo("isInfty");

		/// <summary>
		/// This is dummy function for IF with no body. Implementation have to be in expression evaluator.
		/// </summary>
		/// <remarks>
		/// Function IF can not be directly written like others functions, because its 2nd and 3rd arguments can not be
		/// evaluated till decision from 1st is made. Recursion is the problem. If recursive call is in 2nd or 3rd
		/// argument and it is tried to evaluate, stack overflow exception will obviously occur.
		/// </remarks>
		public static readonly FunctionCore If = new FunctionCore("if",
			specFunsGroup,
			"If the first given value is non-zero, returns second value, otherwise returns third value.",
			3,
			new ExpressionValueTypeFlags[] { ExpressionValueTypeFlags.Constant, ExpressionValueTypeFlags.Any, ExpressionValueTypeFlags.Any },
			(a) => { throw new InvalidOperationException(); });

		#endregion


		#region Other functions -- abs, sign, deg2Rad, rad2deg, toColorGradient

		private const string othFunsGroup = "Other functions";

		public static readonly FunctionCore Abs = new FunctionCore("abs",
			othFunsGroup,
			"Returns the absolute value of given number.",
			1,
			new ExpressionValueTypeFlags[] { ExpressionValueTypeFlags.Constant },
			(a) => {
				return Math.Abs((Constant)a[0]).ToConst();
			});

		public static readonly FunctionCore Sign = new FunctionCore("sign",
			othFunsGroup,
			"Returns a value indicating the sign of given number. Returns value -1 if given number is less than zero, "
				+ "0 if is equal to zero and 1 if number is greater than zero.",
			1,
			new ExpressionValueTypeFlags[] { ExpressionValueTypeFlags.Constant },
			(a) => {
				if (a[0].IsNaN) { return Constant.NaN; }
				return Math.Sign((Constant)a[0]).ToConst();
			});

		public static readonly FunctionCore Deg2Rad = new FunctionCore("deg2rad",
			othFunsGroup,
			"Converts degrees to radians.",
			1,
			new ExpressionValueTypeFlags[] { ExpressionValueTypeFlags.Constant },
			(a) => {
				return ((Constant)a[0] * (Math.PI / 180)).ToConst();
			});

		public static readonly FunctionCore Rad2Deg = new FunctionCore("rad2deg",
			othFunsGroup,
			"Converts radians to degrees.",
			1,
			new ExpressionValueTypeFlags[] { ExpressionValueTypeFlags.Constant },
			(a) => {
				return ((Constant)a[0] * (180 / Math.PI)).ToConst();
			});

		public static readonly FunctionCore ToColorGradient = new FunctionCore("toColorGradient",
			othFunsGroup,
			"Converts array represented as color gradient to array of colors.",
			1,
			new ExpressionValueTypeFlags[] { ExpressionValueTypeFlags.Array },
			(a) => {
				var logger = new MessageLogger();  // TODO: use dummy logger
				var gFac = new Malsys.Media.ColorGradientFactory();
				var gradient = gFac.CreateFromValuesArray((ValuesArray)a[0], logger);
				return gFac.ToValuesArray(gradient);
			});


		#endregion

	}
}

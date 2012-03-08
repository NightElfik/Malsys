using System;
using System.Linq;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;
using Malsys.Evaluators;

namespace Malsys.Resources {
	public static class StdFunctions {


		#region Mathematical functions -- sqrt, factorial, log, log10

		/// <summary>
		/// Returns the square root of a given number.
		/// </summary>
		/// <docGroup>Mathematical functions</docGroup>
		[Alias("sqrt")]
		public static readonly FunctionCore Sqrt = new FunctionCore(1,
			FunctionInfo.ConstantParamsTypes,
			(a, e) => Math.Sqrt((Constant)a[0]).ToConst()
		);

		/// <summary>
		/// Returns the product of all positive integers less than or equal to given number.
		/// </summary>
		/// <docGroup>Mathematical functions</docGroup>
		[Alias("factorial")]
		public static readonly FunctionCore Factorial = new FunctionCore(1,
			FunctionInfo.ConstantParamsTypes,
			(a, e) => {
				if (a[0].IsNaN) {
					return Constant.NaN;
				}
				double result = 1.0;
				double max = (Constant)a[0];
				for (int i = 2; i <= max; i++) {
					result *= i;
				}
				return result.ToConst();
			}
		);

		/// <summary>
		/// Returns the natural (base e) logarithm of a given number.
		/// </summary>
		/// <docGroup>Mathematical functions</docGroup>
		[Alias("log")]
		public static readonly FunctionCore Log = new FunctionCore(1,
			FunctionInfo.ConstantParamsTypes,
			(a, e) => Math.Log((Constant)a[0]).ToConst()
		);

		/// <summary>
		/// Returns the base 10 logarithm of a given number.
		/// </summary>
		/// <docGroup>Mathematical functions</docGroup>
		[Alias("log10")]
		public static readonly FunctionCore Log10 = new FunctionCore(1,
			FunctionInfo.ConstantParamsTypes,
			(a, e) => Math.Log10((Constant)a[0]).ToConst()
		);

		/// <summary>
		/// Returns the logarithm of a given number in a given base.
		/// </summary>
		/// <docGroup>Mathematical functions</docGroup>
		[Alias("logBase", "Log", "log")]
		public static readonly FunctionCore LogBase = new FunctionCore(2,
			FunctionInfo.ConstantParamsTypes,
			(a, e) => Math.Log((Constant)a[0], (Constant)a[1]).ToConst()
		);

		#endregion


		#region Rounding functions -- round, floor, ceiling

		/// <summary>
		/// Rounds given number to the nearest integral value.
		/// If the fractional component is halfway between two integers, then the even number is returned.
		/// </summary>
		/// <docGroup>Rounding functions</docGroup>
		[Alias("round")]
		public static readonly FunctionCore Round = new FunctionCore(1,
			FunctionInfo.ConstantParamsTypes,
			(a, e) => Math.Round((Constant)a[0]).ToConst()
		);

		/// <summary>
		/// Returns the largest integer less than or equal to the given number.
		/// </summary>
		/// <docGroup>Rounding functions</docGroup>
		[Alias("floor")]
		public static readonly FunctionCore Floor = new FunctionCore(1,
			FunctionInfo.ConstantParamsTypes,
			(a, e) => Math.Floor((Constant)a[0]).ToConst()
		);

		/// <summary>
		/// Returns the smallest integer greater than or equal to the given number.
		/// </summary>
		/// <docGroup>Rounding functions</docGroup>
		[Alias("ceiling")]
		public static readonly FunctionCore Ceiling = new FunctionCore(1,
			FunctionInfo.ConstantParamsTypes,
			(a, e) => Math.Ceiling((Constant)a[0]).ToConst()
		);

		#endregion


		#region Multi-values functions -- min, max, sum, product, average

		/// <summary>
		/// Returns the minimum from all given arguments. Returns NaN (not a number) if no argument is given.
		/// </summary>
		/// <docGroup>Multi-values functions</docGroup>
		[Alias("min")]
		public static readonly FunctionCore Min = new FunctionCore(FunctionInfo.AnyParamsCount,
			FunctionInfo.ConstantParamsTypes,
			(a, e) => a.Length == 0 ? Constant.NaN : a.Min());

		/// <summary>
		/// Returns the average from all given arguments. Returns NaN (not a number) if no argument is given.
		/// </summary>
		/// <docGroup>Multi-values functions</docGroup>
		[Alias("max")]
		public static readonly FunctionCore Max = new FunctionCore(FunctionInfo.AnyParamsCount,
			FunctionInfo.ConstantParamsTypes,
			(a, e) => a.Length == 0 ? Constant.NaN : a.Max());

		/// <summary>
		/// Returns the sum of all given arguments. Returns 0 if no argument is given.
		/// </summary>
		/// <docGroup>Multi-values functions</docGroup>
		[Alias("sum")]
		public static readonly FunctionCore Sum = new FunctionCore(FunctionInfo.AnyParamsCount,
			FunctionInfo.ConstantParamsTypes,
			(a, e) => a.Aggregate(0.0, (acc, val) => acc + (Constant)val).ToConst()
		);

		/// <summary>
		/// Returns the product of all given arguments. Returns 1 if no argument is given.
		/// </summary>
		/// <docGroup>Multi-values functions</docGroup>
		[Alias("product")]
		public static readonly FunctionCore Product = new FunctionCore(FunctionInfo.AnyParamsCount,
			FunctionInfo.ConstantParamsTypes,
			(a, e) => a.Aggregate(1.0, (acc, val) => acc * (Constant)val).ToConst()
		);

		/// <summary>
		/// Returns the average of all given arguments. Returns NaN (not a number) if no argument is given.
		/// </summary>
		/// <docGroup>Multi-values functions</docGroup>
		[Alias("average")]
		public static readonly FunctionCore Average = new FunctionCore(FunctionInfo.AnyParamsCount,
			FunctionInfo.ConstantParamsTypes,
			(a, e) => a.Length == 0 ? Constant.NaN : (a.Aggregate(0.0, (acc, val) => acc + (Constant)val) / a.Length).ToConst()
		);


		#endregion


		#region Trigonometric functions -- sin, cos, tan, asin, acos, atan, atan2

		/// <summary>
		/// Returns the sine of the specified angle in radians.
		/// </summary>
		/// <docGroup>Trigonometric functions</docGroup>
		[Alias("sin")]
		public static readonly FunctionCore Sin = new FunctionCore(1,
			FunctionInfo.ConstantParamsTypes,
			(a, e) => Math.Sin((Constant)a[0]).ToConst()
		);

		/// <summary>
		/// Returns the cosine of the specified angle in radians.
		/// </summary>
		/// <docGroup>Trigonometric functions</docGroup>
		[Alias("cos")]
		public static readonly FunctionCore Cos = new FunctionCore(1,
			FunctionInfo.ConstantParamsTypes,
			(a, e) => Math.Cos((Constant)a[0]).ToConst()
		);

		/// <summary>
		/// Returns the tangent of the specified angle in radians.
		/// </summary>
		/// <docGroup>Trigonometric functions</docGroup>
		[Alias("tan")]
		public static readonly FunctionCore Tan = new FunctionCore(1,
			FunctionInfo.ConstantParamsTypes,
			(a, e) => Math.Tan((Constant)a[0]).ToConst()
		);

		/// <summary>
		/// Returns the angle in radians whose sine is the given number.
		/// </summary>
		/// <docGroup>Trigonometric functions</docGroup>
		[Alias("asin")]
		public static readonly FunctionCore Asin = new FunctionCore(1,
			FunctionInfo.ConstantParamsTypes,
			(a, e) => Math.Asin((Constant)a[0]).ToConst()
		);

		/// <summary>
		/// Returns the angle in radians whose cosine is the given number.
		/// </summary>
		/// <docGroup>Trigonometric functions</docGroup>
		[Alias("acos")]
		public static readonly FunctionCore Acos = new FunctionCore(1,
			FunctionInfo.ConstantParamsTypes,
			(a, e) => Math.Acos((Constant)a[0]).ToConst()
		);

		/// <summary>
		/// Returns the angle in radians whose tangent is the given number.
		/// </summary>
		/// <docGroup>Trigonometric functions</docGroup>
		[Alias("atan")]
		public static readonly FunctionCore Atan = new FunctionCore(1,
			FunctionInfo.ConstantParamsTypes,
			(a, e) => Math.Atan((Constant)a[0]).ToConst()
		);

		/// <summary>
		/// Returns the angle in radians whose tangent is the quotient of two given numbers.
		/// </summary>
		/// <docGroup>Trigonometric functions</docGroup>
		[Alias("atan2")]
		public static readonly FunctionCore Atan2 = new FunctionCore(2,
			FunctionInfo.ConstantParamsTypes,
			(a, e) => Math.Atan2((Constant)a[0], (Constant)a[1]).ToConst()
		);

		#endregion


		#region Array functions -- length, min, max, sum, product, average

		private const string arrFunsGroup = "Array functions";
		/// <summary>
		/// Returns a value that represents the total number of elements in given array. Returns -1 if argument is not an array.
		/// </summary>
		/// <docGroup>Array functions</docGroup>
		[Alias("length")]
		public static readonly FunctionCore Length = new FunctionCore(1,
			FunctionInfo.AnyParamsTypes,
			(a, e) => a[0].IsArray ? ((ValuesArray)a[0]).Length.ToConst() : Constant.MinusOne
		);

		/// <summary>
		/// Returns the minimum value from given array.
		/// </summary>
		/// <docGroup>Array functions</docGroup>
		[Alias(true, "Min", "min")]
		public static readonly FunctionCore MinArr = new FunctionCore(1,
			FunctionInfo.ArrayParamsTypes,
			(a, e) => ((ValuesArray)a[0]).Min()
		);

		/// <summary>
		/// Returns the maximum value from given array
		/// </summary>
		/// <docGroup>Array functions</docGroup>
		[Alias(true, "Max", "max")]
		public static readonly FunctionCore MaxArr = new FunctionCore(1,
			FunctionInfo.ArrayParamsTypes,
			(a, e) => ((ValuesArray)a[0]).Max()
		);


		/// <summary>
		/// Returns the sum of all numbers in given array. Sum of an empty array is 0.
		/// </summary>
		/// <docGroup>Array functions</docGroup>
		[Alias(true, "Sum", "sum")]
		public static readonly FunctionCore SumArr = new FunctionCore(1,
			FunctionInfo.ArrayParamsTypes,
			(a, e) => ((ValuesArray)a[0]).Aggregate(0.0, (acc, val) => acc + (Constant)val).ToConst()
		);

		/// <summary>
		/// Returns the product of all numbers in given array. Product of an empty array is 1.
		/// </summary>
		/// <docGroup>Array functions</docGroup>
		[Alias(true, "Product", "product")]
		public static readonly FunctionCore ProductArr = new FunctionCore(1,
			FunctionInfo.ArrayParamsTypes,
			(a, e) => ((ValuesArray)a[0]).Aggregate(1.0, (acc, val) => acc * (Constant)val).ToConst()
		);

		/// <summary>
		/// Returns the average value from all numbers in given array.
		/// </summary>
		/// <docGroup>Array functions</docGroup>
		[Alias(true, "Average", "average")]
		public static readonly FunctionCore AverageArr = new FunctionCore(1,
			FunctionInfo.ArrayParamsTypes,
			(a, e) => {
				var arr = ((ValuesArray)a[0]);
				return (arr.Aggregate(0.0, (acc, val) => acc + (Constant)val) / arr.Length).ToConst();
			});

		#endregion


		#region Special functions -- isNan, isInfinity, if

		/// <summary>
		/// Returns a value indicating whether the given number is NaN (not a number).
		/// </summary>
		/// <docGroup>Special functions</docGroup>
		[Alias("isNan", "isnan")]
		public static readonly FunctionCore IsNan = new FunctionCore(1,
			FunctionInfo.AnyParamsTypes,
			(a, e) => a[0].IsNaN ? Constant.True : Constant.False
		);

		/// <summary>
		/// Returns a value indicating whether the specified number is negative or positive infinity.
		/// </summary>
		/// <docGroup>Special functions</docGroup>
		[Alias("isInfinity", "isinfinity")]
		public static readonly FunctionCore IsInfinity = new FunctionCore(1,
			FunctionInfo.AnyParamsTypes,
			(a, e) => a[0].IsConstant
				? (((Constant)a[0]).IsInfinity ? Constant.True : Constant.False)
				: Constant.False
		);

		/// <summary>
		/// If the first given value is non-zero, returns second value, otherwise returns third value.
		/// </summary>
		/// <remarks>
		/// This is dummy function for IF with no body. Implementation have to be in expression evaluator.
		///
		/// Function IF can not be directly written like others functions, because its 2nd and 3rd arguments can not be
		/// evaluated till decision from 1st is made. Recursion is the problem. If recursive call is in 2nd or 3rd
		/// argument and it is tried to evaluate, stack overflow exception will obviously occur.
		/// </remarks>
		/// <docGroup>Special functions</docGroup>
		[Alias("if")]
		public static readonly FunctionCore If = new FunctionCore(3,
			new ImmutableList<ExpressionValueTypeFlags>(ExpressionValueTypeFlags.Constant, ExpressionValueTypeFlags.Any, ExpressionValueTypeFlags.Any),
			(a, e) => { throw new InvalidOperationException(); });

		#endregion


		#region Other functions -- abs, sign, deg2Rad, rad2deg, toColorGradient

		/// <summary>
		/// Returns the absolute value of given number.
		/// </summary>
		/// <docGroup>Other functions</docGroup>
		[Alias("abs")]
		public static readonly FunctionCore Abs = new FunctionCore(1,
			FunctionInfo.ConstantParamsTypes,
			(a, e) => Math.Abs((Constant)a[0]).ToConst()
		);

		/// <summary>
		/// Returns a value indicating the sign of given number.
		/// Returns value -1 if given number is less than zero, 0 if is equal to zero and 1 if number is greater than zero.
		/// </summary>
		/// <docGroup>Other functions</docGroup>
		[Alias("sign")]
		public static readonly FunctionCore Sign = new FunctionCore(1,
			FunctionInfo.ConstantParamsTypes,
			(a, e) => a[0].IsNaN ? Constant.NaN : Math.Sign((Constant)a[0]).ToConst()
		);

		/// <summary>
		/// Converts degrees to radians.
		/// </summary>
		/// <docGroup>Other functions</docGroup>
		[Alias("deg2rad")]
		public static readonly FunctionCore Deg2Rad = new FunctionCore(1,
			FunctionInfo.ConstantParamsTypes,
			(a, e) => ((Constant)a[0] * (Math.PI / 180)).ToConst()
		);

		/// <summary>
		/// Converts radians to degrees.
		/// </summary>
		/// <docGroup>Other functions</docGroup>
		[Alias("rad2deg")]
		public static readonly FunctionCore Rad2Deg = new FunctionCore(1,
			FunctionInfo.ConstantParamsTypes,
			(a, e) => ((Constant)a[0] * (180 / Math.PI)).ToConst()
		);

		/// <summary>
		/// Converts array represented as color gradient to array of colors.
		/// </summary>
		/// <docGroup>Other functions</docGroup>
		[Alias("toColorGradient")]
		public static readonly FunctionCore ToColorGradient = new FunctionCore(1,
			FunctionInfo.ArrayParamsTypes,
			(a, e) => {
				var logger = new MessageLogger();  // TODO: use dummy logger
				var gFac = new Malsys.Media.ColorGradientFactory();
				var gradient = gFac.CreateFromValuesArray((ValuesArray)a[0], logger);
				return gFac.ToValuesArray(gradient);
			});


		#endregion

	}
}

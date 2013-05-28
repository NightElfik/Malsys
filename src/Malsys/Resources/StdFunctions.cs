// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System;
using System.Linq;
using Malsys.Evaluators;
using Malsys.Media;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Resources {
	[MalsysFunctions]
	public static class StdFunctions {

		private static ColorParser colorParser = new ColorParser(new DullMessageLogger());


		#region Mathematical functions -- sqrt, factorial, log, log10

		/// <summary>
		/// Returns the square root of a given number.
		/// </summary>
		/// <group>Mathematical functions</group>
		[AccessName("sqrt")]
		public static readonly FunctionCore Sqrt = new FunctionCore(1,
			FunctionInfo.ConstantParamsTypes,
			(a, e) => Math.Sqrt((Constant)a[0]).ToConst()
		);

		/// <summary>
		/// Returns the product of all positive integers less than or equal to given number.
		/// </summary>
		/// <group>Mathematical functions</group>
		[AccessName("factorial")]
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
		/// <group>Mathematical functions</group>
		[AccessName("log")]
		public static readonly FunctionCore Log = new FunctionCore(1,
			FunctionInfo.ConstantParamsTypes,
			(a, e) => Math.Log((Constant)a[0]).ToConst()
		);

		/// <summary>
		/// Returns the base 10 logarithm of a given number.
		/// </summary>
		/// <group>Mathematical functions</group>
		[AccessName("log10")]
		public static readonly FunctionCore Log10 = new FunctionCore(1,
			FunctionInfo.ConstantParamsTypes,
			(a, e) => Math.Log10((Constant)a[0]).ToConst()
		);

		/// <summary>
		/// Returns the logarithm of a given number in a given base.
		/// </summary>
		/// <group>Mathematical functions</group>
		[AccessName("logBase", "log")]
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
		/// <group>Rounding functions</group>
		[AccessName("round")]
		public static readonly FunctionCore Round = new FunctionCore(1,
			FunctionInfo.ConstantParamsTypes,
			(a, e) => Math.Round((Constant)a[0]).ToConst()
		);

		/// <summary>
		/// Returns the largest integer less than or equal to the given number.
		/// </summary>
		/// <group>Rounding functions</group>
		[AccessName("floor")]
		public static readonly FunctionCore Floor = new FunctionCore(1,
			FunctionInfo.ConstantParamsTypes,
			(a, e) => Math.Floor((Constant)a[0]).ToConst()
		);

		/// <summary>
		/// Returns the smallest integer greater than or equal to the given number.
		/// </summary>
		/// <group>Rounding functions</group>
		[AccessName("ceiling")]
		public static readonly FunctionCore Ceiling = new FunctionCore(1,
			FunctionInfo.ConstantParamsTypes,
			(a, e) => Math.Ceiling((Constant)a[0]).ToConst()
		);

		#endregion


		#region Multi-values functions -- min, max, sum, product, average

		/// <summary>
		/// Returns the minimum from all given arguments. Returns NaN (not a number) if no argument is given.
		/// </summary>
		/// <group>Multi-values functions</group>
		[AccessName("min")]
		public static readonly FunctionCore Min = new FunctionCore(FunctionInfo.AnyParamsCount,
			FunctionInfo.ConstantParamsTypes,
			(a, e) => a.Length == 0 ? Constant.NaN : a.Min());

		/// <summary>
		/// Returns the average from all given arguments. Returns NaN (not a number) if no argument is given.
		/// </summary>
		/// <group>Multi-values functions</group>
		[AccessName("max")]
		public static readonly FunctionCore Max = new FunctionCore(FunctionInfo.AnyParamsCount,
			FunctionInfo.ConstantParamsTypes,
			(a, e) => a.Length == 0 ? Constant.NaN : a.Max());

		/// <summary>
		/// Returns the sum of all given arguments. Returns 0 if no argument is given.
		/// </summary>
		/// <group>Multi-values functions</group>
		[AccessName("sum")]
		public static readonly FunctionCore Sum = new FunctionCore(FunctionInfo.AnyParamsCount,
			FunctionInfo.ConstantParamsTypes,
			(a, e) => a.Aggregate(0.0, (acc, val) => acc + (Constant)val).ToConst()
		);

		/// <summary>
		/// Returns the product of all given arguments. Returns 1 if no argument is given.
		/// </summary>
		/// <group>Multi-values functions</group>
		[AccessName("product")]
		public static readonly FunctionCore Product = new FunctionCore(FunctionInfo.AnyParamsCount,
			FunctionInfo.ConstantParamsTypes,
			(a, e) => a.Aggregate(1.0, (acc, val) => acc * (Constant)val).ToConst()
		);

		/// <summary>
		/// Returns the average of all given arguments. Returns NaN (not a number) if no argument is given.
		/// </summary>
		/// <group>Multi-values functions</group>
		[AccessName("average")]
		public static readonly FunctionCore Average = new FunctionCore(FunctionInfo.AnyParamsCount,
			FunctionInfo.ConstantParamsTypes,
			(a, e) => a.Length == 0 ? Constant.NaN : (a.Aggregate(0.0, (acc, val) => acc + (Constant)val) / a.Length).ToConst()
		);


		#endregion


		#region Trigonometric functions -- sin, cos, tan, asin, acos, atan, atan2

		/// <summary>
		/// Returns the sine of the specified angle in radians.
		/// </summary>
		/// <group>Trigonometric functions</group>
		[AccessName("sin")]
		public static readonly FunctionCore Sin = new FunctionCore(1,
			FunctionInfo.ConstantParamsTypes,
			(a, e) => Math.Sin((Constant)a[0]).ToConst()
		);

		/// <summary>
		/// Returns the cosine of the specified angle in radians.
		/// </summary>
		/// <group>Trigonometric functions</group>
		[AccessName("cos")]
		public static readonly FunctionCore Cos = new FunctionCore(1,
			FunctionInfo.ConstantParamsTypes,
			(a, e) => Math.Cos((Constant)a[0]).ToConst()
		);

		/// <summary>
		/// Returns the tangent of the specified angle in radians.
		/// </summary>
		/// <group>Trigonometric functions</group>
		[AccessName("tan")]
		public static readonly FunctionCore Tan = new FunctionCore(1,
			FunctionInfo.ConstantParamsTypes,
			(a, e) => Math.Tan((Constant)a[0]).ToConst()
		);

		/// <summary>
		/// Returns the angle in radians whose sine is the given number.
		/// </summary>
		/// <group>Trigonometric functions</group>
		[AccessName("asin")]
		public static readonly FunctionCore Asin = new FunctionCore(1,
			FunctionInfo.ConstantParamsTypes,
			(a, e) => Math.Asin((Constant)a[0]).ToConst()
		);

		/// <summary>
		/// Returns the angle in radians whose cosine is the given number.
		/// </summary>
		/// <group>Trigonometric functions</group>
		[AccessName("acos")]
		public static readonly FunctionCore Acos = new FunctionCore(1,
			FunctionInfo.ConstantParamsTypes,
			(a, e) => Math.Acos((Constant)a[0]).ToConst()
		);

		/// <summary>
		/// Returns the angle in radians whose tangent is the given number.
		/// </summary>
		/// <group>Trigonometric functions</group>
		[AccessName("atan")]
		public static readonly FunctionCore Atan = new FunctionCore(1,
			FunctionInfo.ConstantParamsTypes,
			(a, e) => Math.Atan((Constant)a[0]).ToConst()
		);

		/// <summary>
		/// Returns the angle in radians whose tangent is the quotient of two given numbers.
		/// </summary>
		/// <group>Trigonometric functions</group>
		[AccessName("atan2")]
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
		/// <group>Array functions</group>
		[AccessName("length")]
		public static readonly FunctionCore Length = new FunctionCore(1,
			FunctionInfo.AnyParamsTypes,
			(a, e) => a[0].IsArray ? ((ValuesArray)a[0]).Length.ToConst() : Constant.MinusOne
		);

		/// <summary>
		/// Returns the minimum value from given array.
		/// </summary>
		/// <group>Array functions</group>
		[AccessName("min")]
		public static readonly FunctionCore MinArr = new FunctionCore(1,
			FunctionInfo.ArrayParamsTypes,
			(a, e) => ((ValuesArray)a[0]).Min()
		);

		/// <summary>
		/// Returns the maximum value from given array
		/// </summary>
		/// <group>Array functions</group>
		[AccessName("max")]
		public static readonly FunctionCore MaxArr = new FunctionCore(1,
			FunctionInfo.ArrayParamsTypes,
			(a, e) => ((ValuesArray)a[0]).Max()
		);


		/// <summary>
		/// Returns the sum of all numbers in given array. Sum of an empty array is 0.
		/// </summary>
		/// <group>Array functions</group>
		[AccessName("sum")]
		public static readonly FunctionCore SumArr = new FunctionCore(1,
			FunctionInfo.ArrayParamsTypes,
			(a, e) => ((ValuesArray)a[0]).Aggregate(0.0, (acc, val) => acc + (Constant)val).ToConst()
		);

		/// <summary>
		/// Returns the product of all numbers in given array. Product of an empty array is 1.
		/// </summary>
		/// <group>Array functions</group>
		[AccessName("product")]
		public static readonly FunctionCore ProductArr = new FunctionCore(1,
			FunctionInfo.ArrayParamsTypes,
			(a, e) => ((ValuesArray)a[0]).Aggregate(1.0, (acc, val) => acc * (Constant)val).ToConst()
		);

		/// <summary>
		/// Returns the average value from all numbers in given array.
		/// </summary>
		/// <group>Array functions</group>
		[AccessName("average")]
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
		/// <group>Special functions</group>
		[AccessName("isNan")]
		public static readonly FunctionCore IsNan = new FunctionCore(1,
			FunctionInfo.AnyParamsTypes,
			(a, e) => a[0].IsNaN ? Constant.True : Constant.False
		);

		/// <summary>
		/// Returns a value indicating whether the specified number is negative or positive infinity.
		/// </summary>
		/// <group>Special functions</group>
		[AccessName("isInfinity")]
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
		/// <group>Special functions</group>
		[AccessName("if")]
		public static readonly FunctionCore If = new FunctionCore(3,
			new ImmutableList<ExpressionValueTypeFlags>(ExpressionValueTypeFlags.Constant, ExpressionValueTypeFlags.Any, ExpressionValueTypeFlags.Any),
			(a, e) => { throw new InvalidOperationException(); });

		#endregion


		#region Other functions -- abs, sign, deg2rad, rad2deg, toColorGradient

		/// <summary>
		/// Returns the absolute value of given number.
		/// </summary>
		/// <group>Other functions</group>
		[AccessName("abs")]
		public static readonly FunctionCore Abs = new FunctionCore(1,
			FunctionInfo.ConstantParamsTypes,
			(a, e) => Math.Abs((Constant)a[0]).ToConst()
		);

		/// <summary>
		/// Returns a value indicating the sign of given number.
		/// Returns value -1 if given number is less than zero, 0 if is equal to zero and 1 if number is greater than zero.
		/// </summary>
		/// <group>Other functions</group>
		[AccessName("sign")]
		public static readonly FunctionCore Sign = new FunctionCore(1,
			FunctionInfo.ConstantParamsTypes,
			(a, e) => a[0].IsNaN ? Constant.NaN : Math.Sign((Constant)a[0]).ToConst()
		);

		/// <summary>
		/// Converts degrees to radians.
		/// </summary>
		/// <group>Other functions</group>
		[AccessName("deg2rad")]
		public static readonly FunctionCore Deg2Rad = new FunctionCore(1,
			FunctionInfo.ConstantParamsTypes,
			(a, e) => ((Constant)a[0] * (Math.PI / 180)).ToConst()
		);

		/// <summary>
		/// Converts radians to degrees.
		/// </summary>
		/// <group>Other functions</group>
		[AccessName("rad2deg")]
		public static readonly FunctionCore Rad2Deg = new FunctionCore(1,
			FunctionInfo.ConstantParamsTypes,
			(a, e) => ((Constant)a[0] * (180 / Math.PI)).ToConst()
		);

		/// <summary>
		/// Converts array representing color gradient to array of colors.
		/// </summary>
		/// <group>Other functions</group>
		[AccessName("toColorGradient")]
		public static readonly FunctionCore ToColorGradient = new FunctionCore(1,
			FunctionInfo.ArrayParamsTypes,
			(a, e) => {
				var logger = new DullMessageLogger();
				var gFac = new Malsys.Media.ColorGradientFactory();
				var gradient = gFac.CreateFromValuesArray((ValuesArray)a[0], logger);
				return gFac.ToValuesArray(gradient);
			});

		/// <summary>
		/// Converts arguments representing color gradient to array of colors.
		/// </summary>
		/// <group>Other functions</group>
		[AccessName("toColorGradient")]
		public static readonly FunctionCore ToColorGradientArr = new FunctionCore(FunctionInfo.AnyParamsCount,
			FunctionInfo.ConstantParamsTypes,
			(a, e) => {
				var logger = new DullMessageLogger();
				var gFac = new Malsys.Media.ColorGradientFactory();
				var gradient = gFac.CreateFromValuesArray(a, logger);
				return gFac.ToValuesArray(gradient);
			});


		#endregion


		#region Color functions -- lighten, darken

		/// <summary>
		/// Lightens given color by given amount of percent.
		/// Negative value causes darkening of color.
		/// </summary>
		/// <group>Color functions</group>
		[AccessName("lighten")]
		public static readonly FunctionCore Lighten = new FunctionCore(2,
			new ImmutableList<ExpressionValueTypeFlags>(ExpressionValueTypeFlags.Any, ExpressionValueTypeFlags.Constant),
			(a, e) => {
				ColorF clr;
				if (!colorParser.TryParseColor(a[0], out clr)) {
					throw new EvalException("Failed to convert first argument to color.");
				}

				double h, s, l;
				ColorHelper.ColorToHsl(clr, out h, out s, out l);
				l += ((Constant)a[1]).Value;
				l = MathHelper.Clamp01(l);
				ColorF resultColor = ColorHelper.HslToColor(h, s, l);
				resultColor.T = clr.T;  // preserve transparency

				return ColorHelper.ToIValue(resultColor);
			}
		);

		/// <summary>
		/// Darkens given color by given amount of percent.
		/// Negative value causes lightening of color.
		/// </summary>
		/// <group>Color functions</group>
		[AccessName("darken")]
		public static readonly FunctionCore Darken = new FunctionCore(2,
			new ImmutableList<ExpressionValueTypeFlags>(ExpressionValueTypeFlags.Any, ExpressionValueTypeFlags.Constant),
			(a, e) => {
				ColorF clr;
				if (!colorParser.TryParseColor(a[0], out clr)) {
					throw new EvalException("Failed to convert first argument to color.");
				}

				double h, s, l;
				ColorHelper.ColorToHsl(clr, out h, out s, out l);
				l -= ((Constant)a[1]).Value;
				l = MathHelper.Clamp01(l);
				ColorF resultColor = ColorHelper.HslToColor(h, s, l);
				resultColor.T = clr.T;  // preserve transparency

				return ColorHelper.ToIValue(resultColor);
			}
		);

		#endregion


	}
}

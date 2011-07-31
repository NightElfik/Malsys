using System;
using System.Diagnostics;

namespace Malsys.Expressions {
	static class FloatArithmeticHelper {
		public static readonly double Epsilon = 1e-8;

		public static bool IsZero(double value) {
			Debug.Assert(!double.IsNaN(value), "NaN value sould not reach IsZero test.");
			return Math.Abs(value) < Epsilon;
		}
	}
}

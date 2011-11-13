using System;
using System.Diagnostics;
using Malsys.SemanticModel;

namespace Malsys.Evaluators {
	public static class FloatArithmeticHelper {
		public static readonly double Epsilon = 1e-8;

		public static bool IsZero(double value) {
			Debug.Assert(!double.IsNaN(value), "NaN value sould not reach IsZero test.");
			return Math.Abs(value) < Epsilon;
		}

		public static int EpsilonCompareTo(this double value, double otherValue) {
			if (double.IsNaN(value) || double.IsNaN(otherValue) || double.IsInfinity(value) || double.IsInfinity(otherValue)) {
				return value.CompareTo(otherValue);
			}

			if (Math.Abs(value - otherValue) < Epsilon) {
				return 0;
			}
			else if (value < otherValue) {
				return -1;
			}
			else {
				Debug.Assert(value > otherValue);
				return 1;
			}
		}

		public static bool IsZero(this Constant value) {
			Debug.Assert(!value.IsNaN, "NaN value sould not reach IsZero test.");
			return Math.Abs(value.Value) < Epsilon;
		}
	}
}

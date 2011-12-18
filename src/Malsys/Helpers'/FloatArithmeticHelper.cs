using System;

namespace Malsys {
	public static class FloatArithmeticHelper {

		public static readonly double Epsilon = 1e-8;


		public static bool IsZero(double value) {
			return Math.Abs(value) < Epsilon;
		}

		public static int EpsilonCompareTo(this double value, double otherValue) {

			if (double.IsNaN(value) || double.IsNaN(otherValue) || double.IsInfinity(value) || double.IsInfinity(otherValue)) {
				return value.CompareTo(otherValue);
			}

			if (Math.Abs(value - otherValue) < Epsilon) {
				return 0;
			}
			else if (value > otherValue) {
				return 1;
			}
			else {
				return -1;
			}
		}
	}
}

using System;

namespace Malsys {
	public static class FloatArithmeticHelper {

		public static readonly double Epsilon = 1e-8;

		/// <summary>
		/// Given number is considered as zero if its absolute value is smaller than <c>Epsilon</c>.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool IsZero(double value) {
			return Math.Abs(value) < Epsilon;
		}

		/// <summary>
		/// Compares two doubles with respect to <c>Epsilon</c>.
		/// Given two numbers are considered as the same if the difference between them is smaller than <c>Epsilon</c>.
		/// </summary>
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

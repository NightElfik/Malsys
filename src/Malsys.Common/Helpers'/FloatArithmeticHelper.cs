using System;

namespace Malsys {

	public static class FloatArithmeticHelper {

		public const double DBL_EPSILON = 1.0E-07;
		public const float SGL_EPSILON = 1.0E-05f;


		public static bool IsAlmostZero(this double value) {
			return value < 10.0 * DBL_EPSILON && value > -10.0 * DBL_EPSILON;
		}

		public static bool IsAlmostEqualTo(this double value1, double value2) {

			// in case they are Infinities (then epsilon check does not work)
			if (value1 == value2) {
				return true;
			}

			// This computes (|value1-value2| / (|value1| + |value2| + 10.0)) < DBL_EPSILON
			double eps = (Math.Abs(value1) + Math.Abs(value2) + 10.0) * DBL_EPSILON;
			double delta = value1 - value2;
			return (-eps < delta) && (eps > delta);

		}

		public static bool IsAlmostZero(this float value) {
			return value < 10.0f * SGL_EPSILON && value > -10.0f * SGL_EPSILON;
		}

		public static bool IsAlmostEqualTo(this float value1, float value2) {

			// in case they are Infinities (then epsilon check does not work)
			if (value1 == value2) {
				return true;
			}

			// This computes (|value1-value2| / (|value1| + |value2| + 10.0)) < SGL_EPSILON
			double eps = (Math.Abs(value1) + Math.Abs(value2) + 10.0) * SGL_EPSILON;
			double delta = value1 - value2;
			return (-eps < delta) && (eps > delta);

		}

		public static bool IsEpsilonGreaterThanZero(this double value) {
			return value > 10 * DBL_EPSILON;
		}

		public static bool IsEpsilonGreaterThanZero(this float value) {
			return value > 10 * SGL_EPSILON;
		}

		public static bool IsEpsilonLessThan(this double value, double otherValue) {
			if (value.IsAlmostEqualTo(otherValue)) {
				return false;
			}
			else {
				return value < otherValue;
			}
		}

		public static bool IsEpsilonLessThan(this float value, float otherValue) {
			if (value.IsAlmostEqualTo(otherValue)) {
				return false;
			}
			else {
				return value < otherValue;
			}
		}

		public static int EpsilonCompareTo(this double value, double otherValue) {
			if (value.IsAlmostEqualTo(otherValue)) {
				return 0;
			}
			return value < otherValue ? -1 : 1;
		}

		public static int EpsilonCompareTo(this float value, float otherValue) {
			if (value.IsAlmostEqualTo(otherValue)) {
				return 0;
			}
			return value < otherValue ? -1 : 1;
		}


	}

}

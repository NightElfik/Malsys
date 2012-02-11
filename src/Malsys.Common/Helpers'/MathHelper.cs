using System;

namespace Malsys {
	public static class MathHelper {

		/// <summary>
		/// Constant for converting degrees to radians.
		/// </summary>
		public const double PiOver180 = Math.PI / 180.0;

		public const double PiHalf = Math.PI / 2;


		public static float Clamp(float value, float min, float max) {
			if (value < min) {
				return min;
			}
			else if (value > max) {
				return max;
			}
			else {
				return value;
			}
		}

		public static float Clamp01(float value) {
			if (value < 0) {
				return 0;
			}
			else if (value > 1) {
				return 1;
			}
			else {
				return value;
			}
		}



	}
}

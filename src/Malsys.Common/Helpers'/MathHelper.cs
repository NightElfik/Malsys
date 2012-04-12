using System;

namespace Malsys {
	public static class MathHelper {

		/// <summary>
		/// Constant for converting degrees to radians.
		/// </summary>
		public const double PiOver180 = Math.PI / 180.0;

		public const double PiHalf = Math.PI / 2;


		public static int Clamp(int value, int min, int max) {
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

		public static double Clamp(double value, double min, double max) {
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

		public static double Clamp01(double value) {
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

		public static void ScaleSizeToFit(ref int width, ref int height, int maxWidth, int maxHeight, bool shrinkOnly = false) {

			float wRatio = (float)width / maxWidth;
			float hRatio = (float)height / maxHeight;

			// which dimension is worst?
			if (wRatio > hRatio) {  // width is worst
				if (!shrinkOnly || wRatio > 1) {
					width = (int)Math.Round(width / wRatio);
					height = (int)Math.Round(height / wRatio);
				}
			}
			else {  // height is worst
				if (!shrinkOnly || hRatio > 1) {
					width = (int)Math.Round(width / hRatio);
					height = (int)Math.Round(height / hRatio);
				}
			}

		}



	}
}

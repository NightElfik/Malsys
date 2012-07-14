/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System;
using System.Diagnostics.Contracts;

namespace Malsys {
	public static class MathHelper {

		/// <summary>
		/// Constant for converting degrees to radians.
		/// </summary>
		public const double PiOver180 = Math.PI / 180.0;

		public const double PiHalf = Math.PI / 2;


		public static int Clamp(this int value, int min, int max) {
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

		public static float Clamp(this float value, float min, float max) {
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

		public static double Clamp(this double value, double min, double max) {
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

		public static float Clamp01(this float value) {
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

		public static double Clamp01(this double value) {
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

			double scale = GetScaleSizeToFitScale(width, height, maxWidth, maxHeight);

			width = (int)Math.Round(width * scale);
			height = (int)Math.Round(height * scale);
		}

		/// <summary>
		/// Returns scale which has to be applied to width and height to fit desired dimensions.
		/// </summary>
		public static double GetScaleSizeToFitScale(double width, double height, double maxWidth, double maxHeight, bool shrinkOnly = false) {

			Contract.Requires<ArgumentException>(width > 0);
			Contract.Requires<ArgumentException>(height > 0);
			Contract.Requires<ArgumentException>(maxWidth > 0);
			Contract.Requires<ArgumentException>(maxHeight > 0);
			Contract.Ensures(Contract.Result<double>() > 0);
			Contract.Ensures(shrinkOnly ? Contract.Result<double>() < 1 : true);

			double wScale = maxWidth / width;
			double hScale = maxHeight / height;

			// which scale is more restrictive?
			if (wScale <= hScale) {  // width is more restrictive
				if (!shrinkOnly || wScale < 1) {
					return wScale;
				}
			}
			else {  // height is more restrictive
				if (!shrinkOnly || hScale < 1) {
					return hScale;
				}
			}

			return 1d;
		}



	}
}

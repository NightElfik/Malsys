
using System;
namespace Malsys.Media {
	public struct ColorF {

		// for fast conversion to hexa
		private static readonly char[] hexDigits = {
			 '0', '1', '2', '3', '4', '5', '6', '7',
			 '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'
		};

		public static readonly ColorF Black = new ColorF(0, 0, 0);


		public float A;
		public float R;
		public float G;
		public float B;


		public ColorF(float r, float g, float b) {
			A = 1f;
			R = r;
			G = g;
			B = b;
		}

		public ColorF(float a, float r, float g, float b) {
			A = a;
			R = r;
			G = g;
			B = b;
		}

		public string ToRgbHexString() {

			long r = (long)(MathHelper.Clamp01(R) * 255);
			long g = (long)(MathHelper.Clamp01(G) * 255);
			long b = (long)(MathHelper.Clamp01(B) * 255);
			long clr = (r << 16) | (g << 8) | b;

			char[] result = new char[6];

			for (int i = 5; i >= 0; i--, clr >>= 4) {
				result[i] = hexDigits[clr & 0xf];
			}

			return new string(result);
		}

	}
}

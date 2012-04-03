using System;

namespace Malsys.Media {
	public struct ColorF {

		// for fast conversion to hex
		private static readonly char[] hexDigits = {
			 '0', '1', '2', '3', '4', '5', '6', '7',
			 '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'
		};

		public static readonly ColorF Black = new ColorF(0f, 0f, 0f);
		public static readonly ColorF White = new ColorF(1f, 1f, 1f);

		public static readonly ColorF Red = new ColorF(1f, 0f, 0f);
		public static readonly ColorF Yellow = new ColorF(1f, 1f, 0f);
		public static readonly ColorF Green = new ColorF(0f, 1f, 0f);
		public static readonly ColorF Cyan = new ColorF(0f, 1f, 1f);
		public static readonly ColorF Blue = new ColorF(0f, 0f, 1f);
		public static readonly ColorF Magenta = new ColorF(1f, 0f, 1f);

		public static readonly ColorF TransparentBlack = new ColorF(0f, 0f, 0f, 0f);


		public float A;
		public float R;
		public float G;
		public float B;


		public ColorF(uint argb) {
			A = ((argb >> 24) & 0xFF) / 255f;
			R = ((argb >> 16) & 0xFF) / 255f;
			G = ((argb >> 8) & 0xFF) / 255f;
			B = (argb & 0xFF) / 255f;
		}

		public ColorF(float r, float g, float b) {
			A = 1f;
			R = r;
			G = g;
			B = b;
		}

		public ColorF(double r, double g, double b) {
			A = 1f;
			R = (float)r;
			G = (float)g;
			B = (float)b;
		}

		public ColorF(float a, float r, float g, float b) {
			A = a;
			R = r;
			G = g;
			B = b;
		}

		public ColorF(double a, double r, double g, double b) {
			A = (float)a;
			R = (float)r;
			G = (float)g;
			B = (float)b;
		}

		public uint ToRgb() {

			uint r = (uint)(MathHelper.Clamp01(R) * 255);
			uint g = (uint)(MathHelper.Clamp01(G) * 255);
			uint b = (uint)(MathHelper.Clamp01(B) * 255);

			return (r << 16) | (g << 8) | b;
		}

		public uint ToArgb() {

			uint a = (uint)(MathHelper.Clamp01(A) * 255);
			uint r = (uint)(MathHelper.Clamp01(R) * 255);
			uint g = (uint)(MathHelper.Clamp01(G) * 255);
			uint b = (uint)(MathHelper.Clamp01(B) * 255);

			return (a << 24) | (r << 16) | (g << 8) | b;
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

		public override string ToString() {
			return "{0},{1},{2},{3}".FmtInvariant(A, R, G, B);
		}

	}
}

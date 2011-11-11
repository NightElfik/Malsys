
using System;
namespace Malsys.Media {
	public struct ColorF {

		public static readonly ColorF Black = new ColorF();


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

		public string ToArgbHexString() {

			char[] result = new char[8];
			long a = (long)(MathHelper.Clamp01(A) * 255);
			long r = (long)(MathHelper.Clamp01(R) * 255);
			long g = (long)(MathHelper.Clamp01(G) * 255);
			long b = (long)(MathHelper.Clamp01(B) * 255);
			long clr = (a << 24) | (r << 16) | (g << 8) | b;

			return Convert.ToString(clr, 16);
		}

	}
}

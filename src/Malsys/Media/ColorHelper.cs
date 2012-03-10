using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;
using System;

namespace Malsys.Media {
	public static class ColorHelper {

		public static bool IsColor(IValue value) {

			if (value.IsConstant) {
				var c = ((Constant)value);
				if (c.IsNaN) {
					return false;
				}
				long val = c.RoundedLongValue;
				if (val < 0 || val > 0xFFFFFF) {
					return false;
				}
			}
			else if (value.IsArray) {
				var arr = (ValuesArray)value;

				if (arr.Length != 3 && arr.Length != 4) {
					return false;
				}

				if (!arr.IsConstArray()) {
					return false;
				}
			}

			return true;
		}

		public static bool TryParseColor(IValue value, out ColorF color) {

			if (IsColor(value)) {
				color = toColor(value);
				return true;
			}
			else {
				color = ColorF.Black;
				return false;
			}
		}

		public static void ParseColor(IValue value, ref ColorF color) {

			if (IsColor(value)) {
				color = toColor(value);
			}

		}

		public static ColorF FromIValue(IValue value, IMessageLogger logger) {

			if (value.IsConstant) {
				long val = ((Constant)value).RoundedLongValue;
				if (val < 0 || val > 0xFFFFFF) {
					logger.LogMessage(Message.ColorConnstantOutOfRrange, value.AstPosition);
				}
			}
			else if (value.IsArray) {

				var arr = (ValuesArray)value;

				if (arr.Length != 3 && arr.Length != 4) {
					logger.LogMessage(Message.ExpectedColorAsArrayLen34, arr.AstPosition);
					return ColorF.Black;
				}

				for (int i = 0; i < arr.Length; i++) {
					if (!arr[i].IsConstant) {
						logger.LogMessage(Message.ExpectedConstAtIndexI, value.AstPosition, i);
						return ColorF.Black;
					}
				}
			}
			else {
				logger.LogMessage(Message.UnknownValue, value.AstPosition);
				return ColorF.Black;
			}

			return toColor(value);
		}

		public static ValuesArray ToIValue(ColorF color) {

			IValue[] arr = new IValue[4];
			arr[0] = color.A.ToConst();
			arr[1] = color.R.ToConst();
			arr[2] = color.G.ToConst();
			arr[3] = color.B.ToConst();

			return new ValuesArray(new ImmutableList<IValue>(arr, true));
		}

		/// <summary>
		/// Converts RGB color to HSL (hue, saturation, lightness).
		/// </summary>
		/// <remarks>
		/// http://www.geekymonkey.com/Programming/CSharp/RGB2HSL_HSL2RGB.htm
		/// </remarks>
		public static void ColorToHsl(ColorF c, out double h, out double s, out double l) {

			double r = c.R;
			double g = c.G;
			double b = c.B;

			double v;
			double m;
			double vm;
			double r2, g2, b2;

			h = 0; // default to black
			s = 0;
			l = 0;
			v = Math.Max(r, g);
			v = Math.Max(v, b);
			m = Math.Min(r, g);
			m = Math.Min(m, b);
			l = (m + v) / 2.0;

			if (l <= 0.0) {
				return;
			}

			vm = v - m;
			s = vm;
			if (s > 0.0) {
				s /= (l <= 0.5) ? (v + m) : (2.0 - v - m);
			}
			else {
				return;
			}

			r2 = (v - r) / vm;
			g2 = (v - g) / vm;
			b2 = (v - b) / vm;

			if (r == v) {
				h = (g == m ? 5.0 + b2 : 1.0 - g2);
			}
			else if (g == v) {
				h = (b == m ? 1.0 + r2 : 3.0 - b2);
			}
			else {
				h = (r == m ? 3.0 + g2 : 5.0 - r2);
			}
			h /= 6.0;

		}

		/// <summary>
		/// Converts HSL (hue, saturation, lightness) to color.
		/// </summary>
		/// <remarks>
		/// http://www.geekymonkey.com/Programming/CSharp/RGB2HSL_HSL2RGB.htm
		/// </remarks>
		public static ColorF HslToColor(double h, double s, double l) {

			double v;
			double r, g, b;

			r = l;   // default to gray
			g = l;
			b = l;
			v = (l <= 0.5) ? (l * (1.0 + s)) : (l + s - l * s);

			if (v > 0) {

				double m;
				double sv;
				int sextant;
				double fract, vsf, mid1, mid2;

				m = l + l - v;
				sv = (v - m) / v;
				h *= 6.0;
				sextant = (int)h;
				fract = h - sextant;
				vsf = v * sv * fract;
				mid1 = m + vsf;
				mid2 = v - vsf;

				switch (sextant) {
					case 0:
					case 6:
						r = v;
						g = mid1;
						b = m;
						break;
					case 1:
						r = mid2;
						g = v;
						b = m;
						break;
					case 2:
						r = m;
						g = v;
						b = mid1;
						break;
					case 3:
						r = m;
						g = mid2;
						b = v;
						break;
					case 4:
						r = mid1;
						g = m;
						b = v;
						break;
					case 5:
						r = v;
						g = m;
						b = mid2;
						break;
				}

			}

			return new ColorF(r, g, b);
		}


		private static ColorF toColor(IValue value) {

			if (value.IsConstant) {
				return new ColorF((uint)((Constant)value).RoundedLongValue);
			}
			else {
				ValuesArray arr = (ValuesArray)value;
				if (arr.Length == 3) {
					return new ColorF(((Constant)arr[0]).Value, ((Constant)arr[1]).Value, ((Constant)arr[2]).Value);
				}
				else {
					return new ColorF(((Constant)arr[0]).Value, ((Constant)arr[1]).Value, ((Constant)arr[2]).Value, ((Constant)arr[3]).Value);
				}
			}
		}


		public enum Message {

			[Message(MessageType.Warning, "Color as constant have to be between #0 and #FFFFFF.")]
			ColorConnstantOutOfRrange,
			[Message(MessageType.Warning, "Failed to parse a color from array. Expected constant at index {0}.")]
			ExpectedConstAtIndexI,
			[Message(MessageType.Warning, "Color as array must have length of 3 (RGB) or 4 (ARGB).")]
			ExpectedColorAsArrayLen34,
			[Message(MessageType.Warning, "Failed to parse color. Unknown value type.")]
			UnknownValue,

		}

	}
}

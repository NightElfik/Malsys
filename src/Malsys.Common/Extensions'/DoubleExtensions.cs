using System.Globalization;

namespace Malsys {
	public static class DoubleExtensions {

		public static string ToStringInvariant(this double val) {
			return val.ToString(CultureInfo.InvariantCulture.NumberFormat);
		}

		public static string ToStringInvariant(this double val, string format) {
			return val.ToString(format, CultureInfo.InvariantCulture.NumberFormat);
		}
	}
}

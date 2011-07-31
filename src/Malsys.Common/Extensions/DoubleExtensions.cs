using System.Globalization;

namespace Malsys {
	public static class DoubleExtensions {

		public static string ToStringInvariant(this double val) {
			return val.ToString(CultureInfo.InvariantCulture.NumberFormat);
		}
	}
}

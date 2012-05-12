/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System.Globalization;

namespace Malsys {
	public static class DoubleExtensions {

		public static string ToStringInvariant(this double val) {
			return val.ToString(CultureInfo.InvariantCulture.NumberFormat);
		}
	}
}

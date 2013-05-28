// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System;

namespace Malsys {
	public static class DataSizeHelper {

		private static readonly string[] units = { "B", "kB", "MB", "GB", "TB", "PB" };
		private static readonly string[] formats = { "{0:0.00}", "{0:0.0}", "{0:00}", "{0:000}" };

		/// <summary>
		/// Returns size in optimal units rounded to two significant digits.
		/// </summary>
		public static string ToOptimalUnitString(long size) {

			if (size == 0) {
				return formats[0].FmtInvariant(0) + " " + units[0];
			}

			int binaryDigits = (int)Math.Log(size, 2);
			int unitIndex = binaryDigits / 10;
			double value = (double)size / ((long)1 << (unitIndex * 10));

			if (value >= 1000) {
				value /= 1024;  // avoid values 1000 – 1024
				unitIndex++;
			}

			// 3 for 100 – 999
			// 2 for 10 – 99
			// 1 for 1 – 9
			// 0 for rest
			int formatIndex = Math.Max(0, (int)Math.Floor(Math.Log10(value)) + 1);

			return formats[formatIndex].FmtInvariant(value) + " " + units[unitIndex];

		}


	}
}

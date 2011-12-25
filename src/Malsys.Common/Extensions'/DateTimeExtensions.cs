using System;

namespace Malsys {
	public static class DateTimeExtensions {

		private static readonly string[] units = { "msec", "sec", "min", "hr", "day" };


		public static string ToTimeSpanStringFromNow(this DateTime dateTime) {
			return ToTimeSpanStringFromNow(dateTime, StandardDateTimeProvider.Instance);
		}

		public static string ToTimeSpanStringFromNow(this DateTime dateTime, IDateTimeProvider dateTimeProvider) {

			var ts = dateTimeProvider.Now - dateTime;

			double value;
			int unitIndex;

			if (ts.TotalMilliseconds < 1000) {
				value = ts.TotalMilliseconds;
				unitIndex = 0;
			}
			if (ts.TotalSeconds < 60) {
				value = ts.TotalSeconds;
				unitIndex = 1;
			}
			else if (ts.TotalMinutes < 60) {
				value = ts.TotalMinutes;
				unitIndex = 2;
			}
			else if (ts.TotalHours < 24) {
				value = ts.TotalHours;
				unitIndex = 3;
			}
			else {
				value = ts.TotalDays;
				unitIndex = 4;
			}


			if (value < 10) {
				return Math.Round(value, 1) + " " + units[unitIndex] + (value <= 1 ? "" : "s");
			}
			else {
				return Math.Round(value) + " " + units[unitIndex] + "s";
			}

		}

	}
}

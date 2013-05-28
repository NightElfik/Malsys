// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System;

namespace Malsys {
	public static class TimeExtensions {

		private static readonly string[] units = { "ms", "sec", "min", "hr", "day" };
		private static readonly string[] unitsPlural = { "ms", "sec", "min", "hrs", "days" };


		public static string ToTimeSpanStringFromNow(this DateTime dateTime) {
			return ToTimeSpanStringFromNow(dateTime, StandardDateTimeProvider.Instance);
		}

		public static string ToTimeSpanStringFromNow(this DateTime dateTime, IDateTimeProvider dateTimeProvider) {
			return ToAutoscaledString(dateTimeProvider.Now - dateTime);
		}

		public static string ToAutoscaledString(this TimeSpan timespan) {

			double value;
			int unitIndex;

			if (timespan.TotalMilliseconds < 1000) {
				value = timespan.TotalMilliseconds;
				unitIndex = 0;
			}
			else if (timespan.TotalSeconds < 60) {
				value = timespan.TotalSeconds;
				unitIndex = 1;
			}
			else if (timespan.TotalMinutes < 60) {
				value = timespan.TotalMinutes;
				unitIndex = 2;
			}
			else if (timespan.TotalHours < 24) {
				value = timespan.TotalHours;
				unitIndex = 3;
			}
			else {
				value = timespan.TotalDays;
				unitIndex = 4;
			}


			if (value < 10) {
				return Math.Round(value, 1) + " " + (value <= 1 ? units[unitIndex] : unitsPlural[unitIndex]);
			}
			else {
				return Math.Round(value) + " " + unitsPlural[unitIndex];
			}

		}

	}
}

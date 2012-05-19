/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System;

namespace Malsys.Web.Infrastructure {
	public static class DateTimeExtensions {

		public static string Hash(this DateTime dt) {
			return Convert.ToBase64String(BitConverter.GetBytes(int.Parse(dt.ToString("MMddhhmmss"))))
				.Replace('+','-')
				.Replace('/','_')
				.TrimEnd('=');
		}

	}
}
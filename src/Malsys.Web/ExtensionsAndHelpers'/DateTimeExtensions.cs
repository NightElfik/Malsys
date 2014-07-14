﻿// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System;

namespace Malsys.Web {
	public static class DateTimeExtensions {

		public static string Hash(this DateTime dt) {
			return Convert.ToBase64String(BitConverter.GetBytes(int.Parse(dt.ToString("MMddhhmmss"))))
				.Replace('+','-')
				.Replace('/','_')
				.TrimEnd('=');
		}

	}
}
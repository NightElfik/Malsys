/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System;

namespace Malsys {
	/// <summary>
	/// Standard implementation of the IDateTimeProvider interface.
	/// Returns information about current date.
	/// </summary>
	public class StandardDateTimeProvider : IDateTimeProvider {

		public static readonly StandardDateTimeProvider Instance = new StandardDateTimeProvider();


		public DateTime Now {
			get { return DateTime.Now; }
		}

	}
}

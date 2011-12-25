using System;

namespace Malsys {
	public class StandardDateTimeProvider : IDateTimeProvider {

		public static readonly StandardDateTimeProvider Instance = new StandardDateTimeProvider();


		public DateTime Now {
			get { return DateTime.Now; }
		}

	}
}

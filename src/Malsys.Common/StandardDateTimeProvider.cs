using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Malsys {
	public class StandardDateTimeProvider : IDateTimeProvider {

		public DateTime Now {
			get { return DateTime.Now; }
		}

	}
}

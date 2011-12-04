using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Malsys {
	public interface IDateTimeProvider {

		DateTime Now { get; }

	}
}

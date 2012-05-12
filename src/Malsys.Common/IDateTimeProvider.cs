/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System;

namespace Malsys {
	public interface IDateTimeProvider {

		DateTime Now { get; }

	}
}

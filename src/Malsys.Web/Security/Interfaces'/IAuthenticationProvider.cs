// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.

namespace Malsys.Web.Security {
	public interface IAuthenticationProvider {

		void LogOn(string userName, bool persistent);

		void LogOff();

	}
}

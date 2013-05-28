// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.

namespace Malsys.Web.Infrastructure {
	public interface IAppSettingsProvider {

		string this[string key] { get; }

	}
}

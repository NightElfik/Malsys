// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System.Web.Mvc;

namespace Malsys.Web {
	public static class UrlHelperExtensions {

		public static bool IsUrlSafeToRedirect(this UrlHelper urlHelper, string url) {
			return url != null && urlHelper.IsLocalUrl(url) && url.Length > 1 && url.StartsWith("/")
				&& !url.StartsWith("//") && !url.StartsWith("/\\");
		}

	}
}
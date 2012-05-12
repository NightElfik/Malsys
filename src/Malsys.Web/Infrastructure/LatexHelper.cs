/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System.Web;

namespace Malsys.Web.Infrastructure {
	public static class LatexHelper {

		public static HtmlString Print(string text) {
			return new HtmlString(text
				.Replace("#", @"\#")
				.Replace("{", @"\{")
				.Replace("}", @"\}")
				.Replace("^", @"\^")
			);
		}

	}
}
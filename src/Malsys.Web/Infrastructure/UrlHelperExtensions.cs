using System;
using System.Web;
using System.Web.Mvc;

namespace Malsys.Web.Infrastructure {
	public static class UrlHelperExtensions {

		public static HtmlString TypeLink(this UrlHelper url,  Type t) {

			string link = url.Action(MVC.Help.Predefined.Components());

			return new HtmlString("<a href=\"{0}#{2}\" class=\"type\">[<abbr title=\"{2}\">{1}</abbr>]</a>"
				.Fmt(link, t.Name, t.FullName));
		}


		public static HtmlString AnchorLink(this UrlHelper url, string text, ActionResult result, string anchor,
				bool anchorLink = false, bool autoHide = false) {

			return new HtmlString("<a href=\"{0}#{2}\" class=\"{3} {4}\">{1}</a>"
				.Fmt(url.Action(result), text, anchor, anchorLink ? "anchorLink" : "", autoHide ? "autoHide" : ""));
		}

	}
}
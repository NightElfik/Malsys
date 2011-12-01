using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Malsys.Web.Infrastructure {
	public static class HtmlHelpers {

		public static MvcHtmlString Link(this HtmlHelper html, string link) {
			return Link(html, link, link);
		}

		public static MvcHtmlString Link(this HtmlHelper html, string text, string link) {

			var tb = new TagBuilder("a");

			tb.MergeAttribute("href", link);
			tb.SetInnerText(text);

			return MvcHtmlString.Create(tb.ToString(TagRenderMode.Normal));
		}

	}
}
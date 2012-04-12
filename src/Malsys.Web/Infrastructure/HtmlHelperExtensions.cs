﻿using System.Web.Mvc;

namespace Malsys.Web.Infrastructure {
	public static class HtmlHelperExtensions {

		public static MvcHtmlString Link(this HtmlHelper html, string link) {
			return Link(html, link, link);
		}

		public static MvcHtmlString Link(this HtmlHelper html, string text, string link) {

			var tb = new TagBuilder("a");

			tb.MergeAttribute("href", link);
			tb.SetInnerText(text);

			return MvcHtmlString.Create(tb.ToString(TagRenderMode.Normal));
		}


		public static MvcHtmlString EmailLink(this HtmlHelper html, string email, string title = null) {
			return Link(html, title ?? email, "mailto:" + email);
		}

		public static MvcHtmlString SubmitButton(this HtmlHelper html, string name, string value) {

			var tb = new TagBuilder("input");
			tb.MergeAttribute("type", "submit");

			tb.MergeAttribute("name", name);
			tb.MergeAttribute("value", value);

			return MvcHtmlString.Create(tb.ToString(TagRenderMode.SelfClosing));
		}

		public static MvcHtmlString SubmitButton(this HtmlHelper html, string value) {

			var tb = new TagBuilder("input");
			tb.MergeAttribute("type", "submit");

			tb.MergeAttribute("value", value);

			return MvcHtmlString.Create(tb.ToString(TagRenderMode.SelfClosing));
		}

	}
}
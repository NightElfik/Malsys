/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System;
using System.Web;
using System.Web.Mvc;

namespace Malsys.Web.Infrastructure {
	public static class HtmlHelperExtensions {

		public static HtmlString Link(this HtmlHelper html, string link) {
			return Link(html, link, link);
		}

		public static HtmlString Link(this HtmlHelper html, string text, string link, string title = null, bool newWindow = false) {
			var tb = new TagBuilder("a");
			tb.SetInnerText(text);
			tb.MergeAttribute("href", link);
			if (title != null) {
				tb.MergeAttribute("title", title);
			}
			if (newWindow) {
				tb.MergeAttribute("target", "_blank");
			}

			return new HtmlString(tb.ToString());
		}


		public static HtmlString EmailLink(this HtmlHelper html, string email, string text = null, string title = null) {
			return Link(html, text ?? email, "mailto:" + email, title);
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

		public static HtmlString Tag(this HtmlHelper html, string tagName, string linkText = null) {
			return html.ActionLink(linkText ?? tagName, MVC.Gallery.Index(tag: tagName), new { @class = "tag" });
		}

		public static HtmlString TypeLink(this HtmlHelper html, Type t) {

			string link = new UrlHelper(html.ViewContext.RequestContext).Action(MVC.Help.Predefined.Components());

			return new HtmlString("<a href=\"{0}#{2}\" class=\"type\">[<abbr title=\"{2}\">{1}</abbr>]</a>"
				.Fmt(link, t.Name, t.FullName));
		}


		public static HtmlString AnchorLink(this HtmlHelper html, string text, ActionResult result, string anchor,
				bool anchorLink = false, bool autoHide = false) {

			return new HtmlString("<a href=\"{0}#{2}\" class=\"{3} {4}\">{1}</a>"
				.Fmt(new UrlHelper(html.ViewContext.RequestContext).Action(result), text, anchor, anchorLink ? "anchorLink" : "", autoHide ? "autoHide" : ""));
		}


		public static HtmlString InputPermaLink(this HtmlHelper html, string urlId) {
			return new HtmlString("<a href=\"{0}\" \">{0}</a>".Fmt(new UrlHelper(html.ViewContext.RequestContext).ActionAbsolute(MVC.Permalink.Index(urlId))));
		}

	}
}
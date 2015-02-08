using System;
using System.Text;
using System.Web;
using System.Web.Mvc;
using MvcContrib.Pagination;

namespace Malsys.Web {
	public static class HtmlHelperExtensions {

		public static HtmlString Link(this HtmlHelper html, string link, bool newWindow = false) {
			return Link(html, link, link, link, newWindow);
		}

		public static HtmlString Link(this HtmlHelper html, string text, string link, bool newWindow = false) {
			return Link(html, text, link, link, newWindow);
		}

		public static HtmlString Link(this HtmlHelper html, string text, string link, string title, bool newWindow = false) {
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

			string link = new UrlHelper(html.ViewContext.RequestContext).Action(MVC.Documentation.Predefined.Components());

			return new HtmlString("<a href=\"{0}#{2}\" class=\"type\">[<abbr title=\"{2}\">{1}</abbr>]</a>"
				.Fmt(link, t.Name, t.FullName));
		}


		public static HtmlString AnchorLink(this HtmlHelper html, string text, string anchor, ActionResult result = null,
				bool anchorLink = false, bool autoHide = false) {

			return new HtmlString("<a href=\"{0}#{2}\" class=\"{3} {4}\">{1}</a>"
				.Fmt(result == null ? "" : new UrlHelper(html.ViewContext.RequestContext).Action(result), text, anchor, anchorLink ? "anchorLink" : "", autoHide ? "autoHide" : ""));
		}


		public static HtmlString InputPermaLink(this HtmlHelper html, string urlId) {
			return new HtmlString("<a href=\"{0}\" \">{0}</a>".Fmt(new UrlHelper(html.ViewContext.RequestContext).ActionAbsolute(MVC.Permalink.Index(urlId))));
		}

		public static HtmlString Image(this HtmlHelper html, string src, int width, int height, string alt) {
			return new HtmlString("<img src=\"{0}\" width=\"{1}px\" height=\"{2}px\" alt=\"{3}\" />".Fmt(src, width, height, alt));
		}

		public static HtmlString CleverPager(this HtmlHelper helper, IPagination pagination, Func<int, string> urlResolver) {

			var sb = new StringBuilder();
			int advance = 2;

			sb.Append("<div class=\"pagination\">");
			sb.AppendFormat("<span class=\"paginationLeft\">Showing {0} - {1} of {2}</span>",
				pagination.FirstItem, pagination.LastItem, pagination.TotalItems);
			sb.Append("<span class=\"paginationRight\">");

			if (pagination.HasPreviousPage) {
				sb.AppendFormat("<a href=\"{0}\" title=\"Previous page\">Previous</a> | ",
					urlResolver(pagination.PageNumber - 1));
			}

			if (pagination.PageNumber - advance > 1) {
				sb.AppendFormat("<a href=\"{0}\" title=\"First page\">{1}</a>",
					urlResolver(1), 1);
			}

			if (pagination.PageNumber - advance - 1 > 1) {
				sb.Append(" ... ");
			}

			for (int i = pagination.PageNumber - advance; i <= pagination.PageNumber + advance; i++) {
				if (i < 1 || i > pagination.TotalPages) {
					continue;
				}

				sb.AppendFormat("<a href=\"{0}\" title=\"Page {1}\" {2}>{3}</a>", urlResolver(i),
					i,
					i == pagination.PageNumber ? "class=\"active\"" : "",
					i == pagination.PageNumber ? "[<b>" + i + "</b>]" : i.ToString());
			}


			if (pagination.PageNumber + advance + 1 < pagination.TotalPages) {
				sb.Append(" ... ");
			}

			if (pagination.PageNumber + advance < pagination.TotalPages) {
				sb.AppendFormat("<a href=\"{0}\" title=\"Last page\">{1}</a>",
					urlResolver(pagination.TotalPages), pagination.TotalPages);
			}

			if (pagination.HasNextPage) {
				sb.AppendFormat(" | <a href=\"{0}\" title=\"Next page\">Next</a>",
					urlResolver(pagination.PageNumber + 1));
			}

			sb.Append("</span></div>");

			return new HtmlString(sb.ToString());
		}

	}
}
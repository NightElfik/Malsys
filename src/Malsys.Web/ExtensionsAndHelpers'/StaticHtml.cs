using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;


namespace Malsys.Web {
	public static class StaticHtml {

		private const string reqScriptKey = "RequiredScripts";
		private const string inlineScriptKey = "InlineScripts";
		private static readonly Regex htmlTagRegex = new Regex(@"<.*?>", RegexOptions.Compiled);

		public static HtmlString Js(string filePath) {
			return new HtmlString("<script src=\"" + filePath + "\"></script>");
		}


		public const int DefaultOrder = 10;
		public const int SoonerOrder = 5;
		public const int LaterOrder = 15;
		public const int VeryFirstOrder = 0;

		/// <summary>
		/// Registers script path and automatically includes the script at the bottom of the loaded page.
		/// Redundant scripts with the same path are filtered and included only once.
		/// </summary>
		/// <remarks>
		/// Ordering is table so order of the entries with the same values  will remain as it was on the input.
		/// </remarks>
		/// <param name="path">Local or global path to the script that will be put to "src" attribute of a script tag.</param>
		/// <param name="order">Order of all included scripts (the lower, the sooner included).</param>
		public static void RequireScript(string path, int order = DefaultOrder) {
			var requiredScripts = HttpContext.Current.Items[reqScriptKey] as Dictionary<string, int>;
			if (requiredScripts == null) {
				HttpContext.Current.Items[reqScriptKey] = requiredScripts = new Dictionary<string, int>();
			}
			requiredScripts[path] = order;
		}

		public static HtmlString EmitRequiredScripts() {
			var requiredScripts = HttpContext.Current.Items[reqScriptKey] as Dictionary<string, int>;
			if (requiredScripts == null) {
				return new HtmlString("");
			}

			var sb = new StringBuilder();
			foreach (var scriptPath in requiredScripts.OrderBy(x => x.Value).Select(x => x.Key)) {
				sb.Append("<script src=\"");
				sb.Append(scriptPath);
				sb.Append("\"></script>");
			}
			return new HtmlString(sb.ToString());
		}

		/// <summary>
		/// Registers inline javascript code and automatically includes the script at the bottom of the loaded page (after all included scripts).
		/// Redundant scripts (with identical code) are filtered and printed only once.
		/// </summary>
		/// <param name="script">Javascript code.</param>
		/// <param name="order">Order of all inlined scripts (the lower, the sooner included).</param>
		public static void InlineScript(int order, params string[] script) {
			var inlineScripts = HttpContext.Current.Items[inlineScriptKey] as Dictionary<string, int>;
			if (inlineScripts == null) {
				HttpContext.Current.Items[inlineScriptKey] = inlineScripts = new Dictionary<string, int>();
			}
			inlineScripts[string.Join("", script)] = order;
		}

		public static void InlineScript(params string[] script) {
			InlineScript(DefaultOrder, script);
		}

		public static HtmlString EmitInlineScripts() {
			var inlineScripts = HttpContext.Current.Items[inlineScriptKey] as Dictionary<string, int>;
			if (inlineScripts == null) {
				return new HtmlString("");
			}

			var sb = new StringBuilder();
			sb.Append("<script>");
			foreach (var script in inlineScripts.OrderBy(x => x.Value).Select(x => x.Key)) {
				sb.Append(script);
			}
			sb.Append("</script>");
			return new HtmlString(sb.ToString());
		}


		public static HtmlString ExternalLink(string link) {
			return ExternalLink(link, link, link);
		}

		public static HtmlString ExternalLink(string text, string link) {
			return ExternalLink(text, link, link);
		}

		public static HtmlString ExternalLink(string text, string title, string link) {
			var sb = new StringBuilder();
			Link(sb, text, title, link, "externalLink", true);
			return new HtmlString(sb.ToString());
		}

		public static HtmlString Link(string text, string title, string link, string htmlClass, bool newWindow) {
			var sb = new StringBuilder();
			Link(sb, text, title, link, htmlClass, newWindow);
			return new HtmlString(sb.ToString());
		}

		public static void Link(StringBuilder sb, string text, string title, string link, string htmlClass, bool newWindow) {
			sb.Append("<a href=\"");
			sb.Append(link);
			if (title != null) {
				sb.Append("\" title=\"");
				sb.Append(title);
			}
			if (htmlClass != null) {
				sb.Append("\" class=\"");
				sb.Append(htmlClass);
			}
			if (newWindow) {
				sb.Append("\" target=\"_blank");
			}
			sb.Append("\">");
			sb.Append(text);
			sb.Append("</a>");
		}

		public static HtmlString WikiLink(string text, string wikiLink = null) {
			if (wikiLink == null) {
				if (text.EndsWith("[s]")) {
					wikiLink = text.Substring(0, text.Length - 3);
					text = wikiLink + "s";
				}
				else {
					wikiLink = text;
				}
			}

			wikiLink = wikiLink.Replace(" ", "_");
			string title = char.ToUpperInvariant(text[0]) + text.Substring(1);
			return Link(text, title + " at Wikipedia", "http://en.wikipedia.org/wiki/" + wikiLink, "wikilink", true);
		}

		public static string StripHtmlTags(string str) {
			if (str == null) {
				return null;
			}
			return htmlTagRegex.Replace(str, "");
		}

		public static string StripHtmlTagsAndEncode(string str) {
			if (str == null) {
				return null;
			}
			return HttpUtility.HtmlEncode(StripHtmlTags(str));
		}


		public static HtmlString DisqusComments(string title, string id, string url) {
			var req = HttpContext.Current.Request;
			string currentDomain = req.Url.Scheme + System.Uri.SchemeDelimiter + req.Url.Host
				+ (req.Url.IsDefaultPort ? "" : ":" + req.Url.Port);

			if (!url.StartsWith(currentDomain)) {
				return null;
			}

			url = req.Url.Scheme + System.Uri.SchemeDelimiter + GlobalSettings.Domain
				+ url.Substring(currentDomain.Length);

			InlineScript(@"
var disqus_shortname = '{0}';
var disqus_identifier = '{1}';
var disqus_title = '{2}';
var disqus_url = '{3}';

(function() {{
	var dsq = document.createElement('script');
	dsq.type = 'text/javascript'; dsq.async = true; dsq.src = '//' + disqus_shortname + '.disqus.com/embed.js';
	(document.getElementsByTagName('head')[0] || document.getElementsByTagName('body')[0]).appendChild(dsq);
}})();".FmtInvariant(GlobalSettings.DisqusShortName, id, title.Replace("'", @"\'").Replace("/", @"\/"), url));

			return new HtmlString(@"<div id=""disqus_thread""></div><a href=""http://disqus.com"" class=""dsq-brlink"">comments powered by <span class=""logo-disqus"">Disqus</span></a>");
		}
	}
}
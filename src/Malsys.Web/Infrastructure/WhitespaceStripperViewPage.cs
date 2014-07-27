using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;


namespace Malsys.Web.Infrastructure {
	/// <summary>
	/// View page for automatic stripping unnecessary white spaces.
	/// Strips white spaces only in between HTML tags where they have no purpose.
	/// </summary>
	/// <remarks>
	/// Whitespace stripping is not bullet-proof because input may be written by arbitrary chunks.
	/// For example if single chunk have more start and end whitespace stripping tags, it wont work properly.
	/// However if those tags leak in output they won't be displayed because they are in 'valid' HTML tags syntax.
	/// </remarks>
	public abstract class WhitespaceStripperViewPage<TModel> : System.Web.Mvc.WebViewPage<TModel> {

		private static readonly Regex wsBetweenTagsRegex = new Regex(@">\s+<", RegexOptions.Compiled);
		private static readonly Regex wsBeforeTagRegex = new Regex(@"^\s+<", RegexOptions.Compiled);
		private static readonly Regex wsAfterTagRegex = new Regex(@">\s+$", RegexOptions.Compiled);
		private static readonly Regex wsGlobalRegex = new Regex(@"\s+", RegexOptions.Compiled);

#if DEBUG
		public static bool Enabled = false;
#else
		public static bool Enabled = true;
#endif


		public override void Write(object value) {
			base.Write(value);
		}

		public override void WriteLiteral(object value) {
			base.WriteLiteral(Enabled ? process(value) : value);
		}

		private object process(object value) {
			if (value == null) {
				return null;
			}

			string html = value.ToString();
			html = wsBetweenTagsRegex.Replace(html, "><");
			html = wsBeforeTagRegex.Replace(html, "<");
			html = wsAfterTagRegex.Replace(html, ">");
			html = wsGlobalRegex.Replace(html, " ");

			if (value is HtmlString || value is MvcHtmlString) {
				return new HtmlString(html);
			}
			else {
				return html;
			}
		}

	}
}
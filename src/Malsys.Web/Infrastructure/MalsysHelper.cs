using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Malsys {
	public static class MalsysHelper {

		private static Regex terminal = new Regex("('(.[^']*|')')", RegexOptions.Compiled);
		private static Regex terminalRange = new Regex(@"\[(.)\-(.)\]", RegexOptions.Compiled);
		private static Regex terminalList = new Regex(@"\[([^'\-<\]]+)\]", RegexOptions.Compiled);
		private static Regex quantifier = new Regex("([^'])([?+*])", RegexOptions.Compiled);


		public static HtmlString TocLink(bool autoHide = false) {
			return new HtmlString("<div class=\"clearfix\"><a href=\"#toc\" class=\"tocLink" + (autoHide ? " autoHide" : "")
				+ "\">↑ table of contents ↑</a></div>");
		}


		public static HtmlString GrammarCode(string code) {

			code = HtmlEncode(code);
			code = highlightGrammar(code);
			return new HtmlString("<code>" + code + "</code>");

		}

		public static HtmlString GrammarBox(params string[] lines) {

			string code = HtmlEncode(StringHelper.JoinLines(lines));
			code = highlightGrammar(code);
			return new HtmlString("<pre class=\"grammar box\">" + code + "</pre>");

		}


		/// <summary>
		/// HTML-encodes a string and returns the encoded string.
		/// </summary>
		public static string HtmlEncode(string text) {

			if (text == null) {
				return null;
			}

			int len = text.Length;
			StringBuilder sb = new StringBuilder(len);

			for (int i = 0; i < len; i++) {
				switch (text[i]) {
					case '<': sb.Append("&lt;"); break;
					case '>': sb.Append("&gt;"); break;
					case '"': sb.Append("&quot;"); break;
					case '&': sb.Append("&amp;"); break;
					default:
						if (text[i] > 159) {
							// decimal numeric entity
							sb.Append("&#");
							sb.Append(((int)text[i]).ToString(CultureInfo.InvariantCulture));
							sb.Append(";");
						}
						else {
							sb.Append(text[i]);
						}
						break;
				}
			}

			return sb.ToString();
		}

		private static string highlightGrammar(string code) {

			code = terminal.Replace(code, "<span class=\"grTerminal\">$1</span>");
			code = terminalRange.Replace(code, "[<span class=\"grTerminal\">$1</span>-<span class=\"grTerminal\">$2</span>]");
			code = terminalList.Replace(code, "[<span class=\"grTerminal\">$1</span>]");
			// quantifiers needs to be after terminals because of quantifiers after terminal
			code = quantifier.Replace(code, "$1<span class=\"grQuantifier\">$2</span>");
			return code;

		}

	}
}
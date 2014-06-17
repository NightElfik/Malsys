using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Malsys.Web.ArticleTools {
	public class CodeHighlighter {

		private const string beforeTokenRegex = @"([;(\s])";
		private const string afterTokenRegex = @"([\s\[;:,&)(\*])";

		private static readonly Regex malsysKeywords = new Regex(beforeTokenRegex + "("
				+ "abstract|all|as|component|configuration|connect|consider|container|default|extends|fun|interpret|let"
				+ "|lsystem|nothing|or|process|return|rewrite|set|symbols|this|to|typeof|use|virtual|weight|with|where"
				+ ")" + afterTokenRegex,
			RegexOptions.Compiled);
		
		private static readonly Regex lineComment = new Regex(@"(//.*?)\r?\n", RegexOptions.Compiled);
		private static readonly Regex stringComment = new Regex(@"""((\\[^\n]|[^""\n])*)""", RegexOptions.Compiled);

		private int listingsCounter = 1;
		private StringBuilder sharedStringBuilder = new StringBuilder();


		public CodeHighlighter() {
			DefaultProgLang = ProgLang.Malsys;
		}

		public ProgLang DefaultProgLang { get; set; }


		public string Highlight(string code) {
			return Highlight(DefaultProgLang, code);
		}

		public string Highlight(ProgLang lang, string code) {
			code = " " + HttpUtility.HtmlEncode(code) + " ";  // Add spaces to highlight keywords even on begin/end.

			if ((int)(lang & ProgLang.Malsys) != 0) {
				code = malsysKeywords.Replace(code, "$1<span class=\"kw\">$2</span>$3");
				code = malsysKeywords.Replace(code, "$1<span class=\"kw\">$2</span>$3");  // Two passes of keywords to cover keywords after each other.
			}

			code = lineComment.Replace(code, (m) => "<span class=\"cm\">" + StaticHtml.StripHtmlTags(m.Groups[1].Value) + "</span>\n");

			return code.Trim(' ', '\r', '\n');  // Trim added white-spaces, do not trim tabs.
		}
	}

	[Flags]
	public enum ProgLang {
		None = 0x00,
		Malsys = 0x01,
	}
}
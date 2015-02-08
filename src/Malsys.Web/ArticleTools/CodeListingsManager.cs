using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Malsys.Web.ArticleTools {
	public class CodeListingsManager {

		private const string beforeTokenRegex = @"([;(\s])";
		private const string afterTokenRegex = @"([\s\[;:,&)(\*\.])";


		private static readonly Regex lineComment = new Regex(@"(//.*?)\r?\n", RegexOptions.Compiled);
		private static readonly Regex multiLineComment = new Regex(@"(/\*(.|[\r\n])*?\*/)", RegexOptions.Compiled | RegexOptions.Multiline);
		private static readonly Regex stringLiteral = new Regex(@"(&quot;(\\[^\n]|[^\n])*?&quot;)", RegexOptions.Compiled);

		private static readonly Regex csharpKeywords = new Regex(beforeTokenRegex + "("
				+ "abstract|as|base|bool|break|byte|case|catch|char|checked|class|const|continue|decimal|default"
				+ "|delegate|do|double|else|enum|event|explicit|extern|false|finally|fixed|float|for|foreach|goto|if"
				+ "|implicit|in|int|interface|internal|is|lock|long|namespace|new|null|object|operator|out|override"
				+ "|params|private|protected|public|readonly|ref|return|sbyte|sealed|short|sizeof|stackalloc|static"
				+ "|string|struct|switch|this|throw|true|try|typeof|uint|ulong|unchecked|unsafe|ushort|using|virtual"
				+ "|void|volatile|while|var"
				+ ")" + afterTokenRegex,
			RegexOptions.Compiled);

		private static readonly Regex csharpType = new Regex(beforeTokenRegex + "("
				+ "IRenderer2D|IRenderer|Point|ColorF|Polygon2D|Graphics|Matrix|MatrixOrder|PngReader|ChunkReader"
				+ "|PngWriter|FileStream|FileMode|FileAccess|Console"
				+ ")" + afterTokenRegex,
			RegexOptions.Compiled);

		private static readonly Regex malsysKeywords = new Regex(beforeTokenRegex + "("
				+ "abstract|all|as|component|configuration|connect|consider|container|default|extends|fun|interpret|let"
				+ "|lsystem|nothing|or|process|return|rewrite|set|symbols|this|to|typeof|use|virtual|weight|with|where"
				+ ")" + afterTokenRegex,
			RegexOptions.Compiled);


		private static readonly Regex terminal = new Regex("('(.[^']*|')')", RegexOptions.Compiled);
		private static readonly Regex terminalRange = new Regex(@"\[(.)\-(.)\]", RegexOptions.Compiled);
		private static readonly Regex terminalList = new Regex(@"\[([^'\-<\]]+)\]", RegexOptions.Compiled);
		private static readonly Regex quantifier = new Regex("([^'])([?+*])", RegexOptions.Compiled);

		private static readonly Regex fullName = new Regex(@"`(([a-zA-Z0-9\+]+\.)+([a-zA-Z0-9\+]+))`", RegexOptions.Compiled);
		private static readonly Regex quoted = new Regex(@"`(.+?)`", RegexOptions.Compiled);



		private int listingsCounter = 1;
		private StringBuilder sharedStringBuilder = new StringBuilder();


		public CodeListingsManager() {
			DefaultProgLang = ProgLang.None;
		}

		public ProgLang DefaultProgLang { get; set; }


		public CodeListing Code(string caption, string code, int startingLineNumber = 1) {
			return Code(DefaultProgLang, caption, code, startingLineNumber);
		}

		public CodeListing Code(ProgLang lang, string caption, string code, int startingLineNumber = 1) {
			return new CodeListing(this, getNextId(), caption, highlight(lang, code), startingLineNumber);
		}

		public HtmlString CodeNoFigure(ProgLang lang, string code, int maxHeight = int.MaxValue, int startingLineNumber = 1, bool shadow = false) {

			var sb = new StringBuilder();
			sb.AppendFormat("<div class='code {0}'>", shadow ? "" : "noShadow");
			toTable(sb, highlight(lang, code), maxHeight, startingLineNumber);
			sb.Append("</div>");

			return new HtmlString(sb.ToString());
		}

		public HtmlString InlineCode(string code) {
			return InlineCode(DefaultProgLang, code);
		}

		public HtmlString InlineCode(ProgLang lang, string code) {
			sharedStringBuilder.Length = 0;
			sharedStringBuilder.Append("<code class=\"code\">");
			sharedStringBuilder.Append(highlight(lang, code));
			sharedStringBuilder.Append("</code>");
			return new HtmlString(sharedStringBuilder.ToString());
		}


		private string getNextId() {
			string id = listingsCounter.ToString();
			++listingsCounter;
			return id;
		}

		private string highlight(ProgLang lang, string code) {
			code = " " + HttpUtility.HtmlEncode(code) + " ";  // Add spaces to highlight keywords even on begin/end.

			if (lang == ProgLang.Grammar) {
				code = code.Replace("&#39;", "'");
				code = terminal.Replace(code, "<span class=\"str\">$1</span>");
				code = terminalRange.Replace(code, "[<span class=\"str\">$1</span>-<span class=\"str\">$2</span>]");
				code = terminalList.Replace(code, "[<span class=\"str\">$1</span>]");
				// quantifiers needs to be after terminals because of quantifiers after terminal
				code = quantifier.Replace(code, "$1<span class=\"quant\">$2</span>");
				code = code.Replace("'", "&#39;");
			}
			else {
				if ((int)(lang & ProgLang.Malsys) != 0) {
					// Two passes of keywords to cover keywords after each other.
					code = malsysKeywords.Replace(code, "$1<span class='kw'>$2</span>$3");
					code = malsysKeywords.Replace(code, "$1<span class='kw'>$2</span>$3");
				}

				if ((int)(lang & ProgLang.Csharp) != 0) {
					// Two passes of keywords to cover keywords after each other.
					code = csharpKeywords.Replace(code, "$1<span class='kw'>$2</span>$3");
					code = csharpKeywords.Replace(code, "$1<span class='kw'>$2</span>$3");

					code = csharpType.Replace(code, "$1<span class='tp'>$2</span>$3");
					code = csharpType.Replace(code, "$1<span class='tp'>$2</span>$3");

					code = stringLiteral.Replace(code, (m) => "<span class='str'>" + StaticHtml.StripHtmlTags(m.Groups[1].Value) + "</span>");
				}

				if ((int)(lang & ProgLang.Xml) == 0) {
					code = lineComment.Replace(code, (m) => "<span class='cm'>" + StaticHtml.StripHtmlTags(m.Groups[1].Value) + "</span>\n");
					code = multiLineComment.Replace(code, (m) =>
						string.Join("\n", m.Groups[1].Value.SplitToLines()
							.Select(x => "<span class='cm'>" + StaticHtml.StripHtmlTags(x) + "</span>"))
					);
				}
			}

			return code.Trim(' ', '\r', '\n');  // Trim added white spaces, do not trim tabs.
		}

		private string toTable(string code, int maxHeight, int startingLineNumber) {
			var sb = new StringBuilder();
			toTable(sb, code, maxHeight, startingLineNumber);
			return sb.ToString();
		}

		private void toTable(StringBuilder sb, string code, int maxHeight, int startingLineNumber) {
			var lines = code.Replace("\t", "  ").SplitToLines().ToList();
			int linesCount = lines.Count;
			sb.Append("<div class='scroll'");
			if (maxHeight != int.MaxValue) {
				sb.Append(" style='height:");
				sb.Append(maxHeight);
				sb.Append("px;'");
			}
			sb.Append("><table><tr><td class='lineNumbers'>");
			for (int i = 0; i < linesCount; ++i) {
				sb.Append("<div");
				if (i % 2 == 1) {
					sb.Append(" class='alt'");
				}
				sb.Append(">");
				sb.Append(i + startingLineNumber);
				sb.Append("</div>");
			}
			sb.Append("</td><td class='codeBody'>");
			for (int i = 0; i < linesCount; ++i) {
				sb.Append("<div");
				if (i % 2 == 1) {
					sb.Append(" class='alt'");
				}
				sb.Append(">");
				sb.Append(lines[i].Length == 0 ? "&nbsp;" : lines[i]);
				sb.Append("</div>");
			}
			sb.Append("</td></tr></table></div>");
		}


		public class CodeListing : IFigure {

			private string id;
			private CodeListingsManager parent;
			private string caption;
			private string code;
			private int startingLineNumber;

			public CodeListing(CodeListingsManager parent, string id, string caption, string code, int startingLineNumber) {
				this.parent = parent;
				this.id = id;
				this.code = code;
				this.caption = caption;
				this.startingLineNumber = startingLineNumber;
			}

			public HtmlString Ref {
				get {
					return new HtmlString("<a href=\"#{0}\" title=\"Go to code listing {1}\">Code listing {1}</a>"
						.Fmt(HtmlId, id));
				}
			}

			public HtmlString Html {
				get {
					return CustomHtml(int.MaxValue);
				}
			}

			public HtmlString CustomHtml(int maxHeight) {
				return new HtmlString(toTable(maxHeight));
			}

			public string HtmlId {
				get {
					return "code-listing-" + id;
				}
			}

			public override string ToString() {
				return "DID YOU MEANT TO CALL REF()?";
			}

			private string toTable(int maxHeight) {
				var sb = new StringBuilder();
				sb.Append("<div class='code' id='");
				sb.Append(HtmlId);
				sb.Append("'><div class='caption'><h5>Code listing ");
				sb.Append(id);
				if (!string.IsNullOrEmpty(caption)) {
					sb.Append(": ");
					sb.Append(caption);
				}

				sb.Append("</h5></div>");
				parent.toTable(sb, code, maxHeight, startingLineNumber);
				sb.Append("</div>");
				return sb.ToString();
			}
		}

	}

	[Flags]
	public enum ProgLang {
		None = 0x00,
		Malsys = 0x01,
		Grammar = 0x2,
		Csharp = 0x10,
		Xml = 0x20,
	}
}
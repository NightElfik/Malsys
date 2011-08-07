using System.Text;
using System.Web;

namespace Malsys.SourceCode.Highlighters {
	public static class HtmlHighlighter {

		public static string HighlightFromString(string inputStr, string sourceName) {

			var hlLines = StringHighlighter.HighlightFromString(inputStr, sourceName, tagFunction,
				s => HttpUtility.HtmlEncode(s).Replace("\t", "    ").Replace(' ', CharHelper.Nbsp));

			return string.Join("\n", hlLines);
		}


		private static string tagFunction(MarkType markType, bool Begin, object[] data) {
			if (Begin) {
				switch (markType) {
					case MarkType.Unknown: return string.Empty;
					case MarkType.Line: return "<li><div class=\"ln\">";
					case MarkType.MsgError:
					case MarkType.MsgWarning:
					case MarkType.MsgNotice: return "<abbr class=\"{0}\" title=\"{1}\">".Fmt(markType.ToString(), data[0]);
					default: return "<span class=\"{0}\">".Fmt(markType.ToString());
				}
			}
			else {
				switch (markType) {
					case MarkType.Unknown: return string.Empty;
					case MarkType.Line: return CharHelper.Nbsp + "</div></li>"; // space before div is inportant for empty lines!
					case MarkType.MsgError:
					case MarkType.MsgWarning:
					case MarkType.MsgNotice: return "</abbr>";
					default: return "</span>";
				}
			}
		}
	}
}

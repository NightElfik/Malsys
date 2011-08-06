using System.Text;

namespace Malsys.SourceCode.Highlighters {
	public static class HtmlHighlighter {

		public static string HighlightFromString(string inputStr, string sourceName) {

			var openTags = StringHighlighter.CreateTagCache(openTagFunction);
			var closeTags = StringHighlighter.CreateTagCache(closeTagFunction);

			var hlLines = StringHighlighter.HighlightFromString(inputStr, sourceName, openTags, closeTags);

			return string.Join("\r\n", hlLines);
		}


		private static string openTagFunction(MarkType mt) {
			return "<span class=\"{0}\">".Fmt(mt.ToString());
		}

		private static string closeTagFunction(MarkType mt) {
			return "</span>";
		}
	}
}

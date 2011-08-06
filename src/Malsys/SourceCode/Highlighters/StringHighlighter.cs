using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Malsys.Compilers;
using Malsys.Parsing;
using Microsoft.FSharp.Text.Lexing;

namespace Malsys.SourceCode.Highlighters {
	public static class StringHighlighter {

		public static IEnumerable<string> HighlightFromString(string inputStr, string sourceName, Dictionary<MarkType, string> openTags, Dictionary<MarkType, string> closeTags) {

			var lexBuff = LexBuffer<char>.FromString(inputStr);
			var msgs = new MessagesCollection();
			var comments = new List<Ast.Comment>();

			var parsedInput = ParserUtils.parseLsystemStatements(lexBuff, comments, msgs, sourceName);

			var inputLines = inputStr.SplitToLines();

			return HighlightFromString(inputLines, parsedInput, comments, msgs, openTags, closeTags);
		}

		public static IEnumerable<string> HighlightFromString(IEnumerable<string> inputLines, Ast.InputBlock ast, IEnumerable<Ast.Comment> comments,
				MessagesCollection msgs, Dictionary<MarkType, string> openTags, Dictionary<MarkType, string> closeTags) {

			var marks = new List<PositionMark>(256);
			MarksCollector.Collect(ast, marks);
			MarksCollector.Collect(msgs, marks);
			MarksCollector.Collect(comments, marks);

			marks.Sort();

			return highlightFromString(inputLines, marks, openTags, closeTags);
		}

		public static Dictionary<MarkType, string> CreateTagCache(Func<MarkType, string> tagFunc) {

			var cache = new Dictionary<MarkType, string>();

			foreach (MarkType mt in Enum.GetValues(typeof(MarkType))) {
				cache.Add(mt, tagFunc(mt));
			}

			return cache;
		}


		private static IEnumerable<string> highlightFromString(IEnumerable<string> inputLines, IEnumerable<PositionMark> sortedMarks,
				Dictionary<MarkType, string> openTags, Dictionary<MarkType, string> closeTags) {

			var inputLinesEnumerator = inputLines.GetEnumerator();
			bool lineAviable = true;
			string ln = string.Empty;

			var marksEnumerator = sortedMarks.GetEnumerator();
			bool markAviable = marksEnumerator.MoveNext();
			PositionMark mark = markAviable ? marksEnumerator.Current : null;

			int lineNumber = 0;
			Stack<MarkType> openMarks = new Stack<MarkType>();
			StringBuilder sb = new StringBuilder();


			while (markAviable || lineAviable) {
				// line numbers are counted from 1
				lineNumber++;
				// columns are counted from 0
				int col = 0;

				// get next line if is aviable
				if (lineAviable) {
					lineAviable = inputLinesEnumerator.MoveNext();
					ln = lineAviable ? inputLinesEnumerator.Current : string.Empty;
				}

				while (markAviable && mark.Line == lineNumber) {
					if (mark.Column == col) {
						// mark is at current position, append it and move to next mark
						sb.Append(mark.Begin ? openTags[mark.Type] : closeTags[mark.Type]);
						markAviable = tryGetNextMark(marksEnumerator, out mark);
					}
					else {
						// append substring to next mark
						sb.AppendSpaceFill(ln, col, mark.Column);
						col = mark.Column;
					}
				}

				// append reminder of line
				if (col < ln.Length) {
					sb.AppendPos(ln, col, ln.Length);
				}

				yield return sb.ToString();
				sb.Clear();
			}
		}

		private static bool tryGetNextMark(IEnumerator<PositionMark> enumerator, out PositionMark result) {
#if DEBUG
			PositionMark prev = enumerator.Current;
#endif
			if (enumerator.MoveNext()) {
				result = enumerator.Current;
				Debug.Assert((prev.Line == result.Line && prev.Column <= result.Column) || prev.Line < result.Line, "Marks are not sorted correctly.");
				return true;
			}
			else {
				result = null;
				return false;
			}

		}
	}
}

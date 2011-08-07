using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Malsys.Compilers;
using Malsys.Parsing;
using Microsoft.FSharp.Text.Lexing;
using TagFun = System.Func<Malsys.SourceCode.Highlighters.MarkType, bool, object[], string>;

namespace Malsys.SourceCode.Highlighters {
	public static class StringHighlighter {

		public static IEnumerable<string> HighlightFromString(string inputStr, string sourceName, TagFun tagFun, Func<string, string> inputEncoder) {

			var lexBuff = LexBuffer<char>.FromString(inputStr);
			var msgs = new MessagesCollection();
			var comments = new List<Ast.Comment>();

			var parsedInput = ParserUtils.parseLsystemStatements(lexBuff, comments, msgs, sourceName);

			// just to get errors
			InputCompiler.CompileFromAst(parsedInput, msgs);

			var inputLines = inputStr.SplitToLines();

			return HighlightFromString(inputLines, parsedInput, comments, msgs, tagFun, inputEncoder);
		}

		public static IEnumerable<string> HighlightFromString(IEnumerable<string> inputLines, Ast.InputBlock ast, IEnumerable<Ast.Comment> comments,
				MessagesCollection msgs, TagFun tagFun, Func<string, string> inputEncoder) {

			var marks = new List<PositionMark>(256);
			MarksCollector.Collect(ast, marks);
			MarksCollector.Collect(msgs, marks);
			MarksCollector.Collect(comments, marks);

			marks.Sort();

			return highlightFromString(inputLines, marks, tagFun, inputEncoder);
		}

		public static Dictionary<MarkType, string> CreateTagCache(Func<MarkType, string> tagFunc) {

			var cache = new Dictionary<MarkType, string>();

			foreach (MarkType mt in Enum.GetValues(typeof(MarkType))) {
				cache.Add(mt, tagFunc(mt));
			}

			return cache;
		}


		private static IEnumerable<string> highlightFromString(IEnumerable<string> inputLines, IEnumerable<PositionMark> sortedMarks,
				TagFun tagFun, Func<string, string> inputEncoder) {

			var inputLinesEnumerator = inputLines.GetEnumerator();
			bool lineAviable = true;
			string ln = string.Empty;

			var marksEnumerator = sortedMarks.GetEnumerator();
			bool markAviable = marksEnumerator.MoveNext();
			PositionMark mark = markAviable ? marksEnumerator.Current : null;

			int lineNumber = 0;
			IndexableStack<PositionMark> opendMarks = new IndexableStack<PositionMark>();
			StringBuilder sb = new StringBuilder();


			while (markAviable || lineAviable) {
				// line numbers are counted from 1
				lineNumber++;
				// columns are counted from 0
				int col = 0;

				// start of line
				sb.Append(tagFun(MarkType.Line, true, null));

				// open all tags
				for (int i = 0; i < opendMarks.Count; i++) {
					sb.Append(tagFun(opendMarks[i].Type, true, opendMarks[i].Data));
				}

				// get next line if is aviable
				if (lineAviable) {
					lineAviable = inputLinesEnumerator.MoveNext();
					ln = lineAviable ? inputLinesEnumerator.Current : string.Empty;
				}

				while (markAviable && mark.Line == lineNumber) {
					if (mark.Column == col) {
						// mark is at current position, append it and move to next mark
						if (mark.Begin) {
							opendMarks.Push(mark);
							sb.Append(tagFun(mark.Type, true, mark.Data));
						}
						else {
							var m = opendMarks.Pop();
							Debug.Assert(m.PairId == mark.PairId, "Marks `{0}` and `{1}` are crossing each other".Fmt(mark.Type, m.Type));
							sb.Append(tagFun(mark.Type, false, mark.Data));
						}
						markAviable = tryGetNextMark(marksEnumerator, out mark);
					}
					else {
						// append substring to next mark
						var str = ln.SubstringSpaceFill(col, mark.Column);
						sb.Append(inputEncoder(str));
						col = mark.Column;
					}
				}

				// append reminder of line
				if (col < ln.Length) {
					var str = ln.SubstringPos(col, ln.Length);
					sb.Append(inputEncoder(str));
				}

				// close all tags
				for (int i = opendMarks.Count - 1; i >= 0; i--) {
					sb.Append(tagFun(opendMarks[i].Type, false, null));
				}

				// end of line
				sb.Append(tagFun(MarkType.Line, false, null));

				yield return sb.ToString();
				sb.Clear();
			}

			Debug.Assert(opendMarks.Count == 0, "Some marks are not closed.");
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

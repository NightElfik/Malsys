/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Malsys {
	public static class StringHelper {

		public static string AppendLinesAutoIndentWithSpaces(params string[] lines) {

			int indentLevel = 0;
			var sb = new StringBuilder();
			AppendLinesAutoIndent(' ', 4, ref indentLevel, lines, sb);
			return sb.ToString();
		}

		public static string AppendLinesAutoIndentWithTabs(params string[] lines) {

			int indentLevel = 0;
			var sb = new StringBuilder();
			AppendLinesAutoIndent('\t', 1, ref indentLevel, lines, sb);
			return sb.ToString();
		}

		public static string AppendLinesAutoIndent(char indentChar, int indentWidth, params string[] lines) {

			int indentLevel = 0;
			var sb = new StringBuilder();
			AppendLinesAutoIndent(indentChar, indentWidth, ref indentLevel, lines, sb);
			return sb.ToString();
		}

		public static string AppendLinesAutoIndent(char indentChar, int indentWidth, params IEnumerable<string>[] linesOfLines) {

			int indentLevel = 0;
			var sb = new StringBuilder();

			foreach (var lines in linesOfLines) {
				AppendLinesAutoIndent(indentChar, indentWidth, ref indentLevel, lines, sb);
			}

			return sb.ToString();
		}

		public static string AppendLinesAutoIndent(char indentChar, int indentWidth, IEnumerable<string> lines) {

			int indentLevel = 0;
			var sb = new StringBuilder();
			AppendLinesAutoIndent(indentChar, indentWidth, ref indentLevel, lines, sb);
			return sb.ToString();
		}

		public static void AppendLinesAutoIndent(char indentChar, int indentWidth, ref int indentLevel, IEnumerable<string> lines, StringBuilder sb) {

			bool first = sb.Length == 0;

			foreach (var line in lines) {
				if (first) {
					first = false;
				}
				else {
					sb.AppendLine();
				}

				if (line.StartsWith("}")) {
					indentLevel--;
				}

				sb.Append(new string(indentChar, indentLevel * indentWidth));

				sb.Append(line);

				if (line.EndsWith("{")) {
					indentLevel++;
				}
			}
		}

		public static string JoinLines(params string[] lines) {
			return string.Join(Environment.NewLine, lines);
		}

	}
}

﻿using System.Text;
using System.Collections.Generic;

namespace Malsys {
	public static class StringExtensions {
		/// <summary>
		/// Formates this string by standard <c>string.Format</c> function.
		/// </summary>
		/// <remarks>
		/// Name of this method cannot be `Format` because of wired error:
		/// Member 'string.Format(string, object)' cannot be accessed with an instance reference; qualify it with a type name instead.
		/// </remarks>
		public static string Fmt(this string format, params object[] args) {
			return string.Format(format, args);
		}

		public static string SubstringPos(this string str, int beginIndex, int endIndex) {
			return str.Substring(beginIndex, endIndex - beginIndex);
		}

		/// <summary>
		/// Splits this string to lines with respect to Windows, Unix and MAC line endings.
		/// </summary>
		public static IEnumerable<string> SplitToLines(this string str) {

			int beginIndex = 0;

			for (int i = 0; i < str.Length; i++) {

				switch (str[i]) {
					case '\r':
						var lineR = str.SubstringPos(beginIndex, i);
						if (i + 1 < str.Length && str[i + 1] == '\n') {
							i++;
						}
						beginIndex = i + 1;
						yield return lineR;
						break;

					case '\n':
						var lineN = str.SubstringPos(beginIndex, i);
						beginIndex = i + 1;
						yield return lineN;
						break;
				}
			}

			if (beginIndex != str.Length) {
				yield return str.Substring(beginIndex);
			}
		}

		/// <summary>
		/// Appends a copy of a specified substring to the end of this instance.
		/// </summary>
		/// <param name="beginIndex">Zero-based inclusive starting character index of a substring.</param>
		/// <param name="endIndex">Zero-based exclusive end character index of a substring.</param>
		public static void AppendPos(this StringBuilder sb, string str, int beginIndex, int endIndex) {
			sb.Append(str, beginIndex, endIndex - beginIndex);
		}

		/// <summary>
		/// Appends a copy of a specified substring to the end of this instance. If given string do not have enough
		/// characters in given range, spaces are used to fill it.
		/// </summary>
		/// <param name="beginIndex">Zero-based inclusive starting character index of a substring.</param>
		/// <param name="endIndex">Zero-based exclusive end character index of a substring.</param>
		public static void AppendSpaceFill(this StringBuilder sb, string str, int beginIndex, int endIndex) {
			int len = endIndex - beginIndex;

			if (endIndex < str.Length) {
				sb.Append(str, beginIndex, len);
			}
			else {
				if (beginIndex < str.Length) {
					int fromStrLen = str.Length - beginIndex;

					sb.Append(str, beginIndex, fromStrLen);
					sb.Append(' ', len - fromStrLen);
				}
				else {
					sb.Append(' ', len);
				}
			}
		}

	}
}

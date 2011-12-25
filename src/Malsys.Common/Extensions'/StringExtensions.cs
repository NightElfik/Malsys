using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Text;

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

			Contract.Requires<ArgumentNullException>(format != null);
			Contract.Requires<ArgumentNullException>(args != null);
			Contract.Ensures(Contract.Result<string>() != null);

			return string.Format(format, args);
		}

		/// <summary>
		/// Formates this string by standard <c>string.Format</c> function using Invariant Culture.
		/// </summary>
		public static string FmtInvariant(this string format, params object[] args) {

			Contract.Requires<ArgumentNullException>(format != null);
			Contract.Requires<ArgumentNullException>(args != null);
			Contract.Ensures(Contract.Result<string>() != null);

			return string.Format(CultureInfo.InvariantCulture, format, args);
		}

		/// <summary>
		/// Returns a copy of a specified substring of this instance.
		/// </summary>
		/// <param name="beginIndex">Zero-based inclusive starting character index of a substring.</param>
		/// <param name="endIndex">Zero-based exclusive ending character index of a substring.</param>
		public static string SubstringPos(this string str, int beginIndex, int endIndex) {

			Contract.Requires<ArgumentNullException>(str != null);
			Contract.Requires<ArgumentOutOfRangeException>(beginIndex >= 0 && beginIndex <= str.Length);
			Contract.Requires<ArgumentOutOfRangeException>(endIndex >= 0 && endIndex <= str.Length);
			Contract.Requires<ArgumentOutOfRangeException>(beginIndex <= endIndex);
			Contract.Ensures(Contract.Result<string>() != null);

			return str.Substring(beginIndex, endIndex - beginIndex);
		}

		/// <summary>
		/// Returns a copy of a specified substring of this instance. If this string do not have enough
		/// characters in given range, spaces are used to fill it.
		/// </summary>
		/// <param name="beginIndex">Zero-based inclusive starting character index of a substring.</param>
		/// <param name="endIndex">Zero-based exclusive ending character index of a substring.</param>
		public static string SubstringSpaceFill(this string str, int beginIndex, int endIndex) {
			int len = endIndex - beginIndex;

			if (endIndex < str.Length) {
				return str.Substring(beginIndex, len);
			}
			else {
				if (beginIndex < str.Length) {
					StringBuilder sb = new StringBuilder(len);
					int fromStrLen = str.Length - beginIndex;

					sb.Append(str, beginIndex, fromStrLen);
					sb.Append(' ', len - fromStrLen);

					return sb.ToString();
				}
				else {
					return new string(' ', len);
				}
			}
		}

		/// <summary>
		/// Splits this string to lines with respect to Windows, Unix or MAC line endings.
		/// </summary>
		public static IEnumerable<string> SplitToLines(this string str) {

			Contract.Requires<ArgumentNullException>(str != null);

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
			else {
				yield return "";
			}
		}

		/// <summary>
		/// Appends a copy of a specified substring to the end of this instance.
		/// </summary>
		/// <param name="beginIndex">Zero-based inclusive starting character index of a substring.</param>
		/// <param name="endIndex">Zero-based exclusive ending character index of a substring.</param>
		public static void AppendPos(this StringBuilder sb, string str, int beginIndex, int endIndex) {

			Contract.Requires<ArgumentNullException>(sb != null);
			Contract.Requires<ArgumentNullException>(str != null);
			Contract.Requires<ArgumentOutOfRangeException>(beginIndex >= 0 && beginIndex <= str.Length);
			Contract.Requires<ArgumentOutOfRangeException>(endIndex >= 0 && endIndex <= str.Length);
			Contract.Requires<ArgumentOutOfRangeException>(beginIndex <= endIndex);
			Contract.Requires<ArgumentOutOfRangeException>(str.Length - endIndex >= 0);

			sb.Append(str, beginIndex, endIndex - beginIndex);
		}

		/// <summary>
		/// Appends a copy of a specified substring to the end of this instance. If given string do not have enough
		/// characters in given range, spaces are used to fill it.
		/// </summary>
		/// <param name="beginIndex">Zero-based inclusive starting character index of a substring.</param>
		/// <param name="endIndex">Zero-based exclusive ending character index of a substring.</param>
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

		/// <summary>
		/// Repeats this string count-times.
		/// </summary>
		public static string Repeat(this string pattern, int count) {

			Contract.Requires<ArgumentNullException>(pattern != null);
			Contract.Requires<ArgumentOutOfRangeException>(count >= 0);
			Contract.Ensures(Contract.Result<string>() != null);

			return new StringBuilder().Insert(0, pattern, count).ToString();
		}
	}
}

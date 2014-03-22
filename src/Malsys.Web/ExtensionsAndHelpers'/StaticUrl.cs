using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;

namespace Malsys.Web {
	public class StaticUrl {

		private static readonly Regex nonAlphanumRegex = new Regex("[^a-zA-Z0-9_]", RegexOptions.Compiled);
		private static readonly Regex multiDash = new Regex("[-]+", RegexOptions.Compiled);

		/// <summary>
		/// Converts given string to safe representation usable in URLs or IDs.
		/// </summary>
		/// <remarks>
		/// Converts all non-alphanumeric characters to dashes and all multi-dashes to one.
		/// Also, dashes are deleted from beginning and end. This means that non-alphanumeric strings results in empty string.
		/// </remarks>
		public static string UrlizeString(string str) {
			Contract.Requires(str != null);
			Contract.Ensures(Contract.Result<string>() != null);

			str = nonAlphanumRegex.Replace(str, "-");
			str = multiDash.Replace(str, "-");
			str = str.Trim('-');
			return str;
		}

	}
}
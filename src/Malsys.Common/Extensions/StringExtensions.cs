
namespace Malsys {
	public static class StringExtensions {
		/// <summary>
		/// Formates given string by standard <c>string.Format</c> function.
		/// </summary>
		/// <remarks>
		/// Name of this method cannot be `Format` because of wired error:
		/// Member 'string.Format(string, object)' cannot be accessed with an instance reference; qualify it with a type name instead.
		/// </remarks>
		public static string Fmt(this string format, params object[] args) {
			return string.Format(format, args);
		}
	}
}

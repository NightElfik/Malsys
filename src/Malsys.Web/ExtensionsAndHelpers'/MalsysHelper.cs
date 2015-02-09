using System.Text.RegularExpressions;
using System.Web;

namespace Malsys.Web {
	public static class MalsysHelper {

		private static Regex fullName = new Regex(@"`(([a-zA-Z0-9\+]+\.)+([a-zA-Z0-9\+]+))`", RegexOptions.Compiled);
		private static Regex quoted = new Regex(@"`(.+?)`", RegexOptions.Compiled);


		public static HtmlString SimplifyMessage(string msg) {
			msg = HttpUtility.HtmlEncode(msg);
			msg = fullName.Replace(msg, "`<abbr title=\"$1\">$3</abbr>`");
			msg = quoted.Replace(msg, "`<span class=\"quoted\">$1</span>`");
			return new HtmlString(msg);
		}

	}
}

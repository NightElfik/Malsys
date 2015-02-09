using System.Web;

namespace Malsys.Web.Infrastructure {
	public static class LatexHelper {

		public static HtmlString Print(string text) {
			return new HtmlString(text
				.Replace("#", @"\#")
				.Replace("{", @"\{")
				.Replace("}", @"\}")
				.Replace("^", @"\^")
			);
		}

	}
}

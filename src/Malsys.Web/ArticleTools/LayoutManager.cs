using System.Diagnostics.Contracts;
using System.Web;

namespace Malsys.Web.ArticleTools {
	public class LayoutManager {

		public HtmlString NewBodySection(string id = null, string extraClass = null) {
			return new HtmlString("</div><div class=\"grid-container"
				+ (extraClass == null ? "" : " " + extraClass) + "\""
				+ ((id == null) ? "" : " id=\"" + id + "\"")
				+ ">");
		}


		public HtmlString StartParent(string extraClass = "") {
			return new HtmlString("<div class=\"grid-100 mobile-grid-100 grid-parent {0}\">"
				.Fmt(extraClass));
		}

		public HtmlString StartColumn(int firstColumnPerc = 50, string extraClass = "", string extraParentClass = "") {
			Contract.Requires(firstColumnPerc % 5 == 0 || firstColumnPerc == 33 || firstColumnPerc == 66);

			return new HtmlString("<div class=\"grid-100 mobile-grid-100 grid-parent {2}\"><div class=\"grid-{0} mobile-grid-100 {1}\">"
				.Fmt(firstColumnPerc, extraClass, extraParentClass));
		}


		public HtmlString NextColumn(int columnPerc = 50, string extraClass = "") {
			Contract.Requires(columnPerc % 5 == 0 || columnPerc == 33 || columnPerc == 66);

			return new HtmlString("</div><div class=\"grid-{0} mobile-grid-100 {1}\">"
				.Fmt(columnPerc, extraClass));
		}


		public HtmlString EndColumn() {
			return new HtmlString("</div></div>");
		}

		public HtmlString EndParent() {
			return new HtmlString("</div>");
		}

	}
}

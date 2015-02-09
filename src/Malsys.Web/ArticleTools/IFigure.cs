using System.Web;

namespace Malsys.Web.ArticleTools {
	public interface IFigure {

		HtmlString Ref { get; }

		HtmlString Html { get; }

		string HtmlId { get; }

	}

	public class SimpleFigure : IFigure {

		public HtmlString Ref { get; set; }

		public HtmlString Html { get; set; }

		public string HtmlId { get; set; }

		public override string ToString() {
			return "DID YOU MEANT TO CALL REF()?";
		}

	}
}

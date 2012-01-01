using System.Web.Mvc;

namespace Malsys.Web.Controllers {
	public partial class ExamplesController : Controller {

		public virtual ActionResult Index() {
			return View();
		}


		public virtual ActionResult FassCurves() {
			return View();
		}

		public virtual ActionResult Abop() {
			return View();
		}

		public virtual ActionResult Tilings() {
			return View();
		}

		public virtual ActionResult Ascii() {
			return View();
		}

		public virtual ActionResult Other() {
			return View();
		}

	}
}

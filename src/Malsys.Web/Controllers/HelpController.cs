using System.Web.Mvc;

namespace Malsys.Web.Controllers {
	public class HelpController : Controller {

		public ActionResult Index() {
			return View();
		}

		public ActionResult Syntax() {
			return View();
		}

	}
}

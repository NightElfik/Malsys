using System.Web.Mvc;

namespace Malsys.Web.Controllers {
	public partial class HomeController : Controller {
		public virtual ActionResult Index() {
			return View();
		}

		public virtual ActionResult About() {
			return View();
		}
	}
}

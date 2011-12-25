using System.Web.Mvc;

namespace Malsys.Web.Areas.Administration.Controllers {
	[Authorize(Roles = "admins")]
	public partial class HomeController : Controller {

		public virtual ActionResult Index() {
			return View();
		}

	}
}

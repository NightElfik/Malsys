using System.Web.Mvc;

namespace Malsys.Web.Controllers {
	public partial class PermalinkController : Controller {

		public virtual ActionResult Index(string id) {
			return RedirectToAction(MVC.Gallery.ActionNames.Detail, MVC.Gallery.Name, new { id = id });
		}

	}
}

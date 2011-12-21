using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Malsys.Web.Controllers.Administration {
	[Authorize(Roles="admins")]
	public class AdministrationController : Controller {

		public ActionResult Index() {
			return View();
		}

	}
}

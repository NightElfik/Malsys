using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Malsys.Web.Controllers {
	public partial class DevDiaryController : Controller {

		public virtual ActionResult Index() {
			return View();
		}


		public virtual ActionResult PngAnimationRenderer() {
			return View();
		}

	}
}

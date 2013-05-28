// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System.Web.Mvc;

namespace Malsys.Web.Controllers {
	public partial class PermalinkController : Controller {

		public virtual ActionResult Index(string id) {
			return RedirectToAction(MVC.Gallery.ActionNames.Detail, MVC.Gallery.Name, new { id = id });
		}

	}
}

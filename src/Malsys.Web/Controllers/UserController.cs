/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System.Web.Mvc;
using Malsys.Web.Models;

namespace Malsys.Web.Controllers {
	[Authorize]
	public partial class UserController : Controller {

		private readonly IMalsysInputRepository malsysInputRepository;

		public UserController(IMalsysInputRepository malsysInputRepository) {

			this.malsysInputRepository = malsysInputRepository;
		}


		public virtual ActionResult Index() {
			return View();
		}

	}
}

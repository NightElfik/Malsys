/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System;
using System.Web.Helpers;
using System.Web.Mvc;
using Malsys.Web.Areas.Administration.Models;
using Malsys.Web.Entities;

namespace Malsys.Web.Areas.Administration.Controllers {
	[Authorize(Roles = UserRoles.Administrator)]
	public partial class HomeController : Controller {

		private readonly IUsersDb usersDb;


		public HomeController(IUsersDb usersDb) {
			this.usersDb = usersDb;
		}


		public virtual ActionResult Index() {
			return View();
		}

		public virtual ActionResult TryThrowException() {
			throw new Exception("Test exception");
		}

		public virtual ActionResult TrySendMail() {
			string userNameLower = User.Identity.Name.ToLowerInvariant();
			return View(new MailModel() {
				Address = usersDb.GetUserByName(User.Identity.Name).Email,
				Subject = "Test mail from Malsys",
				Body = "Mail sending is working!"
			});
		}

		[HttpPost]
		public virtual ActionResult TrySendMail(MailModel model) {

			if (ModelState.IsValid) {
				bool success = false;
				try {
					WebMail.Send(model.Address, model.Subject, model.Body);
					success = true;
				}
				catch (Exception ex) {
					ModelState.AddModelError("", "Failed to send an e-mail. " + ex.Message);
				}

				if (success) {
					return RedirectToAction("Index");
				}
			}

			return View(model);
		}

	}
}

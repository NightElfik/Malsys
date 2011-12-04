using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Malsys.Web.Models;
using Malsys.Web.Security;

namespace Malsys.Web.Controllers {
	public class AccountController : Controller {

		private readonly IUsersRepository usersRepo;

		private readonly IUserAuthenticator userAuth;


		public AccountController(IUsersRepository usersRepo, IUserAuthenticator userAuth) {
			this.usersRepo = usersRepo;
			this.userAuth = userAuth;
		}



		public ActionResult Register() {
			return View();
		}

		[HttpPost]
		public ActionResult Register(NewUserModel model) {

			if (ModelState.IsValid) {
				bool success = false;
				try {
					usersRepo.CreateUser(model);
					success = true;
				}
				catch (Exception ex) {
					ModelState.AddModelError("", ex.Message);
				}

				if (success) {
					return RedirectToAction("LogOn", "Authentication");
				}
			}

			return View(model);
		}

		[Authorize]
		public ActionResult ChangePassword() {
			return View();
		}

		[Authorize]
		[HttpPost]
		public ActionResult ChangePassword(ChangePasswordModel model) {

			if (ModelState.IsValid) {
				bool success = false;
				try {
					userAuth.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
					success = true;
				}
				catch (Exception) {
					ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
				}

				if (success) {
					return RedirectToAction("ChangePasswordSuccess");
				}
			}

			return View(model);
		}

		public ActionResult ChangePasswordSuccess() {
			return View();
		}

	}
}

/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System;
using System.Web.Mvc;
using Malsys.Web.Infrastructure;
using Malsys.Web.Models;
using Malsys.Web.Security;
using Microsoft.Web.Helpers;

namespace Malsys.Web.Controllers {
	public partial class AccountController : Controller {

		private readonly IUsersRepository usersRepo;

		private readonly IUserAuthenticator userAuth;


		public AccountController(IUsersRepository usersRepo, IUserAuthenticator userAuth, IAppSettingsProvider appSettingsProvider) {
			this.usersRepo = usersRepo;
			this.userAuth = userAuth;

			ReCaptcha.PublicKey = appSettingsProvider[AppSettingsKeys.ReCaptchaPublicKey];
			ReCaptcha.PrivateKey = appSettingsProvider[AppSettingsKeys.ReCaptchaPrivateKey];
		}



		public virtual ActionResult Register() {
			return View();
		}

		[HttpPost]
		public virtual ActionResult Register(NewUserModel model) {

			if (!ModelState.IsValid) {
				return View(model);
			}

			if (!ReCaptcha.Validate()) {
				ModelState.AddModelError("", "Captcha invalid.");
				return View(model);
			}

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
			else {
				return View(model);
			}
		}

		[Authorize]
		public virtual ActionResult ChangePassword() {
			return View();
		}

		[Authorize]
		[HttpPost]
		public virtual ActionResult ChangePassword(ChangePasswordModel model) {

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

		public virtual ActionResult ChangePasswordSuccess() {
			return View();
		}

	}
}

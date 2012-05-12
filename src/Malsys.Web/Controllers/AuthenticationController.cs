/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System.Linq;
using System.Web.Mvc;
using Malsys.Web.Models;
using Malsys.Web.Security;

namespace Malsys.Web.Controllers {
	public partial class AuthenticationController : Controller {

		private readonly IUserAuthenticator userAuth;
		private readonly IAuthenticationProvider authProvider;
		private readonly IUsersRepository usersRepo;


		public AuthenticationController(IUserAuthenticator userAuth, IAuthenticationProvider authProvider, IUsersRepository usersRepo) {
			this.userAuth = userAuth;
			this.authProvider = authProvider;
			this.usersRepo = usersRepo;
		}



		public virtual ActionResult LogOn(string userName = null) {

			if (string.IsNullOrWhiteSpace(userName)) {
				return View();
			}
			else {
				return View(new LogOnModel() { UserName = userName });
			}
		}

		[HttpPost]
		public virtual ActionResult LogOn(LogOnModel model, string returnUrl) {

			if (ModelState.IsValid) {

				if (userAuth.ValidateUser(model.UserName, model.Password)) {
					var realName = usersRepo.Users.Where(u => u.NameLowercase == model.UserName.Trim().ToLower()).Single().Name;
					authProvider.LogOn(realName, model.RememberMe);

					if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
							&& !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\")) {
						return Redirect(returnUrl);
					}
					else {
						return RedirectToAction("Index", "Home");
					}
				}
				else {
					ModelState.AddModelError("", "The user name or password provided is incorrect.");
				}
			}

			return View(model);
		}


		public virtual ActionResult LogOff() {

			authProvider.LogOff();

			return RedirectToAction("Index", "Home");
		}

	}
}

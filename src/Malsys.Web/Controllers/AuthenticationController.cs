/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System.Linq;
using System.Web.Mvc;
using Malsys.Web.Models;
using Malsys.Web.Security;
using Malsys.Web.Entities;

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

				var userResult = userAuth.ValidateUser(model.UserName, model.Password);
				if (userResult) {
					var user = userResult.Data;
					var realName = user.Name;
					authProvider.LogOn(realName, model.RememberMe);

					usersRepo.UsersDb.Log("LogIn", ActionLogSignificance.Low, null, user);

					if (Url.IsUrlSafeToRedirect(returnUrl)) {
						return Redirect(returnUrl);
					}
					else {
						return RedirectToAction(MVC.Home.Index());
					}
				}
				else {
					usersRepo.UsersDb.Log("LogIn-Fail", ActionLogSignificance.Medium, model.UserName);
					ModelState.AddModelError("", userResult.ErrorMessage);
				}
			}

			return View(model);
		}


		public virtual ActionResult LogOff() {

			if (User.Identity.IsAuthenticated) {
				var user = usersRepo.UsersDb.TryGetUserByName(User.Identity.Name);
				usersRepo.UsersDb.Log("LogOff", ActionLogSignificance.Low, null, user);
			}

			authProvider.LogOff();
			return RedirectToAction(MVC.Home.Index());
		}

	}
}

using System.Web.Mvc;
using Malsys.Web.Entities;
using Malsys.Web.Infrastructure;
using Malsys.Web.Models;
using Malsys.Web.Security;

namespace Malsys.Web.Controllers {
	public partial class AccountController : Controller {

		private readonly IUsersRepository usersRepo;
		private readonly IUserAuthenticator userAuth;
		private readonly ICaptcha captcha;


		public AccountController(IUsersRepository usersRepo, IUserAuthenticator userAuth, ICaptcha captcha) {
			this.usersRepo = usersRepo;
			this.userAuth = userAuth;
			this.captcha = captcha;
		}


		public virtual ActionResult Register() {
			return View(new NewUserModel() { Captcha = captcha });
		}

		[HttpPost]
		public virtual ActionResult Register(NewUserModel model) {
			model.Captcha = captcha;

			if (!ModelState.IsValid) {
				return View(model);
			}

			if (!captcha.Validate(ControllerContext.HttpContext)) {
				ModelState.AddModelError("", "Captcha is invalid.");
				return View(model);
			}

			var userResult = usersRepo.CreateUser(model);
			if (userResult) {
				usersRepo.UsersDb.Log("Register", ActionLogSignificance.Medium, null, userResult.Data);
				return RedirectToAction(MVC.Authentication.LogOn(userResult.Data.Name));
			}
			else {
				usersRepo.UsersDb.Log("Register-Fail", ActionLogSignificance.Medium, userResult.ErrorMessage);
			}

			ModelState.AddModelError("", "Failed to create user. " + userResult.ErrorMessage);
			return View(model);
		}

		[Authorize]
		public virtual ActionResult ChangePassword() {
			return View();
		}

		[Authorize]
		[HttpPost]
		public virtual ActionResult ChangePassword(ChangePasswordModel model) {

			if (ModelState.IsValid) {
				var userResult = userAuth.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
				if (userResult) {
					usersRepo.UsersDb.Log("PwdChange", ActionLogSignificance.Medium, null, usersRepo.UsersDb.TryGetUserByName(User.Identity.Name));
					return RedirectToAction(Actions.ChangePasswordSuccess());
				}

				usersRepo.UsersDb.Log("PwdChange-Fail", ActionLogSignificance.Medium, "wrong password", usersRepo.UsersDb.TryGetUserByName(User.Identity.Name));
				ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
			}

			return View(model);
		}

		public virtual ActionResult ChangePasswordSuccess() {
			return View();
		}

	}
}

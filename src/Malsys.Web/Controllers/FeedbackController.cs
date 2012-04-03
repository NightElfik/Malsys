﻿using System.Linq;
using System.Web.Mvc;
using Malsys.Web.Entities;
using Malsys.Web.Infrastructure;
using Malsys.Web.Models;
using Microsoft.Web.Helpers;
using MvcContrib.Pagination;

namespace Malsys.Web.Controllers {
	public partial class FeedbackController : Controller {

		private readonly IFeedbackDb feedbackDb;
		private readonly IDateTimeProvider dateTimeProvider;
		private readonly IUsersDb usersDb;


		public FeedbackController(IFeedbackDb feedbackDb, IDateTimeProvider dateTimeProvider,
				IUsersDb usersDb, IAppSettingsProvider appSettingsProvider) {

			this.feedbackDb = feedbackDb;
			this.dateTimeProvider = dateTimeProvider;
			this.usersDb = usersDb;

			ReCaptcha.PublicKey = appSettingsProvider[AppSettingsKeys.ReCaptchaPublicKey];
			ReCaptcha.PrivateKey = appSettingsProvider[AppSettingsKeys.ReCaptchaPrivateKey];
		}


		public virtual ActionResult Index() {
			return View();
		}

		[HttpPost]
		public virtual ActionResult Index(FeedbackModel model) {

			if (!ModelState.IsValid) {
				return View(model);
			}

			if (!ReCaptcha.Validate()) {
				ModelState.AddModelError("", "Captcha invalid.");
				return View(model);
			}

			var feedback = new Feedback() {
				User = usersDb.TryGetUserByName(User.Identity.Name),
				SubmitDate = dateTimeProvider.Now,
				Subject = model.Subject.Trim(),
				Email = string.IsNullOrWhiteSpace(model.Email) ? null : model.Email.Trim(),
				Message = model.Message.Trim(),
				IsNew = true
			};

			feedbackDb.AddFeedback(feedback);
			feedbackDb.SaveChanges();

			return RedirectToAction("Thanks");
		}

		public virtual ActionResult Thanks() {
			return View();
		}


		public virtual ActionResult EmailToAuthor() {
			return View((string)null);
		}

		[HttpPost]
		[ActionName("EmailToAuthor")]
		public virtual ActionResult EmailToAuthorPost() {

			if (ReCaptcha.Validate()) {
				return View(model: "malsys@marekfiser.cz");
			}
			else {
				ModelState.AddModelError("", "Captcha invalid.");
				return View((string)null);
			}

		}


		[Authorize(Roles = UserRoles.ViewFeedbacks)]
		public virtual ActionResult List(int page = 1) {

			if (page <= 0) {
				page = 1;
			}

			return View(feedbackDb.Feedbacks.OrderByDescending(x => x.SubmitDate).AsPagination(page));
		}


	}
}

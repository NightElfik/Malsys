using System.Web.Mvc;
using Malsys.Web.Entities;
using Malsys.Web.Models;
using Malsys.Web.Infrastructure;
using Microsoft.Web.Helpers;

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

			ReCaptcha.PublicKey = appSettingsProvider["ReCaptcha_PublicKey"];
			ReCaptcha.PrivateKey = appSettingsProvider["ReCaptcha_PrivateKey"];
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

	}
}

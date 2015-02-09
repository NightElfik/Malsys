using System;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;
using Malsys.Web.Entities;
using Malsys.Web.Infrastructure;
using Malsys.Web.Models;
using MvcContrib.Pagination;

namespace Malsys.Web.Controllers {
	public partial class FeedbackController : Controller {

		private readonly IFeedbackDb feedbackDb;
		private readonly IDateTimeProvider dateTimeProvider;
		private readonly IUsersDb usersDb;
		private readonly ICaptcha captcha;


		public FeedbackController(IFeedbackDb feedbackDb, IDateTimeProvider dateTimeProvider,
				IUsersDb usersDb, ICaptcha captcha) {

			this.feedbackDb = feedbackDb;
			this.dateTimeProvider = dateTimeProvider;
			this.usersDb = usersDb;
			this.captcha = captcha;
		}


		public virtual ActionResult Index() {
			return View(new FeedbackModel() { Captcha = captcha });
		}

		[HttpPost]
		public virtual ActionResult Index(FeedbackModel model) {
			model.Captcha = captcha;

			if (!ModelState.IsValid) {
				return View(model);
			}

			if (!captcha.Validate(ControllerContext.HttpContext)) {
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

			var fbRole = usersDb.Roles.Where(r => r.NameLowercase == UserRoles.ViewFeedbacks).SingleOrDefault();
			if (fbRole != null) {
				foreach (var user in fbRole.Users) {
					if (string.IsNullOrWhiteSpace(user.Email)) {
						continue;
					}

					try {
						WebMail.Send(user.Email, "Feedback submitted on Malsys -- " + model.Subject.Trim(), model.Message.Trim());
					}
					catch (Exception ex) {
						Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Failed to send feedback e-mail", ex));
					}
				}

			}

			return RedirectToAction("Thanks");
		}

		public virtual ActionResult Thanks() {
			return View();
		}


		public virtual ActionResult EmailToAuthor() {
			return View(captcha);
		}

		[HttpPost]
		[ActionName("EmailToAuthor")]
		public virtual ActionResult EmailToAuthorPost() {

			if (captcha.Validate(ControllerContext.HttpContext)) {
				return View(model: "malsys@marekfiser.cz");
			}
			else {
				ModelState.AddModelError("", "Captcha invalid.");
				return View(captcha);
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

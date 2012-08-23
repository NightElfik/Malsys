using System;
using System.Linq;
using System.Web.Mvc;
using Malsys.Web.Entities;
using Malsys.Web.Models;
using MvcContrib.Pagination;

namespace Malsys.Web.Controllers {
	public partial class DiscussionController : Controller {


		private readonly IDiscussionRepository discusRepo;


		public DiscussionController(IDiscussionRepository discusRepo) {
			this.discusRepo = discusRepo;
		}


		public virtual ActionResult Index() {

			var cats = discusRepo.DiscusCategories.OrderBy(c => c.CategoryId);

			return View(cats);
		}


		public virtual ActionResult Category(int? id = null, int? page = null) {

			if (page == null || page <= 0) {
				page = 1;
			}

			if (id == null || id < 0) {
				return HttpNotFound();
			}

			var cat = discusRepo.DiscusCategories.Where(x => x.CategoryId == id).SingleOrDefault();

			if (cat == null) {
				return HttpNotFound();
			}

			var threads = discusRepo.DiscusThreads
				.Where(x => x.CategoryId == cat.CategoryId)
				.OrderByDescending(x => x.LastUpdateDate)
				.AsPagination(page.Value, 10);

			return View(new Tuple<DiscusCategory, IPagination<DiscusThread>>(cat, threads));
		}

		[HttpPost, ValidateInput(false)]
		public virtual ActionResult NewMessage(NewDiscusMessageModel model) {

			DiscusMessage msg = null;
			string clientMessage = "";

			if (ModelState.IsValid) {
				msg = discusRepo.AddMessage(model.ThreadId, model.Message, User.Identity.IsAuthenticated ? User.Identity.Name : null, model.AuthorNameNonRegistered);
			}
			else {
				clientMessage = ModelState.Values.SelectMany(x => x.Errors.Select(y => y.ErrorMessage)).JoinToString("\n");
			}

			return Json(new {
				error = msg == null,
				message = clientMessage
			});
		}

		[HttpPost, ValidateInput(false)]
		public virtual ActionResult NewThread(NewDiscusThreadModel threadModel) {

			DiscusThread thread = null;
			string clientMessage = "";

			string authAutorName = User.Identity.IsAuthenticated ? User.Identity.Name : null;

			if (ModelState.IsValid) {
				var cat = discusRepo.DiscusCategories.Where(c => c.CategoryId == threadModel.CategoryId).SingleOrDefault();
				// thread will be added if category is general or user is moderator => non-moderators can add threads only in general
				if (cat != null && (cat.Is(DiscussionCategory.General) || User.IsInRole(UserRoles.DiscusModerator))) {
					thread = discusRepo.CreateThreadWithFirstMessage(threadModel.CategoryId, threadModel.Title, authAutorName,
						threadModel.AuthorNameNonRegistered, threadModel.FirstMessage);
					discusRepo.DiscusDb.Log("DiscusNewThread", ActionLogSignificance.Low, thread.ThreadId.ToString());
				}
				else {
					clientMessage = "Invalid category";
				}
			}
			else {
				clientMessage = ModelState.Values.SelectMany(x => x.Errors.Select(y => y.ErrorMessage)).JoinToString("\n");
			}

			return Json(new {
				error = thread == null,
				threadId = thread != null ? thread.ThreadId : -1,
				message = clientMessage
			});
		}


		public virtual ActionResult Thread(int id) {

			var thread = discusRepo.DiscusThreads
				.Where(x => x.ThreadId == id)
				.SingleOrDefault();

			if (thread == null) {
				return HttpNotFound();
			}

			return View(new DiscusThreadViewModel() {
				Thread = thread,
				Messages = thread.DiscusMessages.OrderBy(x => x.CreationDate) });
		}

		[ChildActionOnly, ValidateInput(false)]
		public virtual ActionResult AutoThreadInline(string threadName, string threadTitle, DiscussionCategory category) {

			var cat = discusRepo.GetKnownDiscussCat(category);

			if (cat == null) {
				return View(new DiscusThreadViewModel());
			}

			var thread = discusRepo.DiscusThreads
				.Where(x => x.ThreadName == threadName && x.DiscusCategory.CategoryId == cat.CategoryId)
				.SingleOrDefault();

			if (thread == null) {
				thread = discusRepo.CreateThread(cat.CategoryId, threadTitle, null, null, threadName);
			}
			else {
				if (thread.Title != threadTitle) {
					thread.Title = threadTitle;
					discusRepo.SaveChanges();
				}
			}

			return View(new DiscusThreadViewModel() {
				Thread = thread,
				Messages = thread.DiscusMessages.OrderBy(x => x.CreationDate)
			});
		}

		[Authorize(Roles = UserRoles.DiscusModerator)]
		public virtual ActionResult DeleteThread(int id, string returnUrl = null) {

			var thread = discusRepo.DiscusThreads.Where(x => x.ThreadId == id).SingleOrDefault();
			if (thread == null) {
				return HttpNotFound();
			}

			return View(thread);
		}

		[HttpPost]
		[Authorize(Roles = UserRoles.DiscusModerator)]
		public virtual ActionResult DeleteThread(int id, string returnUrl = null, string yes = null) {

			if (yes != null) {
				var thread = discusRepo.DiscusThreads.Where(x => x.ThreadId == id).SingleOrDefault();
				if (thread == null) {
					return HttpNotFound();
				}

				discusRepo.DiscusDb.DeleteDiscusThread(thread);
				discusRepo.SaveChanges();
			}

			if (Url.IsUrlSafeToRedirect(returnUrl)) {
				return Redirect(returnUrl);
			}
			else {
				return RedirectToAction(MVC.Home.Index());
			}
		}

		[Authorize(Roles = UserRoles.DiscusModerator)]
		public virtual ActionResult DeleteMessage(int id, string returnUrl = null) {

			var msg = discusRepo.DiscusMessages.Where(x => x.MessageId == id).SingleOrDefault();
			if (msg == null) {
				return HttpNotFound();
			}

			return View(msg);
		}

		[HttpPost]
		[Authorize(Roles = UserRoles.DiscusModerator)]
		public virtual ActionResult DeleteMessage(int id, string returnUrl = null, string yes = null) {

			if (yes != null) {
				var msg = discusRepo.DiscusMessages.Where(x => x.MessageId == id).SingleOrDefault();
				if (msg == null) {
					return HttpNotFound();
				}
				discusRepo.DiscusDb.DeleteDiscusMessage(msg);
				discusRepo.SaveChanges();
			}

			if (Url.IsUrlSafeToRedirect(returnUrl)) {
				return Redirect(returnUrl);
			}
			else {
				return RedirectToAction(MVC.Home.Index());
			}
		}


		[Authorize(Roles = UserRoles.DiscusModerator)]
		public virtual ActionResult LockThread(int id, string returnUrl = null) {

			var thread = discusRepo.DiscusThreads.Where(x => x.ThreadId == id).SingleOrDefault();
			if (thread == null) {
				return HttpNotFound();
			}

			return View(thread);
		}

		[HttpPost]
		[Authorize(Roles = UserRoles.DiscusModerator)]
		public virtual ActionResult LockThread(int id, string returnUrl = null, string yes = null) {

			if (yes != null) {
				var thread = discusRepo.DiscusThreads.Where(x => x.ThreadId == id).SingleOrDefault();
				if (thread == null) {
					return HttpNotFound();
				}

				thread.IsLocked = !thread.IsLocked;
				discusRepo.SaveChanges();
			}

			if (Url.IsUrlSafeToRedirect(returnUrl)) {
				return Redirect(returnUrl);
			}
			else {
				return RedirectToAction(MVC.Home.Index());
			}
		}

	}
}

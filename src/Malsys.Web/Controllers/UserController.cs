using System.Linq;
using System.Web.Mvc;
using Malsys.Web.Models;
using MvcContrib.Pagination;
using System.Collections.Generic;

namespace Malsys.Web.Controllers {
	[Authorize]
	public partial class UserController : Controller {

		private readonly IMalsysInputRepository malsysInputRepository;

		public UserController(IMalsysInputRepository malsysInputRepository) {

			this.malsysInputRepository = malsysInputRepository;
		}


		public virtual ActionResult Index() {
			return View();
		}

		public virtual ActionResult SavedInputs(int page = 1) {

			if (page <= 0) {
				page = 1;
			}

			string userLower = User.Identity.Name.Trim().ToLowerInvariant();

			return View(malsysInputRepository.InputDb.SavedInputs
				.Where(x => x.User.NameLowercase == userLower)
				.OrderByDescending(x => x.Date)
				.AsPagination(page));
		}

		[HttpPost]
		public virtual ActionResult DeleteInput(string id, bool confirm = false) {

			string userLower = User.Identity.Name.Trim().ToLowerInvariant();
			bool admin = User.IsInRole(UserRoles.Administrator);

			var malsysInput = malsysInputRepository.InputDb.SavedInputs
				.Where(x => x.RandomId == id)
				.Where(x => admin || x.User.NameLowercase == userLower)
				.SingleOrDefault();

			if (malsysInput == null) {
				return RedirectToAction(Actions.SavedInputs());
			}

			if (confirm) {
				malsysInputRepository.InputDb.DeleteSavedInput(malsysInput);
				malsysInputRepository.InputDb.SaveChanges();
				return RedirectToAction(Actions.SavedInputs());
			}
			else {
				return View(new KeyValuePair<string, string>(id, malsysInput.Source));
			}

		}

	}
}

using System.Linq;
using System.Web.Mvc;
using Malsys.Web.Models;
using MvcContrib.Pagination;

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

			string user = User.Identity.Name.Trim().ToLower();

			return View(malsysInputRepository.InputDb.SavedInputs
				.Where(x => x.User.NameLowercase == user)
				.OrderByDescending(x => x.Date)
				.AsPagination(page));
		}

	}
}

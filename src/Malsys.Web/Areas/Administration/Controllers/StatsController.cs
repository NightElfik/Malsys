using System.Linq;
using System.Web.Mvc;
using Malsys.Web.Areas.Administration.Models;
using Malsys.Web.Entities;
using MvcContrib.Pagination;

namespace Malsys.Web.Areas.Administration.Controllers {
	[Authorize(Roles = UserRoles.ViewStats)]
	public partial class StatsController : Controller {

		private readonly IInputDb inputDb;
		private readonly IUsersDb usersDb;


		public StatsController(IInputDb inputDb, IUsersDb usersDb) {
			this.inputDb = inputDb;
			this.usersDb = usersDb;
		}


		public virtual ActionResult Index() {
			return View();
		}

		public virtual ActionResult Input(int page = 1) {

			if (page <= 0) {
				page = 1;
			}

			var result = inputDb.InputProcesses
				.OrderByDescending(p => p.ProcessDate)
				.Select(p => new InputProcessesHistoryItem {
					ProcessId = p.InputProcessId,
					ParentProcessId = p.ParentInputProcessId,
					User = p.User.Name,
					Date = p.ProcessDate,
					Duration = p.Duration,
					SourceId = p.CanonicInput.CanonicInputId,
					Source = p.CanonicInput.Source
				})
				.AsPagination(page, 10);

			return View(result);
		}


		public virtual ActionResult SavedInputs(int page = 1) {

			if (page <= 0) {
				page = 1;
			}

			var result = inputDb.SavedInputs
				.OrderByDescending(x => x.EditDate)
				.Select(x => new SavedInputModel {
					SavedInputId = x.SavedInputId,
					UrlId = x.UrlId,
					User = x.User.Name,
					EditDate = x.EditDate,
					Duration = x.Duration,
					Source = x.Source
				})
				.AsPagination(page, 10);

			return View(result);
		}

	}
}

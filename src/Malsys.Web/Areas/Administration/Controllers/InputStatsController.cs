using System.Linq;
using System.Web.Mvc;
using Malsys.Web.Entities;
using Malsys.Web.Models;
using MvcContrib.Pagination;

namespace Malsys.Web.Areas.Administration.Controllers {
	[Authorize]
	public partial class InputStatsController : Controller {

		private readonly IInputDb inputDb;
		private readonly IUsersDb usersDb;


		public InputStatsController(IInputDb inputDb, IUsersDb usersDb) {
			this.inputDb = inputDb;
			this.usersDb = usersDb;
		}

		public virtual ActionResult Index(int page = 1) {

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
					SourceId = p.CanonicInput.CanonicInputId,
					Source = p.CanonicInput.Source
				})
				.AsPagination(page, 10);

			return View(result);
		}


	}
}

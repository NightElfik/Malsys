using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Malsys.Web.Entities;
using Malsys.Web.Models;
using MvcContrib.Pagination;

namespace Malsys.Web.Controllers.Administration {
	[Authorize]
	public class InputStatsController : Controller {

		private readonly IInputDb inputDb;
		private readonly IUsersDb usersDb;


		public InputStatsController(IInputDb inputDb, IUsersDb usersDb) {
			this.inputDb = inputDb;
			this.usersDb = usersDb;
		}

		public ActionResult Index(int page = 1) {

			if (page <= 0) {
				page = 1;
			}

			var result = inputDb.InputProcesses
				.Join(usersDb.Users, p => p.UserId, u => u.UserId, (p, u) => new InputProcessesHistoryItem {
					ProcessId = p.InputProcessId,
					User = u.Name,
					Date = p.ProcessDate,
					SourceId = p.CanonicInput.CanonicInputId,
					Source = p.CanonicInput.Source
				})
				.OrderByDescending(p => p.Date)
				.AsPagination(page, 5);

			return View(result);
		}


	}
}

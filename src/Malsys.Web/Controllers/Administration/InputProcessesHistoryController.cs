using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Malsys.Web.Entities;
using Malsys.Web.Models;

namespace Malsys.Web.Controllers.Administration {
	[Authorize]
	public class InputProcessesHistoryController : Controller {

		private readonly IInputDb inputDb;
		private readonly IUsersDb usersDb;


		public InputProcessesHistoryController(IInputDb inputDb, IUsersDb usersDb) {
			this.inputDb = inputDb;
			this.usersDb = usersDb;
		}

		public ActionResult Index(int page = 0, int pageSize = 10, bool anonymous = false) {

			IList<InputProcessesHistoryItem> list;

			if (anonymous) {
				list = inputDb.InputProcesses
					.Where(p => p.UserId == null)
					.OrderByDescending(p => p.ProcessDate)
					.Skip(page * pageSize)
					.Take(pageSize)
					.Select(p => new InputProcessesHistoryItem {
						ProcessId = p.InputProcessId,
						User = "",
						Date = p.ProcessDate,
						SourceId = p.CanonicInput.CanonicInputId,
						Source = p.CanonicInput.Source
					})
					.ToList();
			}
			else {
				list = inputDb.InputProcesses
					.OrderByDescending(p => p.ProcessDate)
					.Skip(page * pageSize)
					.Take(pageSize)
					.Join(usersDb.Users, ip => ip.UserId, u => u.UserId, (ip, u) => new InputProcessesHistoryItem {
						ProcessId = ip.InputProcessId,
						User = u.Name,
						Date = ip.ProcessDate,
						SourceId = ip.CanonicInput.CanonicInputId,
						Source = ip.CanonicInput.Source
					})
					.ToList();
			}

			return View(new Tuple<int, bool, IList<InputProcessesHistoryItem>>(page, anonymous, list));
		}

	}
}

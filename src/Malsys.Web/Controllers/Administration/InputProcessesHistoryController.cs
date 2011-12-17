using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Malsys.Web.Entities;

namespace Malsys.Web.Controllers.Administration {
	public class InputProcessesHistoryController : Controller {

		private readonly IInputDb inputDb;


		public InputProcessesHistoryController(IInputDb inputDb) {
			this.inputDb = inputDb;
		}

		public ActionResult Index(int from = 0, int pageSize = 20) {

			var list = inputDb.InputProcesses
				.Skip(from)
				.Take(pageSize)
				.ToList();

			return View(list);
		}

	}
}

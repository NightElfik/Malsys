// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Malsys.Web.Entities;
using Malsys.Web.Models;

namespace Malsys.Web.Controllers {
	public partial class HomeController : Controller {


		private readonly IInputDb inputDb;
		private readonly IDiscusDb discusDb;
		private readonly LoadedPlugins loadedPlugins;


		public HomeController(IInputDb inputDb, IDiscusDb discusDb, LoadedPlugins loadedPlugins) {
			this.inputDb = inputDb;
			this.discusDb = discusDb;
			this.loadedPlugins = loadedPlugins;
		}


		public virtual ActionResult Index() {


			var cat = discusDb.GetKnownDiscussCat(DiscussionCategory.News);
			var news = discusDb.DiscusThreads
				.Where(x => x.CategoryId == cat.CategoryId)
				.OrderByDescending(x => x.CreationDate)
				.Take(3);

			return View(news);
		}

		public virtual ActionResult LoadedPlugins() {
			return View(loadedPlugins);
		}

		public virtual ActionResult WhyWebgl() {
			return View();
		}

		public virtual ActionResult Thesis() {
			return View();
		}

	}
}

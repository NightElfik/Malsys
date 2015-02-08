using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Malsys.Web.ArticleTools;
using Malsys.Web.Entities;
using Malsys.Web.Models;

namespace Malsys.Web.Controllers {
	public partial class HomeController : Controller {

		private static SectionsManager createSectionsManager() {
			return new SectionsManager((sec, url) => {
				Section parent = sec;
				while (parent.Parent.Parent != null) {
					parent = parent.Parent;
				}
				string baseHref = url.Action(MVC.Home.NewsArchive(StaticUrl.UrlizeString(sec.Name)));

				if (sec.Level >= 3) {
					baseHref += "#" + sec.HtmlId;
				}
				return baseHref;
			});
		}


		private readonly Lazy<IInputDb> inputDb;
		private readonly Lazy<IUsersDb> usersDb;
		private readonly Lazy<IActionLogDb> actionLogDb;


		private readonly LoadedPlugins loadedPlugins;


		public HomeController(LoadedPlugins loadedPlugins, Lazy<IInputDb> inputDb,
				Lazy<IUsersDb> usersDb, Lazy<IActionLogDb> actionLogDb) {

			this.loadedPlugins = loadedPlugins;

			this.inputDb = inputDb;
			this.usersDb = usersDb;
			this.actionLogDb = actionLogDb;
		}


		public virtual ActionResult Index() {
			NewsArchiveViewModel viewModel = new NewsArchiveViewModel();
			viewModel.SectionsManager = createSectionsManager();
			new TocFetcher().FetchToc(ControllerContext, Views.ViewNames.NewsData, viewModel);
			viewModel.SectionsManager.SetDisplayOfSection(0);

			return View(viewModel);
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

		public virtual ActionResult NewsArchive(string id = null) {
			NewsArchiveViewModel viewModel = new NewsArchiveViewModel();
			viewModel.SectionsManager = createSectionsManager();

			new TocFetcher().FetchToc(ControllerContext, Views.ViewNames.NewsData, viewModel);

			if (id == null) {
				return View(viewModel);
			}

			var currSec = viewModel.SectionsManager.RootSection.Subsections
				.FirstOrDefault(s => StaticUrl.UrlizeString(s.Name) == id);

			if (currSec == null) {
				// TODO: Display warning that project was not found.
				return HttpNotFound();
			}

			viewModel.SectionsManager.SetCurrentSection(currSec);
			return View(Views.ViewNames.NewsDetail, viewModel);
		}

		public virtual ActionResult Stats() {

			var processHistByMonth = inputDb.Value.InputProcesses
				.Select(p => p.ProcessDate.Year * 100 + p.ProcessDate.Month)
				.GroupBy(d => d)
				.OrderBy(d => d.Key)
				.AsEnumerable()
				.Select(g => new KeyValuePair<string, int>(
					(g.Key / 100).ToString("0000") + "-" + (g.Key % 100).ToString("00"),
					g.Count()));

			var savedOverTime = inputDb.Value.SavedInputs
				.Where(i => i.IsPublished && !i.IsDeleted)
				.Select(i => i.CreationDate.Year * 100 + i.CreationDate.Month)
				.GroupBy(i => i)
				.OrderBy(i => i.Key)
				.AsEnumerable()
				.Select(g => new KeyValuePair<string, int>(
					(g.Key / 100).ToString("0000") + "-" + (g.Key % 100).ToString("00"),
					g.Count()));

			int runningTotal = 0;
			savedOverTime = savedOverTime.Select(kvp => {
				runningTotal += kvp.Value;
				return new KeyValuePair<string, int>(kvp.Key, runningTotal);
			});

			return View(new StatsViewModel() {
				ProcessHistByMonth = processHistByMonth,
				SavedLsysByMonth = savedOverTime,
			});
		}

	}
}

// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
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



		private readonly LoadedPlugins loadedPlugins;


		public HomeController(LoadedPlugins loadedPlugins) {
			this.loadedPlugins = loadedPlugins;
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

	}
}

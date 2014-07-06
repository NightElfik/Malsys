using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Malsys.Processing;
using Malsys.SemanticModel.Evaluated;
using Malsys.Web.Areas.Documentation.Models;
using Malsys.Web.ArticleTools;
using Malsys.Web.Models.Lsystem;

namespace Malsys.Web.Areas.Documentation.Controllers {
	public partial class ArticlesController : Controller {

		private static SectionsManager createSectionsManager(HelpArticle article) {
			return new SectionsManager((sec, url) => {
				Section parent = sec;
				while (parent.Parent.Parent != null) {
					parent = parent.Parent;
				}
				string baseHref = url.Action(MVC.Documentation.Articles.Article(article.Url, parent.IsFirst ? null : parent.HtmlId));

				if (sec.Level >= 3) {
					baseHref += "#" + sec.HtmlId;
				}
				return baseHref;
			});
		}


		private readonly HelpArticleViewModel viewModel;


		public ArticlesController(IEnumerable<HelpArticle> articles, ProcessManager processManager, InputBlockEvaled stdLib) {
			viewModel = new HelpArticleViewModel() {
				AllArticles = articles,
				LsystemProcessor =  new SimpleLsystemProcessor(processManager, stdLib),
				ProcessManager = processManager,
			};
		}

		public virtual ActionResult Article(string name, string part = null) {
			var currArticle = viewModel.AllArticles
				.FirstOrDefault(x => x.Url == name && !string.IsNullOrEmpty(x.ViewName));
 
			if (currArticle == null) {
				// TODO: Display warning that project was not found.
				return HttpNotFound();
			}

			viewModel.SectionsManager = createSectionsManager(currArticle);

			viewModel.DisplayedArticle = currArticle;
			new TocFetcher().FetchToc(ControllerContext, viewModel.DisplayedArticle.ViewName, viewModel);
			int partNumber = part == null ? 1 : parsePartNumber(part);
			--partNumber;

			if (partNumber < 0 || partNumber >= viewModel.SectionsManager.RootSection.SubsectionsCount) {
				// TODO: Display warning that project was not found.
				return HttpNotFound();
			}

			if (partNumber == 0) {
				if (part != null) {
					return RedirectToActionPermanent(Actions.Article(name));
				}
			}
			else {
				string partUrl = viewModel.SectionsManager.RootSection.Subsections[partNumber].HtmlId;
				if (partUrl != part) {
					return RedirectToActionPermanent(Actions.Article(name, partUrl));
				}
			}

			viewModel.SectionsManager.SetDisplayOfSection(partNumber);
			return View(viewModel);
		}


		private int parsePartNumber(string part) {
			int dashIndex = part.IndexOf('-');
			int partNumber;
			if (dashIndex <= 0) {
				if (int.TryParse(part, out partNumber)) {
					return partNumber;
				}
			}
			else {
				if (int.TryParse(part.Substring(0, dashIndex), out partNumber)) {
					return partNumber;
				}
			}

			return -1;
		}

	}
}

using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Malsys.Web.ArticleTools;
using Malsys.Web.Models;

namespace Malsys.Web.Controllers {
	public partial class DevDiaryController : Controller {

		private static SectionsManager createSectionsManager(DevDiaryEntry article) {
			return new SectionsManager((sec, url) => {
				Section parent = sec;
				while (parent.Parent.Parent != null) {
					parent = parent.Parent;
				}
				string baseHref = url.Action(MVC.DevDiary.Entry(article.Url, parent.IsFirst ? null : parent.HtmlId));

				if (sec.Level >= 3) {
					baseHref += "#" + sec.HtmlId;
				}
				return baseHref;
			});
		}


		private readonly DevDiaryViewModel viewModel;


		public DevDiaryController(IEnumerable<DevDiaryEntry> entries) {
			viewModel = new DevDiaryViewModel() {
				AllEntries = entries,
			};
		}

		public virtual ActionResult Index() {
			return View(viewModel);
		}


		public virtual ActionResult Entry(string name, string part = null) {
			var currEntry = viewModel.AllEntries
				.FirstOrDefault(x => x.Url == name && !string.IsNullOrEmpty(x.ViewName));

			if (currEntry == null) {
				// TODO: Display warning that project was not found.
				return HttpNotFound();
			}

			viewModel.SectionsManager = createSectionsManager(currEntry);

			viewModel.DisplayedEntry = currEntry;
			new TocFetcher().FetchToc(ControllerContext, viewModel.DisplayedEntry.ViewName, viewModel);
			int partNumber = part == null ? 1 : parsePartNumber(part);
			--partNumber;

			if (partNumber < 0 || partNumber >= viewModel.SectionsManager.RootSection.SubsectionsCount) {
				// TODO: Display warning that project was not found.
				return HttpNotFound();
			}

			if (partNumber == 0) {
				if (part != null) {
					return RedirectToActionPermanent(Actions.Entry(name));
				}
			}
			else {
				string partUrl = viewModel.SectionsManager.RootSection.Subsections[partNumber].HtmlId;
				if (partUrl != part) {
					return RedirectToActionPermanent(Actions.Entry(name, partUrl));
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

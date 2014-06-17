using System.IO;
using System.Web.Mvc;
using Malsys.Web.Models;

namespace Malsys.Web.ArticleTools {
	public class TocFetcher {

		public void FetchToc(ControllerContext controllerContext, string partialViewName, SectionsManager sectionsManager) {
			var model = new ArticleModelBase();
			model.SectionsManager = sectionsManager;
			FetchToc(controllerContext, partialViewName, model);
		}

		public void FetchToc(ControllerContext controllerContext, string partialViewName, ArticleModelBase model) {
			var viewResult = ViewEngines.Engines.FindPartialView(controllerContext, partialViewName);
			controllerContext.Controller.ViewData.Model = model;

			var ms = new MemoryStream();
			using (var sw = new StreamWriter(ms)) {
				var viewContext = new ViewContext(controllerContext, viewResult.View,
					controllerContext.Controller.ViewData, controllerContext.Controller.TempData, sw);
				viewResult.View.Render(viewContext, sw);
			}

			viewResult.ViewEngine.ReleaseView(controllerContext, viewResult.View);
		}

	}
}
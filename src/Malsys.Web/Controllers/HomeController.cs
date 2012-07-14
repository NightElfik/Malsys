/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System.Linq;
using System.Web.Mvc;
using Malsys.Web.Entities;
using Malsys.Web.Models;

namespace Malsys.Web.Controllers {
	public partial class HomeController : Controller {


		private readonly IInputDb inputDb;

		public HomeController(IInputDb inputDb) {
			this.inputDb = inputDb;
		}


		public virtual ActionResult Index() {

			var input = inputDb.SavedInputs
				.Where(x => x.IsPublished && !x.IsDeleted)
				.Where(x => x.MimeType == MimeType.Image.SvgXml || x.MimeType == MimeType.Application.Javascript || x.MimeType == MimeType.Image.Png)
				.Where(x => (double)x.RatingSum / ((double)x.RatingCount + 1) > 2)
				.OrderBy(x => x.SavedInputId)
				.RandomOrDefault();

			GalleryThumbnailModel model = null;
			if (input != null) {
				model = new GalleryThumbnailModel() {
					SavedInput = input,
					MaxWidth = 256,
					MaxHeight = 256
				};
			}

			return View(model);
		}

		public virtual ActionResult WhyWebgl() {
			return View();
		}

		public virtual ActionResult Sitemap() {
			return View();
		}

		public virtual ActionResult Thesis() {
			return View();
		}

	}
}

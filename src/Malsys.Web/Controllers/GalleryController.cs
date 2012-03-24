using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Malsys.Web.Models;
using Malsys.Web.Infrastructure;

namespace Malsys.Web.Controllers {
	public partial class GalleryController : Controller {


		private readonly string workDir;


		private readonly IMalsysInputRepository malsysInputRepository;



		public GalleryController(IMalsysInputRepository malsysInputRepository, IAppSettingsProvider appSettingsProvider) {

			this.malsysInputRepository = malsysInputRepository;

			workDir = appSettingsProvider[AppSettingsKeys.GalleryWorkDir];

		}

		public virtual ActionResult Index() {
			return View();
		}

		public virtual ActionResult Puhlish() {
			return View();
		}

	}
}

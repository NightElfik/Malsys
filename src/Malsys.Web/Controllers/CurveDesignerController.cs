using System.Web.Mvc;
using Malsys.Web.Models;

namespace Malsys.Web.Controllers {
	public partial class CurveDesignerController : Controller {

		public virtual ActionResult Index() {
			return View();
		}


		public virtual ActionResult Show(int size, bool? autoMirror, bool? checkFassRules) {
			return View(new DesignerModel() {
				Size =  MathHelper.Clamp(size, 2, 9),
				AutoMirror = autoMirror ?? false,
				CheckFassRules = checkFassRules ?? false
			});
		}

	}
}

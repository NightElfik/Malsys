using System.Web.Mvc;
using Malsys.Web.Entities;
using Malsys.Web.Models;

namespace Malsys.Web.Controllers {
	public partial class CurveDesignerController : Controller {


		private readonly IActionLogDb actionLogDb;

		public CurveDesignerController(IActionLogDb actionLogDb) {
			this.actionLogDb = actionLogDb;
		}

		public virtual ActionResult Index() {
			return View();
		}


		public virtual ActionResult Show(int size, bool? autoMirror, bool? checkFassRules) {
			actionLogDb.Log("DesignerShow", ActionLogSignificance.Low, size.ToString());
			return View(new DesignerModel() {
				Size = MathHelper.Clamp(size, 2, 9),
				AutoMirror = autoMirror ?? false,
				CheckFassRules = checkFassRules ?? false
			});
		}

	}
}

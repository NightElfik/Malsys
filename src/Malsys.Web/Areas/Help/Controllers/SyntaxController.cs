using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Malsys.Web.Areas.Help.Controllers {
	public partial class SyntaxController : Controller {

		public virtual ActionResult Index() {
			return View();
		}

		public virtual ActionResult Grammar() {
			return View();
		}

		public virtual ActionResult GrammarRegexps() {
			return View();
		}

	}
}

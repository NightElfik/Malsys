using System.Web.Mvc;
using Malsys.Processing.Components.Interpreters.TwoD;
using Malsys.Reflection;
using Malsys.Web.Areas.Help.Models;

namespace Malsys.Web.Areas.Help.Controllers {
	public partial class HomeController : Controller {

		private readonly XmlDocReader xmlDocReader;


		public HomeController(XmlDocReader xmlDocReader) {
			this.xmlDocReader = xmlDocReader;
		}

		public virtual ActionResult Index() {
			return View();
		}

		public virtual ActionResult BasicRewriting() {
			return View();
		}

		public virtual ActionResult BasicInterpretation() {
			return View(ComponentModel.FromType(typeof(Interpreter2D), xmlDocReader));
		}


	}
}

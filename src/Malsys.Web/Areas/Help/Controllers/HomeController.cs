using System.Web.Mvc;
using Malsys.Processing.Components.Interpreters;
using Malsys.Reflection;
using Malsys.Web.Areas.Help.Models;
using Malsys.Processing;
using Malsys.SemanticModel.Evaluated;
using Malsys.Web.Models;

namespace Malsys.Web.Areas.Help.Controllers {
	public partial class HomeController : Controller {

		private readonly XmlDocReader xmlDocReader;
		private readonly SimpleLsystemProcessor simpleLsystemProcessor;


		public HomeController(XmlDocReader xmlDocReader, ProcessManager processManager, InputBlock stdLib) {
			this.xmlDocReader = xmlDocReader;
			simpleLsystemProcessor = new SimpleLsystemProcessor(processManager, stdLib);
		}

		public virtual ActionResult Index() {
			return View();
		}

		public virtual ActionResult BasicRewriting() {
			return View(simpleLsystemProcessor);
		}

		public virtual ActionResult BasicInterpretation() {
			return View(ComponentModel.FromType(typeof(Interpreter2D), xmlDocReader));
		}


	}
}

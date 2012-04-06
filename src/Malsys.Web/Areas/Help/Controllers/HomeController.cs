using System.Web.Mvc;
using Malsys.Processing;
using Malsys.Processing.Components.Interpreters;
using Malsys.Reflection;
using Malsys.SemanticModel.Evaluated;
using Malsys.Web.Areas.Help.Models;
using Malsys.Web.Models.Lsystem;
using Malsys.Reflection.Components;

namespace Malsys.Web.Areas.Help.Controllers {
	public partial class HomeController : Controller {

		private readonly ComponentXmlDocLoader xmlDocLoader;
		private readonly SimpleLsystemProcessor simpleLsystemProcessor;
		private readonly InputBlockEvaled stdLib;
		private readonly ProcessManager processManager;


		public HomeController(ComponentXmlDocLoader xmlDocLoader, ProcessManager processManager, InputBlockEvaled stdLib) {

			this.xmlDocLoader = xmlDocLoader;
			this.stdLib = stdLib;
			this.processManager = processManager;

			simpleLsystemProcessor = new SimpleLsystemProcessor(processManager, stdLib);

		}

		public virtual ActionResult Index() {
			return View();
		}

		public virtual ActionResult BasicRewriting() {
			return View(simpleLsystemProcessor);
		}

		public virtual ActionResult BasicInterpretation() {

			var intMeta = processManager.ComponentResolver.ResolveComponentMetadata("TurtleInterpreter");
			xmlDocLoader.LoadXmlDocFor(intMeta);

			return View(new BasicInterpretationModel() {
				SimpleLsystemProcessor = simpleLsystemProcessor,
				StdLib = stdLib,
				Interpreter = intMeta
			});
		}

		public virtual ActionResult Faq() {
			return View();
		}

	}
}

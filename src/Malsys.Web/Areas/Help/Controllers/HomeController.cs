using System.Web.Mvc;
using Malsys.Processing;
using Malsys.SemanticModel.Evaluated;
using Malsys.Web.Areas.Help.Models;
using Malsys.Web.Models.Lsystem;

namespace Malsys.Web.Areas.Help.Controllers {
	[OutputCache(CacheProfile = "HelpCache")]
	public partial class HomeController : Controller {

		private readonly SimpleLsystemProcessor simpleLsystemProcessor;
		private readonly InputBlockEvaled stdLib;
		private readonly ProcessManager processManager;


		public HomeController(ProcessManager processManager, InputBlockEvaled stdLib) {

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

		public virtual ActionResult LsystemSymbols() {
			return View(simpleLsystemProcessor);
		}

		public virtual ActionResult BasicInterpretation() {

			var intMeta = processManager.ComponentResolver.ResolveComponentMetadata("TurtleInterpreter");

			return View(new BasicInterpretationModel() {
				SimpleLsystemProcessor = simpleLsystemProcessor,
				StdLib = stdLib,
				Interpreter = intMeta
			});
		}

		public virtual ActionResult Faq() {
			return View();
		}

		public virtual ActionResult ProcessConfigurations() {
			return View(simpleLsystemProcessor);
		}

	}
}

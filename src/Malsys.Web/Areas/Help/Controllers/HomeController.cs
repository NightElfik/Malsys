using System.Web.Mvc;
using Malsys.Processing;
using Malsys.SemanticModel.Evaluated;
using Malsys.Web.Areas.Help.Models;
using Malsys.Web.Models.Lsystem;
using Malsys.Web.Models;

namespace Malsys.Web.Areas.Help.Controllers {
	[OutputCache(CacheProfile = "HelpCache")]
	public partial class HomeController : Controller {

		private readonly SimpleLsystemProcessor simpleLsystemProcessor;
		private readonly InputBlockEvaled stdLib;
		private readonly ProcessManager processManager;
		private readonly MalsysStdLibSource malsysStdLibSource;


		public HomeController(ProcessManager processManager, InputBlockEvaled stdLib, MalsysStdLibSource malsysStdLibSource) {

			this.stdLib = stdLib;
			this.processManager = processManager;
			this.malsysStdLibSource = malsysStdLibSource;

			simpleLsystemProcessor = new SimpleLsystemProcessor(processManager, stdLib);

		}

		public virtual ActionResult Index() {
			return View();
		}

		public virtual ActionResult Sitemap() {
			return View();
		}

		public virtual ActionResult Faq() {
			return View(simpleLsystemProcessor);
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

		public virtual ActionResult ProcessConfigurations() {
			return View(simpleLsystemProcessor);
		}

		public virtual ActionResult StdLib() {
			return View(malsysStdLibSource);
		}

	}
}

using System.Web.Mvc;
using Malsys.Processing;
using Malsys.SemanticModel.Evaluated;
using Malsys.Web.Models.Lsystem;

namespace Malsys.Web.Areas.Help.Controllers {
	public partial class SyntaxReferenceController : Controller {

		private readonly SimpleLsystemProcessor simpleLsystemProcessor;


		public SyntaxReferenceController(ProcessManager processManager, InputBlockEvaled stdLib) {

			simpleLsystemProcessor = new SimpleLsystemProcessor(processManager, stdLib);
		}


		public virtual ActionResult Index() {
			return View();
		}

		public virtual ActionResult Tokens() {
			return View();
		}

		public virtual ActionResult Constant() {
			return View(simpleLsystemProcessor);
		}

		public virtual ActionResult SymbolInterpretation() {
			return View(simpleLsystemProcessor);
		}

		public virtual ActionResult RewriteRule() {
			return View(simpleLsystemProcessor);
		}

		public virtual ActionResult Grammar() {
			return View();
		}

		public virtual ActionResult GrammarRegexps() {
			return View();
		}

	}
}

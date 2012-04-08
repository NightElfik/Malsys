using System.Web.Mvc;
using Malsys.Processing;
using Malsys.SemanticModel.Evaluated;
using Malsys.Web.Models.Lsystem;

namespace Malsys.Web.Areas.Help.Controllers {
	[OutputCache(CacheProfile="HelpCache")]
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

		public virtual ActionResult ConstantDefinition() {
			return View(simpleLsystemProcessor);
		}

		public virtual ActionResult FunctionDefinition() {
			return View(simpleLsystemProcessor);
		}

		public virtual ActionResult LsystemDefinition() {
			return View(simpleLsystemProcessor);
		}

		public virtual ActionResult ComponentPropertyAssignDefinition() {
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

		public virtual ActionResult ComponentConfigurationDefinition() {
			return View(simpleLsystemProcessor);
		}

	}
}

using System.Linq;
using System.Web.Mvc;
using Malsys.Compilers;
using Malsys.Processing;
using Malsys.Reflection;
using Malsys.Web.Areas.Help.Models;

namespace Malsys.Web.Areas.Help.Controllers {
	public partial class PredefinedController : Controller {

		//private readonly InputBlock stdLib;
		private readonly IComponentContainer componentContainer;
		private readonly IKnownConstantsProvider knownConstantsProvider;
		private readonly IKnownFunctionsProvider knownFunctionsProvider;
		private readonly XmlDocReader xmlDocReader;


		public PredefinedController(IComponentContainer componentContainer, IKnownConstantsProvider knownConstantsProvider,
				IKnownFunctionsProvider knownFunctionsProvider, XmlDocReader xmlDocReader) {

			this.componentContainer = componentContainer;
			this.knownConstantsProvider = knownConstantsProvider;
			this.knownFunctionsProvider = knownFunctionsProvider;
			this.xmlDocReader = xmlDocReader;
		}

		public virtual ActionResult Constants() {
			return View(knownConstantsProvider.GetAllConstants());
		}

		public virtual ActionResult Functions() {
			return View(knownFunctionsProvider.GetAllFunctions());
		}

		public virtual ActionResult Operators() {
			return View();
		}

		public virtual ActionResult Components() {
			var allCompGroupTypes = componentContainer.GetAllRegisteredComponents().GroupBy(x => x.Value);
			var allTypes = allCompGroupTypes.Select(x => x.Key).ToList();
			var components = allCompGroupTypes.Select(g => ComponentModel.FromType(g.Key, xmlDocReader, g.Select(y => y.Key), allTypes));
			return View(components);
		}

	}
}

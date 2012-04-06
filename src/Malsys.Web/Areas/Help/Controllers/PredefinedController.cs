using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Malsys.Compilers;
using Malsys.Evaluators;
using Malsys.Processing;
using Malsys.Reflection;
using Malsys.SemanticModel.Evaluated;
using Malsys.Web.Areas.Help.Models;
using Malsys.Web.Areas.Help.Models.Predefined;

namespace Malsys.Web.Areas.Help.Controllers {
	public partial class PredefinedController : Controller {

		private readonly InputBlockEvaled stdLib;
		private readonly IComponentTypeContainer componentContainer;
		private readonly IKnownConstantsProvider knownConstantsProvider;
		private readonly IExpressionEvaluatorContext expressionEvaluatorContext;
		private readonly XmlDocReader xmlDocReader;


		public PredefinedController(IComponentTypeContainer componentContainer, IKnownConstantsProvider knownConstantsProvider,
			IExpressionEvaluatorContext expressionEvaluatorContext, XmlDocReader xmlDocReader, InputBlockEvaled stdLib) {

			this.componentContainer = componentContainer;
			this.knownConstantsProvider = knownConstantsProvider;
			this.expressionEvaluatorContext = expressionEvaluatorContext;
			this.xmlDocReader = xmlDocReader;
			this.stdLib = stdLib;
		}

		public virtual ActionResult Constants() {
			return View();
		}

		public virtual ActionResult Functions() {

			var model = expressionEvaluatorContext.GetAllStoredFunctions()
				.Select(x => new Function() {
					FunctionInfo = x,
					Documentation = x.Metadata is FieldInfo ? xmlDocReader.GetXmlDocumentation(x.Metadata as FieldInfo) : "",
					Group = x.Metadata is FieldInfo ? xmlDocReader.GetXmlDocumentation(x.Metadata as FieldInfo, "docGroup") : "",
				});

			return View(model);
		}

		public virtual ActionResult Operators() {
			return View();
		}

		public virtual ActionResult Components() {
			var allCompGroupTypes = componentContainer.GetAllRegisteredComponentTypes().GroupBy(x => x.Value);
			var allTypes = allCompGroupTypes.Select(x => x.Key).ToList();
			var components = allCompGroupTypes.Select(g => ComponentModel.FromType(g.Key, xmlDocReader, g.Select(y => y.Key), allTypes));
			return View(components);
		}

	}
}

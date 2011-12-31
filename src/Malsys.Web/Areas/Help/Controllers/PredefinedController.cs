using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using Malsys.Compilers;
using Malsys.Compilers.Expressions;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Web.Areas.Help.Controllers {
	public partial class PredefinedController : Controller {

		//private readonly InputBlock stdLib;
		private readonly IKnownConstantsProvider knownConstantsProvider;
		private readonly IKnownFunctionsProvider knownFunctionsProvider;


		public PredefinedController(IKnownConstantsProvider knownConstantsProvider,
				IKnownFunctionsProvider knownFunctionsProvider) {

			this.knownConstantsProvider = knownConstantsProvider;
			this.knownFunctionsProvider = knownFunctionsProvider;
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

	}
}

using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Malsys.Compilers;
using Malsys.Evaluators;
using Malsys.Processing;
using Malsys.Reflection;
using Malsys.SemanticModel.Evaluated;
using Malsys.Web.Areas.Help.Models.Predefined;
using Malsys.Reflection.Components;
using System.Collections.Generic;
using Malsys.Web.Areas.Help.Models;
using System;

namespace Malsys.Web.Areas.Help.Controllers {
	public partial class PredefinedController : Controller {

		private readonly InputBlockEvaled stdLib;
		private readonly IComponentMetadataContainer componentContainer;
		private readonly IComponentMetadataResolver metadataResolver;
		private readonly IKnownConstantsProvider knownConstantsProvider;
		private readonly IExpressionEvaluatorContext expressionEvaluatorContext;
		private readonly XmlDocReader xmlDocReader;
		private readonly IComponentXmlDocLoader xmlDocLoader;


		public PredefinedController(IComponentMetadataContainer componentContainer, IComponentMetadataResolver metadataResolver, IKnownConstantsProvider knownConstantsProvider,
				IExpressionEvaluatorContext expressionEvaluatorContext, XmlDocReader xmlDocReader, InputBlockEvaled stdLib, IComponentXmlDocLoader xmlDocLoader) {

			this.componentContainer = componentContainer;
			this.metadataResolver = metadataResolver;
			this.knownConstantsProvider = knownConstantsProvider;
			this.expressionEvaluatorContext = expressionEvaluatorContext;
			this.xmlDocReader = xmlDocReader;
			this.stdLib = stdLib;
			this.xmlDocLoader = xmlDocLoader;
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

			var logger = new MessageLogger();
			var allRegistered = componentContainer.GetAllRegisteredComponentsMetadata(logger);
			if (logger.ErrorOccurred) {
				foreach (var msg in logger) {
					ModelState.AddModelError("", msg.GetFullMessage());
				}
			}

			var allComponentsGroupped = allRegistered.GroupBy(x => x.Value.ComponentType);
			var allTypes = allComponentsGroupped.Select(x => x.Key).ToList();

			var components = allComponentsGroupped.Select(compGroup => {
				var names = compGroup.Select(y => y.Key);
				var meta = compGroup.First().Value;
				var type = meta.ComponentType;
				var baseT = (type.BaseType != null ? type.GetInterfaces().Concat(new Type[] { type.BaseType }) : type.GetInterfaces())
					.Where(y => typeof(Malsys.Processing.Components.IComponent).IsAssignableFrom(y)).OrderBy(z => z.FullName);
				var derivedT = allTypes.Where(t => type != t && type.IsAssignableFrom(t)).OrderBy(t => t.FullName);

				return new ComponentModel() {
					AccessNames = names.ToArray(),
					Metadata = meta,
					BaseTypes = baseT.ToArray(),
					DerivedTypes = derivedT.ToArray()
				};
			}).ToList();

			foreach (var c in components) {
				if (!c.Metadata.IsDocumentationLoaded) {
					xmlDocLoader.LoadXmlDoc(c.Metadata);
				}
			}


			return View(components);
		}

		public virtual ActionResult Configurations() {
			return View();
		}

	}
}

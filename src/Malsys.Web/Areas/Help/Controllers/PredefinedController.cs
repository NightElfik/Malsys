using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Malsys.Compilers;
using Malsys.Evaluators;
using Malsys.Processing;
using Malsys.Reflection.Components;
using Malsys.SemanticModel.Evaluated;
using Malsys.Web.Areas.Help.Models;
using Malsys.Resources;
using Malsys.Web.Models.Lsystem;

namespace Malsys.Web.Areas.Help.Controllers {
	[OutputCache(CacheProfile = "HelpCache")]
	public partial class PredefinedController : Controller {

		private readonly InputBlockEvaled stdLib;
		private readonly IComponentMetadataContainer componentContainer;
		private readonly IComponentMetadataResolver metadataResolver;
		private readonly ICompilerConstantsProvider compilerConstantsProvider;
		private readonly IOperatorsProvider operatorsProvider;
		private readonly IExpressionEvaluatorContext expressionEvaluatorContext;

		private readonly SimpleLsystemProcessor simpleLsystemProcessor;


		public PredefinedController(IComponentMetadataContainer componentContainer, IComponentMetadataResolver metadataResolver, ICompilerConstantsProvider compilerConstantsProvider,
				 IOperatorsProvider operatorsProvider, IExpressionEvaluatorContext expressionEvaluatorContext, ProcessManager processManager, InputBlockEvaled stdLib) {

			this.componentContainer = componentContainer;
			this.metadataResolver = metadataResolver;
			this.compilerConstantsProvider = compilerConstantsProvider;
			this.operatorsProvider = operatorsProvider;
			this.expressionEvaluatorContext = expressionEvaluatorContext;
			this.stdLib = stdLib;

			simpleLsystemProcessor = new SimpleLsystemProcessor(processManager, stdLib);
		}

		public virtual ActionResult Constants() {
			return View(new PredefinedConstantsModel() {
				CompilerConstants = compilerConstantsProvider.GetAllConstants(),
				StdLibConstants = stdLib.ExpressionEvaluatorContext.GetAllStoredVariables()
			});
		}

		public virtual ActionResult Functions() {
			return View(expressionEvaluatorContext.GetAllStoredFunctions());
		}

		public virtual ActionResult Operators() {
			return View(new Tuple<IEnumerable<OperatorCore>, SimpleLsystemProcessor>(operatorsProvider.GetAllOperators(), simpleLsystemProcessor));
		}

		public virtual ActionResult Components() {

			var logger = new MessageLogger();
			var allRegistered = componentContainer.GetAllRegisteredComponents();
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


			return View(components);
		}

		public virtual ActionResult Configurations() {

			var componentModels = stdLib.ProcessConfigurations.Select(compKvp => {
				var config = compKvp.Value;

				var allComponentsMetadata = config.Components.Select(x => metadataResolver.ResolveComponentMetadata(x.TypeName))
					.Concat(config.Containers.Select(y => metadataResolver.ResolveComponentMetadata(y.DefaultTypeName))).ToList();

				var gettableProps = allComponentsMetadata.SelectMany(meta => meta.GettableProperties.Select(item =>
					new KeyValuePair<ComponentMetadata, ComponentGettablePropertyMetadata>(meta, item))).ToList();

				var settableProps = allComponentsMetadata.SelectMany(meta => meta.SettableProperties.Select(item =>
					new KeyValuePair<ComponentMetadata, ComponentSettablePropertyMetadata>(meta, item))).ToList();

				var settableSymbolProps = allComponentsMetadata.SelectMany(meta => meta.SettableSymbolsProperties.Select(item =>
					new KeyValuePair<ComponentMetadata, ComponentSettableSybolsPropertyMetadata>(meta, item))).ToList();

				var connectableProps = allComponentsMetadata.SelectMany(meta => meta.ConnectableProperties.Select(item =>
					new KeyValuePair<ComponentMetadata, ComponentConnectablePropertyMetadata>(meta, item))).ToList();

				var callableFuns = allComponentsMetadata.SelectMany(meta => meta.CallableFunctions.Select(item =>
					new KeyValuePair<ComponentMetadata, ComponentCallableFunctionMetadata>(meta, item))).ToList();

				var interpretMethods = allComponentsMetadata.SelectMany(meta => meta.InterpretationMethods.Select(item =>
					new KeyValuePair<ComponentMetadata, ComponentInterpretationMethodMetadata>(meta, item))).ToList();

				return new ConfigurationModel() {
					ProcessConfiguration = config,
					ComponentMetadataResolver = metadataResolver,
					GettableProperties = gettableProps,
					SettableProperties = settableProps,
					SettableSymbolProperties = settableSymbolProps,
					ConnectableProperties = connectableProps,
					CallableFunctions = callableFuns,
					InterpretationMethods = interpretMethods
				};
			});

			return View(componentModels.ToList());
		}

	}
}

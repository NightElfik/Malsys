using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Malsys.Processing;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Evaluated;
using Malsys.Web.Entities;

namespace Malsys.Web.Areas.Documentation.Controllers {
	[OutputCache(CacheProfile = "VaryByUserCache")]
	public partial class ApiController : Controller {

		private readonly IComponentMetadataResolver metadataResolver;
		private readonly InputBlockEvaled stdLib;
		private readonly IActionLogDb actionLogDb;


		public ApiController(IComponentMetadataResolver metadataResolver, InputBlockEvaled stdLib, IActionLogDb actionLogDb) {
			this.metadataResolver = metadataResolver;
			this.stdLib = stdLib;
			this.actionLogDb = actionLogDb;
		}


		public virtual ActionResult Index() {
			return View();
		}

		public virtual ActionResult ProcessConfigMembers(string processConfigName) {

			var processConfig = stdLib.ProcessConfigurations.Where(x => x.Key == processConfigName).Select(x => x.Value).FirstOrDefault();

			if (processConfig == null) {
				processConfig = new ProcessConfigurationStatement(null) {
					Name = "",
					Components = new List<ProcessComponent>(),
					Containers = new List<ProcessContainer>(),
					Connections = new List<ProcessComponentsConnection>(),
				};
			}

			var allComponentsMetadata = processConfig.Components.Select(x => metadataResolver.ResolveComponentMetadata(x.TypeName))
				.Concat(processConfig.Containers.Select(y => metadataResolver.ResolveComponentMetadata(y.DefaultTypeName)))
				.OrderBy(meta => meta.ComponentType.Name)
				.ToList();

			actionLogDb.Log("ConfigDocAjax", ActionLogSignificance.Low, processConfigName);

			return Json(allComponentsMetadata.Select(meta => new {
				ComponentType = meta.ComponentType.Name,
				SettableProperties = meta.SettableProperties.Select(x => new {
					Names = x.Names,
					ValueType = x.ExpressionValueType.ToTypeString(),
					DefaultValue = x.DefaultValueDoc,
					ExpectedValue = x.ExpectedValueDoc,
					TypicalValue = x.TypicalValueDoc,
					Doc = x.SummaryDoc
				}),
				GettableProperties = meta.GettableProperties.Select(x => new {
					Names = x.Names,
					ValueType = x.ExpressionValueType.ToTypeString(),
					Doc = x.SummaryDoc
				})
			}));
		}


	}
}

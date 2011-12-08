using System;
using System.Linq;
using System.Web.Mvc;
using Malsys.Processing;
using Malsys.Web.Models;
using System.Reflection;
using Malsys.Processing.Components;

namespace Malsys.Web.Controllers {
	public class ProcessLsystemController : Controller {

		private static ComponentResolver componentResolver;


		static ProcessLsystemController() {
			var resolver = new ComponentResolver();

			var componentsTypes = Assembly.GetAssembly(typeof(ComponentResolver)).GetTypes().Where(t => t.IsClass && (typeof(IComponent)).IsAssignableFrom(t));
			componentResolver = new ComponentResolver();
			foreach (var type in componentsTypes) {
				componentResolver.RegisterComponentNameAndFullName(type, true);
			}
		}

		public ActionResult Index(string lsystem = null) {

			if (lsystem != null) {
				return IndexPost(lsystem);
			}

			return View(new ProcessLsystemResultModel());
		}

		[HttpPost]
		[ActionName("Index")]
		public ActionResult IndexPost(string sourceCode) {

			var manager = new ProcessManager() { Timeout = new TimeSpan(0, 0, 5) };
			var fileMgr = new FilesManager(Server.MapPath(Url.Content("~/WorkDir")));
			var msgs = new MessageLogger();

			manager.RenderLsystems(sourceCode, fileMgr, msgs, componentResolver);

			var resultModel = new ProcessLsystemResultModel();
			resultModel.SourceCode = sourceCode;
			resultModel.Messages = msgs;
			resultModel.OutputFiles = fileMgr.GetOutputFilePaths();

			return View(resultModel);
		}
	}
}

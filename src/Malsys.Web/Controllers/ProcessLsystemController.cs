using System;
using System.Linq;
using System.Web.Mvc;
using Malsys.Processing;
using Malsys.Web.Models;
using System.Reflection;
using Malsys.Processing.Components;
using System.IO;
using Malsys.Web.Entities;

namespace Malsys.Web.Controllers {
	public class ProcessLsystemController : Controller {

		private const string workDir = "~/WorkDir";
		private static readonly ComponentResolver componentResolver;


		static ProcessLsystemController() {
			var resolver = new ComponentResolver();

			var componentsTypes = Assembly.GetAssembly(typeof(ComponentResolver)).GetTypes()
				.Where(t => (t.IsClass || t.IsInterface) && (typeof(IComponent)).IsAssignableFrom(t));
			componentResolver = new ComponentResolver();
			foreach (var type in componentsTypes) {
				componentResolver.RegisterComponentNameAndFullName(type, true);
			}
		}


		private readonly IInputProcessesRepository inputProcRepo;


		public ProcessLsystemController(IInputProcessesRepository inputProcessesRepository) {
			inputProcRepo = inputProcessesRepository;
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
			var fileMgr = new FilesManager(Server.MapPath(Url.Content(workDir)));
			var logger = new MessageLogger();

			var evaledInput = manager.CompileAndEvaluateInput(sourceCode, logger);

			if (!logger.ErrorOcured) {
				manager.ProcessInput(evaledInput, fileMgr, logger, componentResolver);

				/*if (!logger.ErrorOcured) {
					inputProcRepo.AddInput(evaledInput)
				}*/
			}


			var resultModel = new ProcessLsystemResultModel();
			resultModel.SourceCode = sourceCode;
			resultModel.Messages = logger;
			resultModel.OutputFiles = fileMgr.GetOutputFilePaths();

			return View(resultModel);
		}

		public ActionResult DownloadResult(string fileName, string mime = "application/octet-stream") {

			string realPath = Server.MapPath(Url.Content(workDir));
			string filePath = Path.GetFullPath(realPath + "\\" + fileName);
			if (!filePath.StartsWith(realPath) || !System.IO.File.Exists(filePath)) {
				return new HttpNotFoundResult("File `" + fileName + "` not found");
			}

			return File(new FileStream(filePath, FileMode.Open, FileAccess.Read), mime);
		}

	}
}

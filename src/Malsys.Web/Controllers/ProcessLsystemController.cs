using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Malsys.Processing;
using Malsys.Processing.Components;
using Malsys.Web.Infrastructure;
using Malsys.Web.Models;

namespace Malsys.Web.Controllers {
	public partial class ProcessLsystemController : Controller {

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


		private readonly string workDir;
		private readonly int maxWorkDirFiles;

		private readonly IInputProcessesRepository inputProcRepo;
		private readonly IDateTimeProvider dateTime;


		public ProcessLsystemController(IInputProcessesRepository inputProcessesRepository, IAppSettingsProvider appSettingsProvider,
				IDateTimeProvider dateTimeProvider) {

			inputProcRepo = inputProcessesRepository;
			dateTime = dateTimeProvider;

			workDir = appSettingsProvider["WorkDir"];
			maxWorkDirFiles = int.Parse(appSettingsProvider["WorkDir_MaxFilesCount"]);
		}


		public virtual ActionResult Index(string lsystem = null, bool spacesToTabs = true) {

			if (lsystem != null) {
				return IndexPost(spacesToTabs ? lsystem.Replace("    ", "\t") : lsystem);
			}

			return View(new ProcessLsystemResultModel());
		}

		[HttpPost]
		[ActionName("Index")]
		public virtual ActionResult IndexPost(string sourceCode, int? referenceId = null) {

			string workDirFullPath = Server.MapPath(Url.Content(workDir));
			var manager = new ProcessManager() { Timeout = new TimeSpan(0, 0, 5) };
			var fileMgr = new FilesManager(workDirFullPath);
			var logger = new MessageLogger();

			var evaledInput = manager.CompileAndEvaluateInput(sourceCode, logger);

			inputProcRepo.CleanProcessOutputs(workDirFullPath, maxWorkDirFiles);

			if (!logger.ErrorOcured) {
				manager.ProcessLsystems(evaledInput, fileMgr, logger, componentResolver);

				if (!logger.ErrorOcured) {
					var ip = inputProcRepo.AddInput(evaledInput, referenceId, fileMgr.GetOutputFilePaths(), User.Identity.Name);
					referenceId = ip.InputProcessId;
				}
			}

			var resultModel = new ProcessLsystemResultModel() {
				SourceCode = sourceCode,
				ReferenceId = referenceId,
				Logger = logger,
				OutputFiles = fileMgr.GetOutputFilePaths()
			};

			return View(resultModel);
		}

		public virtual ActionResult DownloadResult(string fileName, bool download = true) {

			var procOutput = inputProcRepo.InputDb.ProcessOutputs.Where(po => po.FileName == fileName).SingleOrDefault();

			if (procOutput == null) {
				return new HttpNotFoundResult("File `" + fileName + "` not found.");
			}

			procOutput.LastOpenDate = dateTime.Now;
			inputProcRepo.InputDb.SaveChanges();

			string realPath = Server.MapPath(Url.Content(workDir));
			string filePath = Path.Combine(realPath, fileName);


			if (download) {
				return File(filePath, "application/octet-stream", fileName);
			}
			else {
				string mime;
				switch (Path.GetExtension(filePath)) {
					case ".txt": mime = "text/plain"; break;
					case ".svg":
						mime = "image/svg+xml";
						break;
					case ".svgz":
						Response.AppendHeader("Content-Encoding", "gzip");
						mime = "image/svg+xml";
						break;
					default: mime = "application/octet-stream"; break;
				}
				return File(filePath, mime);
			}
		}

	}
}

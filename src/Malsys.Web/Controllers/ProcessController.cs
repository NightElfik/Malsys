using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Malsys.Processing;
using Malsys.Processing.Output;
using Malsys.SemanticModel.Evaluated;
using Malsys.Web.Infrastructure;
using Malsys.Web.Models;
using Malsys.Web.Models.Lsystem;

namespace Malsys.Web.Controllers {
	public partial class ProcessController : Controller {


		private readonly string workDir;
		private readonly int maxWorkDirFiles;
		private readonly int workDirCleanAmount;
		private readonly int autoPackTreshold;

		private readonly IMalsysInputRepository malsysInputRepository;
		private readonly IUsersRepository usersRepository;
		private readonly LsystemProcessor lsystemProcessor;
		private readonly IAppSettingsProvider appSettingsProvider;


		public ProcessController(IMalsysInputRepository malsysInputRepository, IAppSettingsProvider appSettingsProvider,
				IUsersRepository usersRepository, LsystemProcessor lsystemProcessor) {

			this.malsysInputRepository = malsysInputRepository;
			this.usersRepository = usersRepository;
			this.lsystemProcessor = lsystemProcessor;
			this.appSettingsProvider = appSettingsProvider;

			workDir = appSettingsProvider[AppSettingsKeys.WorkDir];
			maxWorkDirFiles = int.Parse(appSettingsProvider[AppSettingsKeys.MaxFilesInWorkDir]);
			workDirCleanAmount = int.Parse(appSettingsProvider[AppSettingsKeys.WorkDirCleanAmount]);
			autoPackTreshold = int.Parse(appSettingsProvider[AppSettingsKeys.AutoPackTreshold]);

		}


		public virtual ActionResult Index(string lsystem = null) {

			if (lsystem != null) {
				return IndexPost(lsystem.Replace("    ", "\t"));
			}

			return View(new ProcessLsystemResultModel());
		}

		[HttpPost]
		[ActionName("Index")]
		public virtual ActionResult IndexPost(string sourceCode, int? referenceId = null, string compile = null, string save = null) {

			usersRepository.LogUserActivity(User.Identity.Name);

			TimeSpan timeout;
			if (User.Identity.IsAuthenticated) {
				if (User.IsInRole(UserRoles.TrustedUser)) {
					timeout = new TimeSpan(0, 0, int.Parse(appSettingsProvider[AppSettingsKeys.TrustedUserProcessTime]));
				}
				else {
					timeout = new TimeSpan(0, 0, int.Parse(appSettingsProvider[AppSettingsKeys.RegisteredUserProcessTime]));
				}
			}
			else {
				timeout = new TimeSpan(0, 0, int.Parse(appSettingsProvider[AppSettingsKeys.UnregisteredUserProcessTime]));
			}
#if DEBUG
			timeout = TimeSpan.MaxValue;  // for debugging purposes
#endif
			string workDirFullPath = Server.MapPath(Url.Content(workDir));
			malsysInputRepository.CleanProcessOutputs(workDirFullPath, maxWorkDirFiles, workDirCleanAmount);

			var fileMgr = new FileOutputProvider(workDirFullPath);
			var logger = new MessageLogger();

			var resultModel = new ProcessLsystemResultModel() {
				SourceCode = sourceCode,
				ReferenceId = referenceId,
				Logger = logger
			};


			bool compileOnly = compile != null;

			InputBlockEvaled evaledInput;

			var sw = new Stopwatch();
			sw.Start();
			bool result = lsystemProcessor.TryProcess(sourceCode, timeout, fileMgr, logger, out evaledInput, true, compileOnly, true);
			sw.Stop();

			resultModel.ProcessDuration = sw.Elapsed;

			if (compileOnly || !result) {
				return View(Views.Index, resultModel);
			}

			if (evaledInput.ProcessStatements.Count == 0) {
				resultModel.NoProcessStatement = true;
				return View(Views.Index, resultModel);
			}

			List<OutputFile> outputs;

			if (fileMgr.OutputFilesCount > autoPackTreshold) {
				outputs = fileMgr.GetOutputFilesAsZipArchive().ToList();
			}
			else {
				outputs = fileMgr.GetOutputFiles().ToList();
			}

			if (outputs.Count > 0) {
				var ip = malsysInputRepository.AddInputProcess(evaledInput, referenceId, outputs, User.Identity.Name, sw.Elapsed);
				resultModel.ReferenceId = ip.InputProcessId;
				resultModel.OutputFiles = outputs;
			}

			if (save != null) {
				if (User.Identity.IsAuthenticated) {
					var savedinput = malsysInputRepository.SaveInput(sourceCode, resultModel.ReferenceId, outputs, User.Identity.Name, sw.Elapsed);
					resultModel.SavedInputUrlId = savedinput.UrlId;
				}
				else {
					ModelState.AddModelError("", "Only registered users can save Malsys inputs.");
				}
			}

			return View(Views.Index, resultModel);
		}

		public virtual ActionResult Load(string id) {

			var savedInput = malsysInputRepository.InputDb.SavedInputs.Where(x => x.UrlId == id).SingleOrDefault();
			if (savedInput != null) {
				savedInput.Views++;
				malsysInputRepository.InputDb.SaveChanges();
				return IndexPost(savedInput.SourceCode);
			}

			return View(new ProcessLsystemResultModel() { SavedInputUrlId = id });
		}

	}
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Malsys.Processing.Output;
using Malsys.SemanticModel.Evaluated;
using Malsys.Web.Entities;
using Malsys.Web.Infrastructure;
using Malsys.Web.Models;
using Malsys.Web.Models.Lsystem;

namespace Malsys.Web.Controllers {
	public partial class ApiController : Controller {

		private readonly string workDir;
		private readonly int maxWorkDirFiles;
		private readonly int workDirCleanAmount;
		private readonly int autoPackTreshold;

		private readonly IMalsysInputRepository malsysInputRepository;
		private readonly IUsersRepository usersRepository;
		private readonly LsystemProcessor lsystemProcessor;
		private readonly IAppSettingsProvider appSettingsProvider;

		public ApiController(IMalsysInputRepository malsysInputRepository, IAppSettingsProvider appSettingsProvider,
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


		public virtual ActionResult Process(string sourceCode) {

			if (sourceCode == null) {
				sourceCode = "";
			}

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

			string workDirFullPath = Server.MapPath(Url.Content(workDir));
			malsysInputRepository.CleanProcessOutputs(workDirFullPath, maxWorkDirFiles, workDirCleanAmount);

			var fileMgr = new FileOutputProvider(workDirFullPath);
			var logger = new MessageLogger();

			InputBlockEvaled evaledInput;

			var sw = new Stopwatch();
			sw.Start();
			bool success = lsystemProcessor.TryProcess(sourceCode, timeout, fileMgr, logger, out evaledInput, true, false);
			sw.Stop();

			if (!success) {
				usersRepository.UsersDb.Log("ApiProcess-FAIL", ActionLogSignificance.Low);
				return Json(new {
					error = true,
					messages = logger.Select(x => x.GetFullMessage()).ToList()
				}, JsonRequestBehavior.AllowGet);
			}

			List<OutputFile> outputs;

			if (fileMgr.OutputsCount > autoPackTreshold) {
				outputs = fileMgr.GetOutputFilesAsZipArchive().ToList();
			}
			else {
				outputs = fileMgr.GetOutputFiles().ToList();
			}

			if (outputs.Count > 0) {
				var ip = malsysInputRepository.AddInputProcess(evaledInput, null, outputs, User.Identity.Name, sw.Elapsed);
			}

			usersRepository.UsersDb.Log("ApiProcess", ActionLogSignificance.Low);

			return Json(new {
				error = false,
				messages = logger.Select(x => x.GetFullMessage()).ToList(),
				outputs = outputs.Select(x => new {
					mime = x.MimeType,
					url = Url.ActionAbsolute(MVC.ProcessOutput.Show(Path.GetFileName(x.FilePath))),
					metadata = x.Metadata
				})
			}, JsonRequestBehavior.AllowGet);
		}

	}
}

// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Malsys.IO;
using Malsys.Processing.Output;
using Malsys.SemanticModel.Evaluated;
using Malsys.SourceCode.Printers;
using Malsys.Web.Entities;
using Malsys.Web.Infrastructure;
using Malsys.Web.Models;
using Malsys.Web.Models.Lsystem;
using Microsoft.FSharp.Collections;

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

#if DEBUG
			autoPackTreshold = 50;  // For debugging purposes.
#endif

		}


		public virtual ActionResult Index(string lsystem = null) {

			if (lsystem != null) {
				return IndexPost(lsystem.Replace("    ", "\t"));
			}

			return View(new ProcessLsystemResultModel() { IsEmpty = true });
		}

		[HttpPost, ValidateInput(false)]
		[ActionName("Index")]
		public virtual ActionResult IndexPost(string sourceCode, int? referenceId = null, string compile = null, string save = null) {

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
#if DEBUG
			timeout = TimeSpan.MaxValue;  // For debugging purposes.
#endif
			string workDirFullPath = Server.MapPath(Url.Content(workDir));
			malsysInputRepository.CleanProcessOutputs(workDirFullPath, maxWorkDirFiles, workDirCleanAmount);

			var fileMgr = new FileOutputProvider(workDirFullPath);
			var logger = new MessageLogger();

			var resultModel = new ProcessLsystemResultModel() {
				SourceCode = sourceCode,
				ReferenceId = referenceId,
				Logger = logger,
				MaxProcessDuration = timeout
			};


			bool compileOnly = compile != null;

			InputBlockEvaled evaledInput;

			var sw = new Stopwatch();
			sw.Start();
			bool result = lsystemProcessor.TryProcess(sourceCode, timeout, fileMgr, logger, out evaledInput, true, compileOnly);
			sw.Stop();

			resultModel.ProcessDuration = sw.Elapsed;

			if (compileOnly) {
				if (evaledInput != null) {
					var writer = new IndentStringWriter();
					new CanonicPrinter(writer).Print(evaledInput);
					resultModel.CompiledSourceCode = writer.GetResult();
				}
				else {
					resultModel.CompiledSourceCode = "";
				}
			}


			if (evaledInput != null) {
				resultModel.UsedProcessConfigurationsNames = evaledInput.ProcessStatements.Select(x => x.ProcessConfigName).ToArray();
			}
			else {
				resultModel.UsedProcessConfigurationsNames = new string[0];
			}


			if (compileOnly || !result) {
				return View(Views.Index, resultModel);
			}

			if (fileMgr.OutputsCount == 0 && evaledInput.ProcessStatements.Count == 0) {
				resultModel.NoProcessStatement = true;
				return View(Views.Index, resultModel);
			}

			List<OutputFile> outputs;

			if (fileMgr.OutputsCount > autoPackTreshold) {
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
			else {
				resultModel.OutputFiles = new List<OutputFile>();
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

			malsysInputRepository.InputDb.Log("ProcessLsystem", ActionLogSignificance.Low, null, malsysInputRepository.UsersDb.TryGetUserByName(User.Identity.Name));

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

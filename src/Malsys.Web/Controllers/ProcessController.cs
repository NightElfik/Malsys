﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Malsys.Processing;
using Malsys.SemanticModel.Evaluated;
using Malsys.Web.Infrastructure;
using Malsys.Web.Models;
using Malsys.Processing.Output;

namespace Malsys.Web.Controllers {
	public partial class ProcessController : Controller {


		private readonly string workDir;
		private readonly int maxWorkDirFiles;

		private readonly IMalsysInputRepository malsysInputRepository;
		private readonly IUsersRepository usersRepository;
		private readonly ProcessManager processManager;
		private readonly InputBlock stdLib;


		public ProcessController(IMalsysInputRepository malsysInputRepository, IAppSettingsProvider appSettingsProvider,
				IUsersRepository usersRepository, ProcessManager processManager, InputBlock stdLib) {

			this.malsysInputRepository = malsysInputRepository;
			this.usersRepository = usersRepository;
			this.processManager = processManager;
			this.stdLib = stdLib;

			workDir = appSettingsProvider["WorkDir"];
			maxWorkDirFiles = int.Parse(appSettingsProvider["WorkDir_MaxFilesCount"]);
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
					timeout = new TimeSpan(0, 0, 10);
				}
				else {
					timeout = new TimeSpan(0, 0, 6);
				}
			}
			else {
				timeout = new TimeSpan(0, 0, 4);
			}

			string workDirFullPath = Server.MapPath(Url.Content(workDir));
			malsysInputRepository.CleanProcessOutputs(workDirFullPath, maxWorkDirFiles);

			var fileMgr = new FileOutputProvider(workDirFullPath);
			var logger = new MessageLogger();

			var resultModel = new ProcessLsystemResultModel() {
				SourceCode = sourceCode,
				ReferenceId = referenceId,
				Logger = logger
			};

			var sw = new Stopwatch();
			sw.Start();

			var evaledInput = processManager.CompileAndEvaluateInput(sourceCode, logger);

			if (logger.ErrorOcured || compile != null) {
				return View(Views.Index, resultModel);
			}


			var inAndStdlib = stdLib.JoinWith(evaledInput);

			processManager.ProcessLsystems(inAndStdlib, fileMgr, logger, timeout);

			if (logger.ErrorOcured) {
				return View(Views.Index, resultModel);
			}

			sw.Stop();

			var outputs = fileMgr.GetOutputFiles();

			var ip = malsysInputRepository.AddInputProcess(evaledInput, referenceId, outputs, User.Identity.Name, sw.Elapsed);
			resultModel.ReferenceId = ip.InputProcessId;
			resultModel.OutputFiles = outputs;

			if (save != null) {
				if (User.Identity.IsAuthenticated) {
					var savedinput = malsysInputRepository.SaveInput(sourceCode, resultModel.ReferenceId, outputs, User.Identity.Name, sw.Elapsed);
					resultModel.SavedInputId = savedinput.RandomId;
				}
				else {
					ModelState.AddModelError("", "Only registered users can save Malsys inputs.");
				}
			}

			return View(Views.Index, resultModel);
		}

		public virtual ActionResult Load(string id) {

			var savedInput = malsysInputRepository.InputDb.SavedInputs.Where(x => x.RandomId == id).SingleOrDefault();
			if (savedInput != null) {
				savedInput.Views++;
				malsysInputRepository.InputDb.SaveChanges();
				return IndexPost(savedInput.Source);
			}

			return View(new ProcessLsystemResultModel() { SavedInputId = id });
		}

		public virtual ActionResult MakeAdminAdmin() {

			var role = usersRepository.Roles.Where(x => x.NameLowercase == UserRoles.Administrator.ToLower()).SingleOrDefault();
			if (role == null) {
				role = usersRepository.CreateRole(new NewRoleModel() { RoleName = UserRoles.Administrator });
			}
			var user = usersRepository.Users.Where(x => x.NameLowercase == UserRoles.Administrator.ToLower()).SingleOrDefault();

			if (user != null && role != null) {
				usersRepository.AddUserToRole(user.UserId, role.RoleId);
			}

			return RedirectToAction(MVC.Home.Index());
		}


	}
}
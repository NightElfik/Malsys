﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using Malsys.Compilers;
using Malsys.Evaluators;
using Malsys.Processing;
using Malsys.Reflection;
using Malsys.Resources;
using Malsys.Web.Areas.Documentation.Models;
using Malsys.Web.Entities;
using Malsys.Web.Infrastructure;
using Malsys.Web.Models;
using Malsys.Web.Models.Lsystem;
using Malsys.Web.Models.Repositories;
using Malsys.Web.Security;

namespace Malsys.Web {
	public class MvcApplication : HttpApplication {

		protected void Application_Start() {
			IAppSettingsProvider appSettings = new AppSettingsProvider();						
			loadPrivateData(appSettings[AppSettingsKeys.PrivateDir]);

			var resolver = buildDependencyResolver(appSettings);
			DependencyResolver.SetResolver(resolver);

			AreaRegistration.RegisterAllAreas();


			initializeDb(resolver.GetService<IUsersRepository>());
			initializeDiscussion(resolver.GetService<IDiscussionRepository>());
			checkFileSystem(appSettings);


			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
		}

		private void loadPrivateData(string virtPath) {
			string absPath = Server.MapPath(virtPath);
			PrivateData.LoadFormDir(absPath);
			Debug.Assert(!string.IsNullOrWhiteSpace(PrivateData.GoogleAnalyticsKey));
			Debug.Assert(!string.IsNullOrWhiteSpace(PrivateData.ReCaptchaPrivate));
			Debug.Assert(!string.IsNullOrWhiteSpace(PrivateData.ReCaptchaPublic));
		}

		public override string GetVaryByCustomString(HttpContext context, string custom) {
			if (custom == "user") {
				// cache vary by individual users
				if (context.User.Identity.IsAuthenticated) {
					return context.User.Identity.Name.ToLower();
				}
				else {
					return "";
				}
			}

			return base.GetVaryByCustomString(context, custom);
		}

		private IDependencyResolver buildDependencyResolver(IAppSettingsProvider appSettings) {
			var builder = new ContainerBuilder();

			// registers all MVC controllers in this assembly
			builder.RegisterControllers(typeof(MvcApplication).Assembly);

			builder.RegisterType<StandardDateTimeProvider>().As<IDateTimeProvider>().SingleInstance();
			builder.RegisterType<Sha512PasswordHasher>().As<IPasswordHasher>().SingleInstance();

			builder.RegisterType<MalsysDb>()
				.As<IUsersDb>()
				.As<IInputDb>()
				.As<IFeedbackDb>()
				.As<IDiscusDb>()
				.As<IActionLogDb>()
				.InstancePerHttpRequest();

			builder.RegisterType<UsersRepository>().As<IUsersRepository>().InstancePerHttpRequest();
			builder.RegisterType<MalsysInputRepository>().As<IMalsysInputRepository>().InstancePerHttpRequest();
			builder.RegisterType<DiscussionRepository>().As<IDiscussionRepository>().InstancePerHttpRequest();

			builder.RegisterType<UserAuthenticator>().As<IUserAuthenticator>().InstancePerHttpRequest();
			builder.RegisterType<FormsAuthenticationProvider>().As<IAuthenticationProvider>().SingleInstance();

			builder.Register(x => appSettings)
				.As<IAppSettingsProvider>()
				.SingleInstance();

			string basePath = Server.MapPath("~/bin");  // path where DLL and theirs XML doc files are
			var xmlDocReader = new XmlDocReader(basePath);
			builder.Register(x => xmlDocReader)
				.As<IXmlDocReader>()
				.SingleInstance();


			registerMalsysStuff(builder, xmlDocReader);

			var articles = typeof(HelpArticle)
				.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
				.Where(fi => fi.FieldType == typeof(HelpArticle))
				.Select(fi => (HelpArticle)fi.GetValue(null))
				.Where(p => !string.IsNullOrEmpty(p.ViewName))
				.OrderByDescending(p => p.Date)
				.ToList();
			builder.Register(x => articles)
				.As<IEnumerable<HelpArticle>>()
				.SingleInstance();

			var devDiaryEntries = typeof(DevDiaryEntry)
				.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
				.Where(fi => fi.FieldType == typeof(DevDiaryEntry))
				.Select(fi => (DevDiaryEntry)fi.GetValue(null))
				.Where(p => !string.IsNullOrEmpty(p.ViewName))
				.OrderByDescending(p => p.Date)
				.ToList();
			builder.Register(x => devDiaryEntries)
				.As<IEnumerable<DevDiaryEntry>>()
				.SingleInstance();

			var captcha = new ReCaptcha(PrivateData.ReCaptchaPublic, PrivateData.ReCaptchaPrivate);
			builder.Register(x => captcha)
				.As<ICaptcha>()
				.SingleInstance();

			return new AutofacDependencyResolver(builder.Build());

		}

		private void registerMalsysStuff(ContainerBuilder builder, IXmlDocReader xmlDocReader) {

			var knownStuffProvider = new KnownConstOpProvider();
			IExpressionEvaluatorContext eec = new ExpressionEvaluatorContext();
			var componentResolver = new ComponentResolver();

			var logger = new MessageLogger();

			var loader = new MalsysLoader();
			loader.LoadMalsysStuffFromAssembly(Assembly.GetAssembly(typeof(MalsysLoader)),
				knownStuffProvider, knownStuffProvider, ref eec, componentResolver, logger, xmlDocReader);

			var loadedPlugins = new List<Assembly>();
			Assembly plugin;
			if (tryLoadPlugin("~/bin/ExamplePlugin.dll", out plugin, loadedPlugins)) {
				loader.LoadMalsysStuffFromAssembly(plugin,
					knownStuffProvider, knownStuffProvider, ref eec, componentResolver, logger, xmlDocReader);
			}
			if (tryLoadPlugin("~/bin/Malsys.BitmapRenderers.dll", out plugin, loadedPlugins)) {
				loader.LoadMalsysStuffFromAssembly(plugin,
					knownStuffProvider, knownStuffProvider, ref eec, componentResolver, logger, xmlDocReader);
			}

			if (logger.ErrorOccurred) {
				throw new Exception("Failed to load Malsys stuff" + logger.AllMessagesToFullString());
			}

			builder.Register(x => new LoadedPlugins(loadedPlugins));

			builder.Register(x => knownStuffProvider)
				.As<ICompilerConstantsProvider>()
				.As<IOperatorsProvider>()
				.SingleInstance();

			builder.Register(x => eec)
				.As<IExpressionEvaluatorContext>()
				.SingleInstance();

			builder.Register(x => componentResolver)
				.As<IComponentMetadataContainer>()
				.As<IComponentMetadataResolver>()
				.SingleInstance();


			builder.RegisterType<CompilersContainer>()
				.As<ICompilersContainer>()
				.InstancePerHttpRequest();
			builder.RegisterType<EvaluatorsContainer>()
				.As<IEvaluatorsContainer>()
				.InstancePerHttpRequest();

			builder.RegisterType<ProcessManager>().InstancePerHttpRequest();
			builder.RegisterType<LsystemProcessor>().InstancePerHttpRequest();


			string stdlib;
			using (var stream = new ResourcesReader().GetResourceStream(ResourcesHelper.StdLibResourceName)) {
				using (TextReader reader = new StreamReader(stream)) {
					stdlib = reader.ReadToEnd();
				}
			}

			stdlib += File.ReadAllText(Server.MapPath("~/App_Data/StdLibExtension.malsys"));

			builder.Register(x => new MalsysStdLibSource(stdlib)).SingleInstance();

			var inCompiled = new CompilersContainer(knownStuffProvider, knownStuffProvider).CompileInput(stdlib, ResourcesHelper.StdLibResourceName, logger);
			var stdLib = new EvaluatorsContainer(eec).EvaluateInput(inCompiled, logger);
			if (logger.ErrorOccurred) {
				throw new Exception("Failed to build std lib.");
			}

			builder.Register(x => stdLib).SingleInstance();

		}



		private bool tryLoadPlugin(string path, out Assembly plugin, List<Assembly> loadedPlugins, bool pathIsVirtual = true) {
			if (pathIsVirtual) {
				path = Server.MapPath(path);
			}
			try {
				if (File.Exists(path)) {
					plugin = Assembly.LoadFile(path);
					loadedPlugins.Add(plugin);
					return true;
				}
				plugin = null;
				return false;
			}
			catch (Exception ex) {
				Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
				plugin = null;
				return false;
			}
		}


		private void initializeDiscussion(IDiscussionRepository discusRepo) {

			foreach (DiscussionCategory cat in Enum.GetValues(typeof(DiscussionCategory))) {
				string name = cat.ToName();
				if (discusRepo.EnsureCategory(name) == null) {
					throw new Exception("Failed to ensure discussion category `{0}`".Fmt(name));
				}
			}

		}



		/// <summary>
		/// Checks whether DB contains all well-known user roles and at least one user.
		/// Missing things are added.
		/// </summary>
		/// <remarks>
		/// Standard user groups from class <c>UserRoles</c> are checked and eventually created.
		/// If there are no users in the database, "Administrator" user with password "malsys" is created.
		/// </remarks>
		private void initializeDb(IUsersRepository usersRepo) {

			foreach (var fi in typeof(UserRoles).GetFields(BindingFlags.Public | BindingFlags.Static)) {

				if (fi.FieldType != typeof(string) || !fi.IsLiteral) {
					continue;
				}

				string roleName = ((string)fi.GetValue(null)).Trim().ToLower();

				if (usersRepo.UsersDb.Roles.Where(x => x.NameLowercase == roleName).Count() == 0) {
					if (usersRepo.CreateRole(new NewRoleModel() { RoleName = roleName }) == null) {
						throw new Exception("Failed to create role `{0}`".Fmt(roleName));
					}
				}

			}

			if (usersRepo.UsersDb.Users.Count() == 0) {
				// DB has no users -- lets create admin
				var adminUserResult = usersRepo.CreateUser(new NewUserModel() {
					UserName = "Administrator",
					Email = "temp@email.com",
					Password = "malsys",
					ConfirmPassword = "malsys"
				});

				if (!adminUserResult) {
					throw new Exception("Failed to create administrator user.");
				}

				var adminRole = usersRepo.UsersDb.Roles.Where(x => x.NameLowercase == UserRoles.Administrator).Single();

				usersRepo.AddUserToRole(adminUserResult.Data.UserId, adminRole.RoleId);

			}

		}

		/// <summary>
		/// Checks if all necessary directories exists and are writable.
		/// </summary>
		private void checkFileSystem(IAppSettingsProvider appSettingsProvider) {

			ensureDirExistsAndIsWritable(appSettingsProvider[AppSettingsKeys.WorkDir]);

			ensureDirExistsAndIsWritable(appSettingsProvider[AppSettingsKeys.GalleryWorkDir]);


			// this is not elegant solution but better than nothing (I can not find better solution)
			var section = WebConfigurationManager.OpenWebConfiguration("/").GetSection("elmah/errorLog") as DefaultSection;
			string rawXml = section.SectionInformation.GetRawXml();

			const string start = "logPath=\"";
			const string end = "\"";
			int startI = rawXml.IndexOf(start) + start.Length;
			int endI = rawXml.IndexOf(end, startI);
			string errReportDirPath = rawXml.SubstringPos(startI, endI);

			ensureDirExistsAndIsWritable(errReportDirPath);

		}

		/// <summary>
		/// Ensures that directory at given path exists and is writable.
		/// Directory is created if don't exist and exception is thrown if is not writable.
		/// </summary>
		private void ensureDirExistsAndIsWritable(string path, bool pathIsVirtual = true) {

			if (pathIsVirtual) {
				path = Server.MapPath(path);
			}

			if (!Directory.Exists(path)) {
				Directory.CreateDirectory(path);
			}

			string filePath = Path.Combine(path, "IsThisDirectoryWritableTest." + DateTime.Now.Ticks.ToString());  // unique file name? I hope so :-D
			try {
				File.Create(filePath).Dispose();
				File.Delete(filePath);
			}
			catch (Exception ex) {
				throw new Exception("Directory `{0}` is not writable!".Fmt(path), ex);
			}

		}

	}
}

﻿using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using Malsys.Compilers;
using Malsys.Evaluators;
using Malsys.Processing;
using Malsys.Processing.Components;
using Malsys.Reflection;
using Malsys.Reflection.Components;
using Malsys.Resources;
using Malsys.SemanticModel.Evaluated;
using Malsys.Web.Entities;
using Malsys.Web.Infrastructure;
using Malsys.Web.Models;
using Malsys.Web.Models.Lsystem;
using Malsys.Web.Models.Repositories;
using Malsys.Web.Security;

namespace Malsys.Web {
	public class MvcApplication : HttpApplication {

		protected void Application_Start() {

			var resolver = buildDependencyResolver();
			DependencyResolver.SetResolver(resolver);

			AreaRegistration.RegisterAllAreas();

			initializeDb(resolver);
			checkFileSystem(resolver);

			registerGlobalFilters(GlobalFilters.Filters);
			registerRoutes(RouteTable.Routes);

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

		private void registerGlobalFilters(GlobalFilterCollection filters) {
			filters.Add(new InvariantCultureActionFilter());
		}

		private void registerRoutes(RouteCollection routes) {

			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				"Default",
				"{controller}/{action}/{id}",
				new { controller = "Home", action = "Index", id = UrlParameter.Optional },
				new string[] { "Malsys.Web.Controllers" }
			);

		}

		private IDependencyResolver buildDependencyResolver() {

			var builder = new ContainerBuilder();

			// registers all MVC controllers in this assembly
			builder.RegisterControllers(typeof(MvcApplication).Assembly);

			builder.RegisterType<StandardDateTimeProvider>().As<IDateTimeProvider>().SingleInstance();
			builder.RegisterType<Sha512PasswordHasher>().As<IPasswordHasher>().SingleInstance();

			builder.RegisterType<MalsysDb>()
				.As<IUsersDb>()
				.As<IInputDb>()
				.As<IFeedbackDb>()
				.InstancePerHttpRequest();

			builder.RegisterType<UsersRepository>().As<IUsersRepository>().InstancePerHttpRequest();
			builder.RegisterType<MalsysInputRepository>().As<IMalsysInputRepository>().InstancePerHttpRequest();

			builder.RegisterType<UserAuthenticator>().As<IUserAuthenticator>().InstancePerHttpRequest();
			builder.RegisterType<FormsAuthenticationProvider>().As<IAuthenticationProvider>().SingleInstance();

			builder.RegisterType<AppSettingsProvider>().As<IAppSettingsProvider>().SingleInstance();

			string basePath = Server.MapPath("~/bin");  // path where DLL and theirs XML doc files are
			builder.Register(x => new XmlDocReader(basePath)).SingleInstance();
			builder.RegisterType<ComponentXmlDocLoader>().As<IComponentXmlDocLoader>().SingleInstance();


			registerMalsysStuff(builder);


			return new AutofacDependencyResolver(builder.Build());

		}

		private void registerMalsysStuff(ContainerBuilder builder) {

			var eec = new FunctionDumper().RegiterAllFunctions(typeof(StdFunctions), new ExpressionEvaluatorContext());
			builder.Register(x => eec).As<IExpressionEvaluatorContext>().SingleInstance();

			var knownStuffProvider = new KnownConstOpProvider();
			knownStuffProvider.LoadConstants(typeof(StdConstants));
			knownStuffProvider.LoadOperators(typeof(StdOperators));
			builder.Register(x => knownStuffProvider)
				.As<IKnownConstantsProvider>()
				.As<IKnownOperatorsProvider>()
				.SingleInstance();

			builder.RegisterType<CompilersContainer>().As<ICompilersContainer>().InstancePerHttpRequest();
			builder.RegisterType<EvaluatorsContainer>().As<IEvaluatorsContainer>().InstancePerHttpRequest();

			var componentResolver = new CachedComponentResolver();
			var componentsTypes = Assembly.GetAssembly(typeof(CachedComponentResolver)).GetTypes()
				.Where(t => (t.IsClass || t.IsInterface) && (typeof(IComponent)).IsAssignableFrom(t));
			foreach (var type in componentsTypes) {
				componentResolver.RegisterComponentNameAndFullName(type, false);
			}
			builder.Register(x => componentResolver)
				.As<IComponentMetadataContainer>()
				.As<IComponentMetadataResolver>()
				.SingleInstance();

			builder.RegisterType<ProcessManager>().InstancePerHttpRequest();
			builder.RegisterType<LsystemProcessor>().InstancePerHttpRequest();

			builder.Register(x => buildStdLib(knownStuffProvider, eec)).SingleInstance();

		}

		private InputBlockEvaled buildStdLib(KnownConstOpProvider knownStuffProvider, IExpressionEvaluatorContext eec) {

			string resName = ResourcesHelper.StdLibResourceName;

			var logger = new MessageLogger();

			using (Stream stream = new ResourcesReader().GetResourceStream(resName)) {
				using (TextReader reader = new StreamReader(stream)) {
					var inCompiled = new CompilersContainer(knownStuffProvider, knownStuffProvider)
						.CompileInput(reader, resName, logger);
					var stdLib = new EvaluatorsContainer(eec).EvaluateInput(inCompiled);
					if (!logger.ErrorOccurred) {
						return stdLib;
					}
				}
			}

			throw new Exception("Failed to build std lib.");
		}

		/// <summary>
		/// Checks whether DB contains all well-known user roles and at least one user.
		/// Missing things are added.
		/// </summary>
		/// <remarks>
		/// If there are no user groups in the database, standard user groups from class <c>UserRoles</c> are created.
		/// If there are no users in the database, "Administrator" user with password "malsys" is created.
		/// </remarks>
		private void initializeDb(IDependencyResolver resolver) {

			var usersRepo = resolver.GetService<IUsersRepository>();

			foreach (var fi in typeof(UserRoles).GetFields(BindingFlags.Public | BindingFlags.Static)) {

				if (fi.FieldType != typeof(string) || !fi.IsLiteral) {
					continue;
				}

				string value = (string)fi.GetValue(null);

				if (usersRepo.Roles.Where(x => x.NameLowercase == value).Count() == 0) {
					usersRepo.CreateRole(new NewRoleModel() { RoleName = value });
				}

			}

			if (usersRepo.Users.Count() == 0) {
				// DB has no users -- lets create admin
				var adminUser = usersRepo.CreateUser(new NewUserModel() {
					UserName = "Administrator",
					Email = "temp@email.com",
					Password = "malsys",
					ConfirmPassword = "malsys"
				});

				var adminRole = usersRepo.Roles.Where(x => x.NameLowercase == UserRoles.Administrator).Single();

				usersRepo.AddUserToRole(adminUser.UserId, adminRole.RoleId);

			}

		}


		private void checkFileSystem(IDependencyResolver resolver) {

			var appSettingsProvider = resolver.GetService<IAppSettingsProvider>();

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


		private class InvariantCultureActionFilter : IActionFilter {

			public void OnActionExecuting(ActionExecutingContext filterContext) {
				Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
				Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
			}

			public void OnActionExecuted(ActionExecutedContext filterContext) { }

		}

	}
}
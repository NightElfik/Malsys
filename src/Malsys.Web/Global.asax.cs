using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using Malsys.Compilers;
using Malsys.Evaluators;
using Malsys.Processing;
using Malsys.Processing.Components;
using Malsys.Reflection;
using Malsys.Resources;
using Malsys.SemanticModel.Evaluated;
using Malsys.Web.Entities;
using Malsys.Web.Infrastructure;
using Malsys.Web.Models;
using Malsys.Web.Models.Repositories;
using Malsys.Web.Security;

namespace Malsys.Web {
	public class MvcApplication : HttpApplication {

		public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
			//filters.Add(new HandleErrorAttribute());
		}

		public static void RegisterRoutes(RouteCollection routes) {
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				"Default",
				"{controller}/{action}/{id}",
				new { controller = "Home", action = "Index", id = UrlParameter.Optional },
				new string[] { "Malsys.Web.Controllers" }
			);


		}

		protected void Application_Start() {

			var builder = new ContainerBuilder();
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

			string basePath = Server.MapPath("~/bin");
			builder.Register(x => new XmlDocReader(basePath)).SingleInstance();

			registerMalsysStuff(builder);


			var container = builder.Build();
			DependencyResolver.SetResolver(new AutofacDependencyResolver(container));


			AreaRegistration.RegisterAllAreas();

			RegisterGlobalFilters(GlobalFilters.Filters);
			RegisterRoutes(RouteTable.Routes);

		}

		private void registerMalsysStuff(ContainerBuilder builder) {

			var knownStuffProvider = new KnownConstFunOpProvider();
			knownStuffProvider.LoadFromType(typeof(StdConstants));
			knownStuffProvider.LoadFromType(typeof(StdFunctions));
			knownStuffProvider.LoadFromType(typeof(StdOperators));
			builder.Register(x => knownStuffProvider)
				.As<IKnownConstantsProvider>()
				.As<IKnownFunctionsProvider>()
				.As<IKnownOperatorsProvider>()
				.SingleInstance();

			builder.RegisterType<CompilersContainer>().InstancePerHttpRequest();
			builder.RegisterType<EvaluatorsContainer>().InstancePerHttpRequest();

			var componentResolver = new ComponentResolver();
			var componentsTypes = Assembly.GetAssembly(typeof(ComponentResolver)).GetTypes()
				.Where(t => (t.IsClass || t.IsInterface) && (typeof(IProcessComponent)).IsAssignableFrom(t));
			foreach (var type in componentsTypes) {
				componentResolver.RegisterComponentNameAndFullName(type, false);
			}
			builder.Register(x => componentResolver)
				.As<IComponentResolver>()
				.As<IComponentContainer>()
				.SingleInstance();

			builder.RegisterType<ProcessManager>().InstancePerHttpRequest();

			builder.Register(x => buildStdLib(knownStuffProvider)).SingleInstance();

		}

		private InputBlock buildStdLib(KnownConstFunOpProvider knownStuffProvider) {

			const string resName = "StdLib.malsys";

			var logger = new MessageLogger();

			using (Stream stream = new ResourcesReader().GetResourceStream(resName)) {
				using (TextReader reader = new StreamReader(stream)) {
					var inCompiled = new CompilersContainer(knownStuffProvider, knownStuffProvider, knownStuffProvider)
						.CompileInput(reader, resName, logger);
					var stdLib = new EvaluatorsContainer().EvaluateInput(inCompiled);
					if (!logger.ErrorOcured) {
						return stdLib;
					}
				}
			}

			throw new Exception("Failed to build std lib.");
		}

	}
}
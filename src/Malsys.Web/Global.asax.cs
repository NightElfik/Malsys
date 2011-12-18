using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using Malsys.Web.Entities;
using Malsys.Web.Models;
using Malsys.Web.Models.Repositories;
using Malsys.Web.Security;
using Malsys.Web.Infrastructure;

namespace Malsys.Web {
	public class MvcApplication : HttpApplication {

		public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
			filters.Add(new HandleErrorAttribute());
		}

		public static void RegisterRoutes(RouteCollection routes) {
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				"Default",
				"{controller}/{action}/{id}",
				new { controller = "Home", action = "Index", id = UrlParameter.Optional }
			);

		}

		protected void Application_Start() {

			var builder = new ContainerBuilder();

			builder.RegisterType<StandardDateTimeProvider>().As<IDateTimeProvider>().SingleInstance();
			builder.RegisterType<Sha512PasswordHasher>().As<IPasswordHasher>().SingleInstance();

			builder.RegisterType<MalsysDb>()
				.As<IUsersDb>()
				.As<IInputDb>()
				.InstancePerHttpRequest();

			builder.RegisterType<UsersRepository>().As<IUsersRepository>().InstancePerHttpRequest();
			builder.RegisterType<InputProcessesRepository>().As<IInputProcessesRepository>().InstancePerHttpRequest();

			builder.RegisterType<UserAuthenticator>().As<IUserAuthenticator>().InstancePerHttpRequest();
			builder.RegisterType<FormsAuthenticationProvider>().As<IAuthenticationProvider>().SingleInstance();

			builder.RegisterType<AppSettingsProvider>().As<IAppSettingsProvider>().SingleInstance();


			builder.RegisterControllers(typeof(MvcApplication).Assembly);

			var container = builder.Build();
			DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

			AreaRegistration.RegisterAllAreas();

			RegisterGlobalFilters(GlobalFilters.Filters);
			RegisterRoutes(RouteTable.Routes);
		}
	}
}
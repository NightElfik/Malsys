using System.Web.Mvc;
using System.Web.Routing;

namespace Malsys {
	public static class RouteConfig {

		public static void RegisterRoutes(RouteCollection routes) {

			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
			
			routes.MapRoute(
				"Permalink",
				MVC.Permalink.Name.ToLower() + "/{id}",
				new { controller = MVC.Permalink.Name, action = MVC.Permalink.ActionNames.Index },
				new string[] { "Malsys.Web.Controllers" }
			);

			routes.MapRoute(
				 "Default",
				 "{controller}/{action}/{id}",
				 new { controller = "Home", action = "Index", id = UrlParameter.Optional },
				 new string[] { "Malsys.Web.Controllers" }
			 );

		}
	}
}
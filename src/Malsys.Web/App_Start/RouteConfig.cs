using System.Web.Mvc;
using System.Web.Routing;

namespace Malsys {
	public static class RouteConfig {

		public static void RegisterRoutes(RouteCollection routes) {

			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				name: "Permalink",
				url: MVC.Permalink.Name.ToLower() + "/{id}",
				defaults: new {
					controller = MVC.Permalink.Name,
					action = MVC.Permalink.ActionNames.Index,
				},
				namespaces: new string[] { "Malsys.Web.Controllers" }
			);

			routes.MapRoute(
				name: "Gallery",
				url: MVC.Gallery.Name + "/{action}/{id}/{cacheBust}/{extra}",
				defaults: new {
					controller = MVC.Gallery.Name,
					extra = UrlParameter.Optional,
				},
				namespaces: new string[] { "Malsys.Web.Controllers" }
			);

			routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new {
					controller = "Home",
					action = "Index",
					id = UrlParameter.Optional,
				},
				namespaces: new string[] { "Malsys.Web.Controllers" }
			);

		}
	}
}
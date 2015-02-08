using System.Web.Mvc;

namespace Malsys.Web.Areas.Documentation {
	public class DocumentationAreaRegistration : AreaRegistration {

		public override string AreaName {
			get { return "Documentation"; }
		}

		public override void RegisterArea(AreaRegistrationContext context) {

			context.MapRoute(
				name: "Documentation articles",
				url: MVC.Documentation.Name + "/" + MVC.Documentation.Articles.Name + "/{name}/{part}",
				defaults: new {
					controller = MVC.Documentation.Articles.Name,
					action = MVC.Documentation.Articles.ActionNames.Article,
					part = UrlParameter.Optional,
				}
			);

			context.MapRoute(
				name: "Documentation default",
				url: MVC.Documentation.Name + "/{controller}/{action}/{id}",
				defaults: new {
					controller = MVC.Documentation.Home.Name,
					action = "Index",
					id = UrlParameter.Optional
				}
			);

		}

	}
}

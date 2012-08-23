/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System.Web.Mvc;

namespace Malsys.Web.Areas.Documentation {
	public class DocumentationAreaRegistration : AreaRegistration {

		public override string AreaName {
			get { return "Documentation"; }
		}

		public override void RegisterArea(AreaRegistrationContext context) {

			context.MapRoute(
				"Documentation sitemap",
				"Documentation/sitemap.xml",
				new { controller = MVC.Documentation.Home.Name, action = MVC.Documentation.Home.ActionNames.Sitemap }
			);

			context.MapRoute(
				"Documentation default",
				"Documentation/{controller}/{action}/{id}",
				new { controller = MVC.Documentation.Home.Name, action = "Index", id = UrlParameter.Optional }
			);

		}

	}
}

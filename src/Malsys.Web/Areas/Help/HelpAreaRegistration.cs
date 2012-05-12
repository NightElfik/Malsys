/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System.Web.Mvc;

namespace Malsys.Web.Areas.Help {
	public class HelpAreaRegistration : AreaRegistration {

		public override string AreaName {
			get { return "Help"; }
		}

		public override void RegisterArea(AreaRegistrationContext context) {

			context.MapRoute(
				"Help sitemap",
				"Help/sitemap.xml",
				new { controller = MVC.Help.Home.Name, action = MVC.Help.Home.ActionNames.Sitemap }
			);

			context.MapRoute(
				"Help default",
				"Help/{controller}/{action}/{id}",
				new { controller = MVC.Help.Home.Name, action = "Index", id = UrlParameter.Optional }
			);

		}

	}
}

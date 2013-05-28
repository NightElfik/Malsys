// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System.Web.Mvc;

namespace Malsys.Web.Areas.Administration {
	public class AdministrationAreaRegistration : AreaRegistration {

		public override string AreaName {
			get { return "Administration"; }
		}

		public override void RegisterArea(AreaRegistrationContext context) {
			context.MapRoute(
				"Administration_default",
				"administration/{controller}/{action}/{id}",
				new { action = "Index", id = UrlParameter.Optional }
			);
		}

	}
}

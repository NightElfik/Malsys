using System.Globalization;
using System.Threading;
using System.Web.Mvc;


namespace Malsys {
	public static class FilterConfig {
		public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
			//filters.Add(new HandleErrorAttribute());
			filters.Add(new InvariantCultureActionFilter());
		}
	}

	internal class InvariantCultureActionFilter : IActionFilter {

		public void OnActionExecuting(ActionExecutingContext filterContext) {
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
		}

		public void OnActionExecuted(ActionExecutedContext filterContext) { }

	}
}

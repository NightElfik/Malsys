using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using RazorGenerator.Mvc;

[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(Malsys.Web.App_Start.RazorGeneratorMvcStart), "Start")]

namespace Malsys.Web.App_Start {
	public static class RazorGeneratorMvcStart {
		public static void Start() {
			var engine = new PrecompiledMvcEngine(typeof(RazorGeneratorMvcStart).Assembly) {
#if DEBUG
				UsePhysicalViewsIfNewer = true
#else
				UsePhysicalViewsIfNewer = HttpContext.Current.Request.IsLocal
#endif
			};

			ViewEngines.Engines.Insert(0, engine);

			// StartPage lookups are done by WebPages.
			VirtualPathFactoryManager.RegisterVirtualPathFactory(engine);
		}
	}
}

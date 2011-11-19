using System.Web.Mvc;
using Malsys.Processing;

namespace Malsys.Web.Controllers {
	public class RenderController : Controller {

		public ActionResult Text() {
			return View();
		}

		[HttpPost]
		public ActionResult Text(string src) {

			var renderMgr = new RenderManager();
			var fileMgr = new FilesManager(Server.MapPath(Url.Content("~/WorkDir")));
			var msgs = new MessagesCollection();

			renderMgr.RenderAllLsystemsDefault(src, fileMgr, msgs, false);

			ViewBag.Results = fileMgr.GetOutputFilePaths();
			ViewBag.Messages = msgs;

			return View();
		}
	}
}

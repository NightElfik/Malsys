using System.Web.Mvc;
using Malsys.Processing;
using Malsys.Web.Models;

namespace Malsys.Web.Controllers {
	public class ProcessLsystemController : Controller {

		public ActionResult Text() {
			return View(new ProcessLsystemResultModel());
		}

		[HttpPost]
		public ActionResult Text(string sourceCode) {

			var renderMgr = new RenderManager();
			var fileMgr = new FilesManager(Server.MapPath(Url.Content("~/WorkDir")));
			var msgs = new MessagesCollection();

			renderMgr.RenderAllLsystemsDefault(sourceCode, fileMgr, msgs, false);

			var resultModel = new ProcessLsystemResultModel();
			resultModel.SourceCode = sourceCode;
			resultModel.Messages = msgs;
			resultModel.OutputFiles = fileMgr.GetOutputFilePaths();

			return View(resultModel);
		}
	}
}

using System;
using System.Web.Mvc;
using Malsys.Processing;
using Malsys.Web.Models;

namespace Malsys.Web.Controllers {
	public class ProcessLsystemController : Controller {

		public ActionResult Index(string lsystem = null) {

			if (lsystem != null) {
				return IndexPost(lsystem);
			}

			return View(new ProcessLsystemResultModel());
		}

		[HttpPost]
		[ActionName("Index")]
		public ActionResult IndexPost(string sourceCode) {


			var renderMgr = new RenderManager() { Timeout = new TimeSpan(0, 0, 5) };
			var fileMgr = new FilesManager(Server.MapPath(Url.Content("~/WorkDir")));
			var msgs = new MessageLogger();

			renderMgr.RenderAllLsystemsDefault(sourceCode, fileMgr, msgs, false);

			var resultModel = new ProcessLsystemResultModel();
			resultModel.SourceCode = sourceCode;
			resultModel.Messages = msgs;
			resultModel.OutputFiles = fileMgr.GetOutputFilePaths();

			return View(resultModel);
		}
	}
}

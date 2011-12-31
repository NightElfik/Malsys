using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Malsys.Web.Models;
using Malsys.Web.Infrastructure;
using System.IO;

namespace Malsys.Web.Controllers {
	public partial class ProcessOutputController : Controller {

		private readonly string workDir;
		private readonly IMalsysInputRepository inputProcessesRepository;
		private readonly IDateTimeProvider dateTimeProvider;


		public ProcessOutputController(IMalsysInputRepository inputProcessesRepository, IAppSettingsProvider appSettingsProvider,
				IDateTimeProvider dateTimeProvider) {

			this.inputProcessesRepository = inputProcessesRepository;
			this.dateTimeProvider = dateTimeProvider;
			workDir = appSettingsProvider["WorkDir"];
		}


		public virtual ActionResult Show(string fileName) {
			return download(fileName, false);
		}

		public virtual ActionResult Download(string fileName) {
			return download(fileName, true);
		}


		private ActionResult download(string fileName, bool download) {

			var procOutput = inputProcessesRepository.InputDb.ProcessOutputs.Where(po => po.FileName == fileName).SingleOrDefault();

			if (procOutput == null) {
				return new HttpNotFoundResult("File `" + fileName + "` not found.");
			}

			procOutput.LastOpenDate = dateTimeProvider.Now;
			inputProcessesRepository.InputDb.SaveChanges();

			string realPath = Server.MapPath(Url.Content(workDir));
			string filePath = Path.Combine(realPath, fileName);


			if (download) {
				return File(filePath, "application/octet-stream", fileName);
			}
			else {
				string mime;
				switch (Path.GetExtension(filePath)) {
					case ".txt": mime = "text/plain"; break;
					case ".svg":
						mime = "image/svg+xml";
						break;
					case ".svgz":
						Response.AppendHeader("Content-Encoding", "gzip");
						mime = "image/svg+xml";
						break;
					default: mime = "application/octet-stream"; break;
				}
				return File(filePath, mime);
			}
		}

	}
}

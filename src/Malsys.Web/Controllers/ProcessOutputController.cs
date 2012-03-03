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

			string mimeType = MimeType.Text.Plain;
			if (fileName.EndsWith(".svg")) {
				mimeType = MimeType.Image.SvgXml;
			}
			else if (fileName.EndsWith(".svgz")) {
				Response.AppendHeader("Content-Encoding", "gzip");
				mimeType = MimeType.Image.SvgXml;
			}

			return download(fileName, mimeType, false);
		}

		public virtual ActionResult Download(string fileName) {
			return download(fileName, "application/octet-stream", true);
		}


		private ActionResult download(string fileName, string mimeType, bool download) {

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
				return File(filePath, mimeType);
			}
		}

	}
}

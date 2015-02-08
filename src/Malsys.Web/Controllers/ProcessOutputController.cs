using System.IO;
using System.Linq;
using System.Web.Mvc;
using Malsys.Web.Infrastructure;
using Malsys.Web.Models;
using Malsys.Web.Models.Lsystem;

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


		public virtual ActionResult Show(string id, string extra = null) {
			return download(id, false, extra);
		}

		public virtual ActionResult Download(string id, string extra = null) {
			return download(id, true, extra);
		}


		private ActionResult download(string fileName, bool download, string extra) {

			var procOutput = inputProcessesRepository.InputDb.ProcessOutputs.SingleOrDefault(po => po.FileName == fileName);

			if (procOutput == null) {
				return HttpNotFound();
			}

			procOutput.LastOpenDate = dateTimeProvider.Now;
			inputProcessesRepository.InputDb.SaveChanges();

			string realPath = Server.MapPath(Url.Content(workDir));
			string filePath = Path.Combine(realPath, fileName);


			var metadata = OutputMetadataHelper.DeserializeMetadata(procOutput.Metadata);
			string mimeType;
			MemoryStream ms;

			if (OutputHelper.HandleAdvancedOutput(filePath, extra, metadata, ref fileName, out ms, out mimeType)) {
				// Handle requests OBJ and MTL files from ZIP package.
				if (ms == null || fileName == null || mimeType == null) {
					return HttpNotFound();
				}
				return File(ms, download ? MimeType.Application.OctetStream : mimeType, fileName);
			}

			if (download) {
				return File(filePath, MimeType.Application.OctetStream, fileName);
			}
			else {
				if (fileName.EndsWith(".svg")) {
					mimeType = MimeType.Image.SvgXml;
				}
				else if (fileName.EndsWith(".svgz")) {
					Response.AppendHeader("Content-Encoding", "gzip");
					mimeType = MimeType.Image.SvgXml;
				}
				else {
					mimeType = MimeType.Text.Plain;
				}

				return File(filePath, download ? MimeType.Application.OctetStream : mimeType);
			}
		}

	}
}

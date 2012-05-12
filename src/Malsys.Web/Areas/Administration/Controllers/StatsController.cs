using System.IO;
/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System.Linq;
using System.Web.Mvc;
using Malsys.Web.Areas.Administration.Models;
using Malsys.Web.Entities;
using MvcContrib.Pagination;

namespace Malsys.Web.Areas.Administration.Controllers {
	[Authorize(Roles = UserRoles.ViewStats)]
	public partial class StatsController : Controller {

		private readonly IInputDb inputDb;
		private readonly IUsersDb usersDb;


		public StatsController(IInputDb inputDb, IUsersDb usersDb) {
			this.inputDb = inputDb;
			this.usersDb = usersDb;
		}


		public virtual ActionResult Index() {
			return View();
		}

		public virtual ActionResult Input(int page = 1) {

			if (page <= 0) {
				page = 1;
			}

			var result = inputDb.InputProcesses
				.OrderByDescending(p => p.ProcessDate)
				.Select(p => new InputProcessesHistoryItem {
					ProcessId = p.InputProcessId,
					ParentProcessId = p.ParentInputProcessId,
					User = p.User.Name,
					Date = p.ProcessDate,
					Duration = p.Duration,
					SourceId = p.CanonicInput.CanonicInputId,
					Source = p.CanonicInput.SourceCode
				})
				.AsPagination(page, 10);

			return View(result);
		}


		public virtual ActionResult SavedInputs(int page = 1) {

			if (page <= 0) {
				page = 1;
			}

			var result = inputDb.SavedInputs
				.OrderByDescending(x => x.EditDate)
				.Select(x => new SavedInputModel {
					SavedInputId = x.SavedInputId,
					UrlId = x.UrlId,
					User = x.User.Name,
					EditDate = x.EditDate,
					Duration = x.Duration,
					SourceCode = x.SourceCode
				})
				.AsPagination(page, 10);

			return View(result);
		}


		public virtual ActionResult ExportInputs(bool? published = null, bool? deleted = null) {

			var inputs = inputDb.SavedInputs;
			if (published != null) {
				inputs = inputs.Where(x => x.IsPublished == published);
			}
			if (deleted != null) {
				inputs = inputs.Where(x => x.IsDeleted == deleted);
			}

			const string delimiter = "// =============================================================================";

			var stream = new MemoryStream();
			var writer = new StreamWriter(stream);

			foreach (var i in inputs) {
				writer.WriteLine(delimiter);
				writer.WriteLine("// Name: " + i.PublishName);
				writer.WriteLine("// Author: " + i.User.Name);
				writer.WriteLine("// Published (deleted): {0} ({1})".Fmt(i.IsPublished, i.IsDeleted));
				writer.WriteLine("// Creation (edit) date: {0} ({1})".Fmt(i.CreationDate.ToUniversalTime(), i.EditDate.ToUniversalTime()));
				writer.WriteLine("// Rating (count): {0} ({1})".Fmt(((float)i.RatingSum / (float)i.RatingCount), i.RatingCount));
				writer.WriteLine("// Views: " + i.Views);
				writer.WriteLine(i.SourceCode);
				writer.WriteLine("// Thn extension");
				writer.WriteLine(i.ThumbnailSourceExtension);
				writer.WriteLine();
				writer.WriteLine();
			}

			stream.Seek(0, SeekOrigin.Begin);
			return File(stream, MimeType.Text.Plain);

		}

	}
}

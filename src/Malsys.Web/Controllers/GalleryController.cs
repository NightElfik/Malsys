using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Elmah;
using Malsys.Processing;
using Malsys.Processing.Output;
using Malsys.SemanticModel.Evaluated;
using Malsys.Web.Entities;
using Malsys.Web.Infrastructure;
using Malsys.Web.Models;
using Malsys.Web.Models.Lsystem;
using MvcContrib.Pagination;

namespace Malsys.Web.Controllers {
	public partial class GalleryController : Controller {


		private readonly IMalsysInputRepository malsysInputRepository;
		private readonly LsystemProcessor lsystemProcessor;
		private readonly IAppSettingsProvider appSettingsProvider;
		private readonly IDateTimeProvider dateTimeProvider;



		public GalleryController(IMalsysInputRepository malsysInputRepository, IAppSettingsProvider appSettingsProvider, LsystemProcessor lsystemProcessor,
				IDateTimeProvider dateTimeProvider) {

			this.malsysInputRepository = malsysInputRepository;
			this.lsystemProcessor = lsystemProcessor;
			this.appSettingsProvider = appSettingsProvider;
			this.dateTimeProvider = dateTimeProvider;

		}


		public virtual ActionResult Index(string user = null, string tag = null, int? page = null) {

			if (page == null || page <= 0) {
				page = 1;
			}


			var inputs = malsysInputRepository.InputDb.SavedInputs
				.Where(x => !x.IsDeleted);

			bool owner = user != null && user.ToLower() == User.Identity.Name.ToLower();
			if (!owner) {
				inputs = inputs.Where(x => x.IsPublished);
			}

			if (user != null) {
				string userLower = user.ToLower();
				inputs = inputs.Where(x => x.User.NameLowercase == userLower);
			}

			if (tag != null) {
				string tagLower = tag.ToLower();
				var tagEntity = malsysInputRepository.InputDb.Tags.Where(x => x.NameLowercase == tagLower).SingleOrDefault();
				if (tagEntity != null) {
					inputs = inputs.Where(x => x.Tags.Where(t => t.TagId == tagEntity.TagId).Count() == 1);
				}
			}

			var model = inputs.OrderByDescending(x => x.Rating)
				.AsPagination(page.Value);

			return View(model);
		}

		public virtual ActionResult Detail(string id) {

			var input = malsysInputRepository.InputDb.SavedInputs
				.Where(x => x.UrlId == id && !x.IsDeleted)
				.SingleOrDefault();

			if (input == null || input.IsDeleted) {
				return HttpNotFound();
			}

			bool owner = User.Identity.Name.ToLower() == input.User.NameLowercase;
			if (!input.IsPublished && !owner) {
				return HttpNotFound();
			}

			string filePath = Path.Combine(Server.MapPath(Url.Content(appSettingsProvider[AppSettingsKeys.GalleryWorkDir])), input.UrlId);
			ensureOutput(input);

			var model = new InputDetail() {
				UrlId = input.UrlId,
				Name = input.PublishName,
				AuthorName = input.User.Name,
				CurrentUserIsOwner = owner,
				IsPublished = input.IsPublished,
				Rating = input.Rating,
				UserVote = malsysInputRepository.GetUserVote(input.UrlId, User.Identity.Name),
				Tags = input.Tags.Select(x => x.Name).ToArray(),
				SourceCode = input.SourceCode,
				FilePath = filePath,
				MimeType = input.MimeType,
				Description = input.Description,
				Metadata = OutputMetadataHelper.DeserializeMetadata(input.OutputMetadata)
			};

			return View(model);
		}


		[Authorize]
		public virtual ActionResult UpVote(string id) {

			malsysInputRepository.Vote(id, User.Identity.Name, true);
			return RedirectToAction(Actions.Detail(id));

		}

		[Authorize]
		public virtual ActionResult DownVote(string id) {

			malsysInputRepository.Vote(id, User.Identity.Name, false);
			return RedirectToAction(Actions.Detail(id));
		}


		[Authorize]
		public virtual ActionResult Edit(string id) {

			string userNameLower = User.Identity.Name.ToLower();

			var input = malsysInputRepository.InputDb.SavedInputs
				.Where(x => x.UrlId == id && !x.IsDeleted)
				.Where(x => x.User.NameLowercase == userNameLower)
				.SingleOrDefault();

			var model = new EditSavedInputModel() {
				UrlId = input.UrlId,
				Name = input.PublishName,
				SourceCode = input.SourceCode,
				Publish = input.IsPublished,
				Tags = string.Join(" ", input.Tags.Select(x => x.Name)),
				Description = input.Description
			};

			return View(model);

		}

		[Authorize]
		[HttpPost]
		public virtual ActionResult Edit(string id, EditSavedInputModel model) {

			string userNameLower = User.Identity.Name.ToLower();

			var input = malsysInputRepository.InputDb.SavedInputs
				.Where(x => x.UrlId == id && !x.IsDeleted)
				.Where(x => x.User.NameLowercase == userNameLower)
				.SingleOrDefault();

			if (input == null) {
				return HttpNotFound();
			}

			if (!ModelState.IsValid) {
				return View(model);
			}

			input.PublishName = model.Name;
			input.SourceCode = model.SourceCode;
			input.IsPublished = model.Publish;
			input.Description = model.Description;

			if (model.Tags == null) {
				model.Tags = "";
			}
			var tags = malsysInputRepository.GetTags(model.Tags.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
			input.Tags.Clear();
			foreach (var tag in tags) {
				input.Tags.Add(tag);
			}

			var logger = new MessageLogger();
			if (!tryGenerateInput(input, logger)) {
				ModelState.AddModelError("", "Failed to generate input.");
				foreach (var msg in logger) {
					ModelState.AddModelError("", msg.GetFullMessage());
				}
				return View(model);
			}

			input.EditDate = dateTimeProvider.Now;
			malsysInputRepository.InputDb.SaveChanges();

			return RedirectToAction(Actions.Detail(input.UrlId));
		}

		public virtual ActionResult GetOutput(string id) {

			var input = malsysInputRepository.InputDb.SavedInputs
				.Where(x => x.UrlId == id && !x.IsDeleted)
				.SingleOrDefault();

			if (input == null) {
				return new HttpNotFoundResult();
			}

			string workDirFullPath;
			string filePath = getFilePath(input.UrlId, out workDirFullPath);

			if (!System.IO.File.Exists(filePath)) {
				if (tryGenerateInput(input, workDirFullPath, filePath)) {
					if (!System.IO.File.Exists(filePath)) {
						ErrorSignal.FromCurrentContext().Raise(new Exception("Generation of input `{0}` in gallery returned true but file `{1}` not found."
							.Fmt(input.UrlId, filePath)));
						return new HttpNotFoundResult();
					}
					malsysInputRepository.InputDb.SaveChanges();
				}
				else {
					return new HttpNotFoundResult();
				}
			}

			var metadata = OutputMetadataHelper.DeserializeMetadata(input.OutputMetadata);

			if (metadata.Contains(new KeyValuePair<string, object>(OutputMetadataKeyHelper.OutputIsGZipped, true))) {
				Response.AppendHeader("Content-Encoding", "gzip");
			}

			return File(filePath, input.MimeType);
		}

		public virtual ActionResult GetThumbnail(string id) {
			return GetOutput(id);
		}



		private string getFilePath(string urlId) {
			string workDirFullPath;
			return getFilePath(urlId, out workDirFullPath);
		}

		private string getFilePath(string urlId, out string workDirFullPath) {
			string workDir = appSettingsProvider[AppSettingsKeys.GalleryWorkDir];
			workDirFullPath = Server.MapPath(Url.Content(workDir));
			return Path.Combine(workDirFullPath, urlId);
		}


		private void ensureOutput(SavedInput input) {

			string workDirFullPath;
			string filePath = getFilePath(input.UrlId, out workDirFullPath);

			if (System.IO.File.Exists(filePath)) {
				return;
			}

			if (tryGenerateInput(input, workDirFullPath, filePath)) {
				malsysInputRepository.InputDb.SaveChanges();
			}

		}


		/// <remarks>
		/// Also saves information about output (mime type, metadata) to given input entity.
		/// </remarks>
		private bool tryGenerateInput(SavedInput input, IMessageLogger logger = null) {

			string workDirFullPath;
			string filePath = getFilePath(input.UrlId, out workDirFullPath);

			return tryGenerateInput(input, workDirFullPath, filePath, logger);
		}

		/// <remarks>
		/// Also saves information about output (mime type, metadata) to given input entity.
		/// </remarks>
		private bool tryGenerateInput(SavedInput input, string workDirFullPath, string outputFilePath, IMessageLogger logger = null) {

			TimeSpan timeout = new TimeSpan(0, 0, int.Parse(appSettingsProvider[AppSettingsKeys.GalleryProcessTime]));
			var fileMgr = new FileOutputProvider(workDirFullPath);

			if (logger == null) {
				logger = new MessageLogger();
			}

			InputBlockEvaled evaledInput;
			bool result = lsystemProcessor.TryProcess(input.SourceCode, timeout, fileMgr, logger, out evaledInput);
			if (!result) {
				return false;
			}

			var outputs = fileMgr.GetOutputFiles().ToList();
			if (outputs.Count == 0) {
				return false;
			}

			var output = outputs[0];
			input.MimeType = output.MimeType;
			input.OutputMetadata = OutputMetadataHelper.SerializeMetadata(output.Metadata);

			System.IO.File.Delete(outputFilePath);
			System.IO.File.Copy(output.FilePath, outputFilePath);

			foreach (var o in outputs) {
				try {
					System.IO.File.Delete(o.FilePath);
				}
				catch (Exception) {
					// TODO
				}
			}

			return true;

		}

	}
}

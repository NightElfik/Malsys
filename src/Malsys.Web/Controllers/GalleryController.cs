// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
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



		public GalleryController(IMalsysInputRepository malsysInputRepository, IAppSettingsProvider appSettingsProvider,
				LsystemProcessor lsystemProcessor, IDateTimeProvider dateTimeProvider) {

			this.malsysInputRepository = malsysInputRepository;
			this.lsystemProcessor = lsystemProcessor;
			this.appSettingsProvider = appSettingsProvider;
			this.dateTimeProvider = dateTimeProvider;

		}


		public virtual ActionResult Index(string user = null, string tag = null, int? page = null) {

			if (page == null || page <= 0) {
				page = 1;
			}

			var model = new GalleryModel();

			var inputs = malsysInputRepository.InputDb.SavedInputs
				.Where(x => !x.IsDeleted);

			bool owner = user != null && user.ToLower() == User.Identity.Name.ToLower();
			if (!owner) {
				inputs = inputs.Where(x => x.IsPublished);
			}

			if (user != null) {
				string userLower = user.ToLower();
				inputs = inputs.Where(x => x.User.NameLowercase == userLower);
				model.UserFilter = user;
			}

			if (tag != null) {
				string tagLower = tag.ToLower();
				var tagEntity = malsysInputRepository.InputDb.Tags.Where(x => x.NameLowercase == tagLower).SingleOrDefault();
				if (tagEntity != null) {
					inputs = inputs.Where(x => x.Tags.Where(t => t.TagId == tagEntity.TagId).Count() == 1);
					model.TagFilter = tagEntity;
				}
			}

			model.Inputs = inputs
				.OrderByDescending(x => (float)x.RatingSum / ((float)x.RatingCount + 1) + x.RatingCount)
				.AsPagination(page.Value, 8);

			return View(model);
		}

		public virtual ActionResult Tags() {
			var tags = malsysInputRepository.InputDb.Tags.Select(t => new TagModel() {
				TagName = t.Name,
				Description = t.Description,
				Quantity = t.SavedInputs.Where(x => !x.IsDeleted && x.IsPublished).Count()
			});
			return View(tags);
		}

		public virtual ActionResult Detail(string id) {

			var input = malsysInputRepository.InputDb.SavedInputs
				.Where(x => x.UrlId == id && !x.IsDeleted)
				.SingleOrDefault();

			if (input == null) {
				return HttpNotFound();
			}

			bool isOwner = User.Identity.Name.ToLower() == input.User.NameLowercase;
			bool canEdit = isOwner || User.IsInRole(UserRoles.Administrator);
			if (!input.IsPublished && !canEdit) {
				return HttpNotFound();
			}

			ensureOutput(input, false);
			ensureOutput(input, true);

			input.Views++;

			var model = new InputDetail() {
				Input = input,
				IsAuthor = isOwner,
				CanEdit = canEdit,
				UserVote = malsysInputRepository.GetUserVote(input.UrlId, User.Identity.Name),
				FilePath = getFilePath(input.UrlId, false),
				ThnFilePath = getFilePath(input.UrlId, true),
				Metadata = OutputMetadataHelper.DeserializeMetadata(input.OutputMetadata)
			};

			malsysInputRepository.InputDb.SaveChanges();

			return View(model);
		}


		[Authorize]
		public virtual ActionResult Vote(string id, int rating) {

			malsysInputRepository.Vote(id, User.Identity.Name, MathHelper.Clamp(rating, 0, 5));
			return RedirectToAction(Actions.Detail(id));

		}


		[Authorize]
		public virtual ActionResult Edit(string id) {

			string userNameLower = User.Identity.Name.ToLower();
			bool isAdmin = User.IsInRole(UserRoles.Administrator);

			var input = malsysInputRepository.InputDb.SavedInputs
				.Where(x => x.UrlId == id && !x.IsDeleted)
				.Where(x => isAdmin || x.User.NameLowercase == userNameLower)
				.SingleOrDefault();

			var model = new EditSavedInputModel() {
				UrlId = input.UrlId,
				Name = input.PublishName,
				SourceCode = input.SourceCode,
				SourceCodeThn = input.ThumbnailSourceExtension,
				Publish = input.IsPublished,
				Tags = string.Join(" ", input.Tags.Select(x => x.Name)),
				Description = input.Description
			};

			ViewData["tags"] = malsysInputRepository.InputDb.Tags;

			return View(model);

		}

		[Authorize]
		[HttpPost]
		public virtual ActionResult Edit(string id, EditSavedInputModel model) {

			string userNameLower = User.Identity.Name.ToLower();
			bool isAdmin = User.IsInRole(UserRoles.Administrator);

			var input = malsysInputRepository.InputDb.SavedInputs
				.Where(x => x.UrlId == id && !x.IsDeleted)
				.Where(x => isAdmin || x.User.NameLowercase == userNameLower)
				.SingleOrDefault();

			if (input == null) {
				return HttpNotFound();
			}

			ViewData["tags"] = malsysInputRepository.InputDb.Tags;

			if (!ModelState.IsValid) {
				return View(model);
			}

			input.PublishName = model.Name;
			input.SourceCode = model.SourceCode;
			input.ThumbnailSourceExtension = model.SourceCodeThn;
			input.IsPublished = model.Publish;
			input.Description = model.Description;

			if (model.Tags == null) {
				model.Tags = "";
			}
			var tags = malsysInputRepository.GetTags(model.Tags.Split(new char[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries));
			input.Tags.Clear();
			foreach (var tag in tags) {
				input.Tags.Add(tag);
			}

			var logger = new MessageLogger();
			if (!tryGenerateInput(input, false, logger) || !tryGenerateInput(input, true, logger)) {
				ModelState.AddModelError("", "Failed to generate input or thumbnail.");
				foreach (var msg in logger) {
					ModelState.AddModelError("", msg.GetFullMessage());
				}
				return View(model);
			}

			input.EditDate = dateTimeProvider.Now;
			malsysInputRepository.InputDb.SaveChanges();

			return RedirectToAction(Actions.Detail(input.UrlId));
		}

		[OutputCache(CacheProfile = "LongClientCache")]
		public virtual ActionResult GetOutput(string id, string cacheBust, string extra = null) {
			return getOutput(id, extra, false);
		}

		[OutputCache(CacheProfile = "LongClientCache")]
		public virtual ActionResult GetThumbnail(string id, string cacheBust, string extra = null) {
			return getOutput(id, extra, true);
		}


		private ActionResult getOutput(string id, string extra, bool thumbnail) {
			var input = malsysInputRepository.InputDb.SavedInputs
				.Where(x => x.UrlId == id && !x.IsDeleted)
				.SingleOrDefault();

			if (input == null) {
				return new HttpNotFoundResult();
			}

			string workDirFullPath;
			string filePath = getFilePath(input.UrlId, thumbnail, out workDirFullPath);

			if (!System.IO.File.Exists(filePath)) {
				if (!tryGenerateInput(input, workDirFullPath, filePath, thumbnail)) {
					return HttpNotFound();
				}

				malsysInputRepository.InputDb.SaveChanges();  // metadata set by input processing
			}


			var metadata = OutputMetadataHelper.DeserializeMetadata(thumbnail ? input.OutputThnMetadata : input.OutputMetadata);

			if (metadata.Contains(new KeyValuePair<string, object>(OutputMetadataKeyHelper.OutputIsGZipped, true))) {
				Response.AppendHeader("Content-Encoding", "gzip");
			}

			string downFileName = StaticUrl.UrlizeString(input.PublishName) + MimeType.ToFileExtension(input.MimeType);
			string mimeType;
			MemoryStream ms;

			// Handle requests OBJ and MTL files from ZIP package.
			if (OutputHelper.HandleAdvancedOutput(filePath, extra, metadata, ref downFileName, out ms, out mimeType)) {
				if (ms == null || downFileName == null || mimeType == null) {
					return HttpNotFound();
				}
				return File(ms, mimeType, downFileName);
			}


			return File(filePath, input.MimeType, downFileName);
		}



		private string getFilePath(string urlId, bool thumbnail) {
			string workDirFullPath;
			return getFilePath(urlId, thumbnail, out workDirFullPath);
		}

		private string getFilePath(string urlId, bool thumbnail, out string workDirFullPath) {

			string workDir = appSettingsProvider[AppSettingsKeys.GalleryWorkDir];
			workDirFullPath = Server.MapPath(Url.Content(workDir));

			string fileName = urlId;
			if (thumbnail) {
				fileName += ".thn";
			}

			return Path.Combine(workDirFullPath, fileName);
		}


		private void ensureOutput(SavedInput input, bool thumbnail) {

			string workDirFullPath;
			string filePath = getFilePath(input.UrlId, thumbnail, out workDirFullPath);

			if (System.IO.File.Exists(filePath)) {
				return;
			}

			if (tryGenerateInput(input, workDirFullPath, filePath, thumbnail)) {
				malsysInputRepository.InputDb.SaveChanges();
			}

		}


		/// <remarks>
		/// Also saves information about output (mime type, metadata) to given input entity.
		/// </remarks>
		private bool tryGenerateInput(SavedInput input, bool thumbnail, IMessageLogger logger = null) {

			string workDirFullPath;
			string filePath = getFilePath(input.UrlId, thumbnail, out workDirFullPath);

			return tryGenerateInput(input, workDirFullPath, filePath, thumbnail, logger);
		}

		/// <remarks>
		/// Also saves information about output (mime type, metadata) to given input entity.
		/// </remarks>
		private bool tryGenerateInput(SavedInput input, string workDirFullPath, string outputFilePath, bool thumbnail, IMessageLogger logger = null) {

			TimeSpan timeout = new TimeSpan(0, 0, int.Parse(appSettingsProvider[AppSettingsKeys.GalleryProcessTime]));
			var fileMgr = new FileOutputProvider(workDirFullPath);

			if (logger == null) {
				logger = new MessageLogger();
			}

			string source = input.SourceCode;
			if (thumbnail && !string.IsNullOrEmpty(input.ThumbnailSourceExtension)) {
				source += "\n" + input.ThumbnailSourceExtension;
			}

			InputBlockEvaled evaledInput;
			bool result = lsystemProcessor.TryProcess(source, timeout, fileMgr, logger, out evaledInput);
			if (!result) {
				return false;
			}

			fileMgr.CloseAllOutputStreams();
			var outputs = fileMgr.GetOutputFiles().ToList();
			if (outputs.Count == 0) {
				return false;
			}

			var output = outputs.Last();
			input.MimeType = output.MimeType;
			var meta = OutputMetadataHelper.SerializeMetadata(output.Metadata);

			if (thumbnail) {
				input.OutputThnMetadata = meta;
			}
			else {
				input.OutputMetadata = meta;
				input.OutputSize = new FileInfo(output.FilePath).Length;
			}

			bool error = false;

			try {
				//System.IO.File.Delete(outputFilePath);
				System.IO.File.Copy(output.FilePath, outputFilePath, true);
			}
			catch (Exception ex) {
				Elmah.ErrorSignal.FromCurrentContext()
					.Raise(new Exception("Failed to write output file (over old one if older existed).", ex));
				error = true;
			}

			foreach (var o in outputs) {
				try {
					System.IO.File.Delete(o.FilePath);
				}
				catch (Exception ex) {
					Elmah.ErrorSignal.FromCurrentContext()
						.Raise(new Exception("Failed to delete temp gallery file.", ex));
					error = true;
				}
			}

			return !error;

		}

	}
}

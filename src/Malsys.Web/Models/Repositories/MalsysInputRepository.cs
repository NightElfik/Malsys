using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Elmah;
using Malsys.IO;
using Malsys.Processing.Output;
using Malsys.SemanticModel.Evaluated;
using Malsys.SourceCode.Printers;
using Malsys.Web.Entities;
using Malsys.Web.Models.Lsystem;

namespace Malsys.Web.Models.Repositories {
	public class MalsysInputRepository : IMalsysInputRepository {

		const int urlIdLength = 8;  // must match with DB

		private readonly IUsersDb usersDb;
		private readonly IInputDb inputDb;
		private readonly IDateTimeProvider dateTimeProvider;


		public MalsysInputRepository(IUsersDb usersDb, IInputDb inputDb, IDateTimeProvider dateTimeProvider) {

			this.usersDb = usersDb;
			this.inputDb = inputDb;
			this.dateTimeProvider = dateTimeProvider;
		}


		public IUsersDb UsersDb { get { return usersDb; } }
		public IInputDb InputDb { get { return inputDb; } }


		public InputProcess AddInputProcess(InputBlockEvaled input, int? parentId, IEnumerable<OutputFile> outputs, string userName, TimeSpan duration) {

			var user = usersDb.TryGetUserByName(userName);

			IndentStringWriter writer = new IndentStringWriter();
			CanonicPrinter printer = new CanonicPrinter(writer);
			printer.Print(input);

			var inputStr = writer.GetResult();
			var inputBytes = Encoding.UTF8.GetBytes(inputStr);

			byte[] hash;
			using (var hasher = MD5.Create()) {
				hash = hasher.ComputeHash(inputBytes);
			}

			Debug.Assert(hash.Length == 16);  // just to be sure it will fit to DB

			CanonicInput canonicInput = inputDb.CanonicInputs.Where(x => x.Hash == hash).Where(x => x.SourceCode == inputStr).SingleOrDefault();

			if (canonicInput != null) {
				var oldestProcess = canonicInput.InputProcesses.OrderBy(ci => ci.ProcessDate).FirstOrDefault();
				if (oldestProcess != null) {
					parentId = oldestProcess.InputProcessId;
				}
			}
			else {
				canonicInput = new CanonicInput() {
					Hash = hash,
					SourceCode = inputStr,
					SourceSize = inputStr.Length,
					OutputSize = outputs.Sum(o => new FileInfo(o.FilePath).Length)
				};
				inputDb.AddCanonicInput(canonicInput);
				inputDb.SaveChanges();
			}

			// make sure that parent ID is valid
			var parentEntity = inputDb.InputProcesses.SingleOrDefault(ip => ip.InputProcessId == parentId);
			if (parentEntity == null) {
				parentId = null;
			}

			DateTime now = dateTimeProvider.Now;

			var inputProcess = new InputProcess() {
				CanonicInput = canonicInput,
				ParentInputProcessId = parentId,
				User = user,
				ProcessDate = now,
				ChainLength = parentEntity == null ? 0 : parentEntity.ChainLength + 1,
				Duration = duration.Ticks
			};

			inputDb.AddInputProcess(inputProcess);
			inputDb.SaveChanges();

			foreach (var output in outputs) {
				var o = new ProcessOutput() {
					InputProcess = inputProcess,
					FileName = Path.GetFileName(output.FilePath),
					CreationDate = now,
					LastOpenDate = now - TimeSpan.FromMinutes(1),
					Metadata = OutputMetadataHelper.SerializeMetadata(output.Metadata)
				};

				inputDb.AddProcessOutput(o);
			}
			inputDb.SaveChanges();

			return inputProcess;
		}


		public SavedInput SaveInput(string source, int? parentId, IEnumerable<OutputFile> outputs, string userName, TimeSpan duration) {

			var user = usersDb.TryGetUserByName(userName);
			if (user == null) {
				return null;
			}

			string urlId = generateUniqueUrlId();
			DateTime now = dateTimeProvider.Now;

			var savedInput = new SavedInput() {
				UrlId = urlId,
				ParentInputProcessId = parentId,
				User = user,
				CreationDate = now,
				EditDate = now,
				IsPublished = false,
				IsDeleted = false,
				PublishName = null,
				Views = 0,
				SourceSize = source.Length,
				OutputSize = outputs.Sum(o => new FileInfo(o.FilePath).Length),
				Duration = duration.Ticks,
				MimeType = null,
				SourceCode = source,
				Description = null,
				OutputMetadata = null
			};
			inputDb.SaveChanges();

			return savedInput;
		}


		public void CleanProcessOutputs(string workDirFullPath, int maxFilesCount, int filesToDelete) {

			int count = inputDb.ProcessOutputs.Count();

			if (count <= maxFilesCount) {
				return;
			}


			foreach (var poToDelete in inputDb.ProcessOutputs.OrderBy(po => po.LastOpenDate).Take(filesToDelete)) {

				string filePath = Path.Combine(workDirFullPath, poToDelete.FileName);

				if (File.Exists(filePath)) {
					try {
						File.Delete(filePath);
					}
					catch (FileNotFoundException) {
						// file can be deleted by another request, we are not locking anything
					}
					catch (Exception ex) {
						ErrorSignal.FromCurrentContext().Raise(ex);
						continue;  // do not delete from DB
					}
				}

				try {
					inputDb.DeleteProcessOutput(poToDelete);
				}
				catch (Exception) {
					// DB row could be deleted by another request, we are not locking anything
					// we have to try to delete DB entry because someone could delete file in FS and not DB row
				}

			}

			inputDb.SaveChanges();

		}


		private string generateUniqueUrlId() {

			var rnd = new Random();
			var sb = new StringBuilder(urlIdLength);

			while (true) {
				sb.Clear();
				for (int i = 0; i < urlIdLength; i++) {
					var rNum = rnd.Next(10 + 26 + 26);
					if (rNum < 10) {
						sb.Append((char)('0' + rNum));
					}
					else if (rNum < 10 + 26) {
						sb.Append((char)('a' + rNum - 10));
					}
					else {
						sb.Append((char)('A' + rNum - 10 - 26));
					}
				}

				string urlId = sb.ToString();
				if (inputDb.SavedInputs.Where(x => x.UrlId == urlId).Count() == 0) {
					return urlId;  // free random ID found
				}
			}

		}


		public IEnumerable<Tag> GetTags(params string[] tags) {

			string[] tagsLower = new string[tags.Length];
			for (int i = 0; i < tags.Length; i++) {
				tagsLower[i] = tags[i].ToLower();
			}

			var tagsInDb = inputDb.Tags.Where(x => tagsLower.Contains(x.NameLowercase)).ToList();
			if (tagsInDb.Count == tags.Length) {
				return tagsInDb;
			}

			// some tags are missing
			for (int i = 0; i < tags.Length; i++) {
				if (tagsInDb.Where(x => x.NameLowercase == tagsLower[i]).FirstOrDefault() == null) {
					// tag is missing

					Tag t = new Tag() {
						Name = tags[i],
						NameLowercase = tagsLower[i]
					};

					inputDb.AddTag(t);
					tagsInDb.Add(t);
				}
			}

			inputDb.SaveChanges();

			return tagsInDb;
		}


		public bool Vote(string urlId, string userName, int rating) {

			rating = MathHelper.Clamp(rating, 0, 5);

			var input = inputDb.SavedInputs
				.Where(x => x.UrlId == urlId && !x.IsDeleted)
				.SingleOrDefault();

			if (input == null) {
				return false;
			}

			userName = userName.ToLower();
			var user = usersDb.Users
				.Where(x => x.NameLowercase == userName)
				.SingleOrDefault();

			if (user == null) {
				return false;
			}

			var vote = inputDb.Votes
				.Where(x => x.SavedInputId == input.SavedInputId && x.UserId == user.UserId)
				.SingleOrDefault();

			if (vote != null) {
				if (vote.Rating == rating) {
					return true;
				}

				input.RatingSum += rating - vote.Rating;
				vote.Rating = rating;
			}
			else {
				vote = new SavedInputVote() {
					SavedInput = input,
					User = user,
					Rating = rating
				};

				inputDb.AddVote(vote);
				input.RatingSum += rating;
				input.RatingCount++;
			}

			inputDb.SaveChanges();
			return true;
		}

		public int? GetUserVote(string urlId, string userName) {

			userName = userName.ToLower();

			var vote = inputDb.Votes
				.Where(x => x.SavedInput.UrlId == urlId && x.User.NameLowercase == userName)
				.SingleOrDefault();

			if (vote == null) {
				return null;
			}

			return vote.Rating;

		}

	}
}
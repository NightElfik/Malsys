using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Malsys.IO;
using Malsys.Processing.Output;
using Malsys.SemanticModel.Evaluated;
using Malsys.SourceCode.Printers;
using Malsys.Web.Entities;
using Elmah;

namespace Malsys.Web.Models.Repositories {
	public class MalsysInputRepository : IMalsysInputRepository {

		private readonly IUsersDb usersDb;
		private readonly IInputDb inputDb;
		private readonly IDateTimeProvider dateTimeProvider;


		public MalsysInputRepository(IUsersDb usersDb, IInputDb inputDb, IDateTimeProvider dateTimeProvider) {

			this.usersDb = usersDb;
			this.inputDb = inputDb;
			this.dateTimeProvider = dateTimeProvider;
		}


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

			Debug.Assert(hash.Length == 16);

			CanonicInput canonicInput = inputDb.CanonicInputs.Where(i => i.Hash == hash).Where(i => i.Source == inputStr).SingleOrDefault();

			if (canonicInput != null) {
				var oldestProcess = canonicInput.InputProcesses.OrderBy(ci => ci.ProcessDate).FirstOrDefault();
				if (oldestProcess != null) {
					parentId = oldestProcess.InputProcessId;
				}
			}
			else {
				canonicInput = new CanonicInput() {
					Hash = hash,
					Source = inputStr,
					SourceSize = inputStr.Length,
					OutputSize = outputs.Sum(o => new FileInfo(o.FilePath).Length)
				};
				inputDb.AddCanonicInput(canonicInput);
				inputDb.SaveChanges();
			}

			// make sure that parent ID is valid
			if (inputDb.InputProcesses.SingleOrDefault(ip => ip.InputProcessId == parentId) == null) {
				parentId = null;
			}

			DateTime now = dateTimeProvider.Now;

			var inputProcess = new InputProcess() {
				CanonicInput = canonicInput,
				ParentInputProcessId = parentId,
				User = user,
				ProcessDate = now,
				Duration = duration.Ticks
			};

			inputDb.AddInputProcess(inputProcess);
			inputDb.SaveChanges();

			foreach (var output in outputs) {
				var o = new ProcessOutput() {
					InputProcess = inputProcess,
					FileName = Path.GetFileName(output.FilePath),
					CreationDate = now,
					LastOpenDate = now
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

			const int urlIdLength = 8;

			var rnd = new Random();
			var sb = new StringBuilder(urlIdLength);
			string urlId;

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

				urlId = sb.ToString();
				if (inputDb.SavedInputs.Where(x => x.UrlId == urlId).Count() == 0) {
					break;  // free random ID found
				}
			}


			DateTime now = dateTimeProvider.Now;

			var savedInput = new SavedInput() {
				UrlId = urlId,
				ParentInputProcessId = parentId,
				User = user,
				CreationDate = now,
				EditDate = now,
				Published = false,
				Deleted = false,
				Views = 0,
				Rating = 0,
				SourceSize = source.Length,
				OutputSize = outputs.Sum(o => new FileInfo(o.FilePath).Length),
				Duration = duration.Ticks,
				Source = source,
				Description = null,
				ThumbnailSource = null
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


	}
}
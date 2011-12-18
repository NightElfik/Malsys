using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Malsys.IO;
using Malsys.Processing;
using Malsys.SemanticModel.Evaluated;
using Malsys.SourceCode.Printers;
using Malsys.Web.Entities;

namespace Malsys.Web.Models.Repositories {
	public class InputProcessesRepository : IInputProcessesRepository {

		private readonly IUsersDb usersDb;
		private readonly IInputDb inputDb;
		private readonly IDateTimeProvider dateTimeProvider;


		public InputProcessesRepository(IUsersDb usersDb, IInputDb inputDb, IDateTimeProvider dateTimeProvider) {

			this.usersDb = usersDb;
			this.inputDb = inputDb;
			this.dateTimeProvider = dateTimeProvider;
		}


		public IInputDb InputDb { get { return inputDb; } }


		public InputProcess AddInput(InputBlock input, IEnumerable<OutputFile> outputs, string userName) {

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

			if (canonicInput == null) {
				canonicInput = new CanonicInput() {
					Hash = hash,
					Source = inputStr,
					SourceSize = inputStr.Length,
					OutputSize = outputs.Sum(o => new FileInfo(o.FilePath).Length)
				};
				inputDb.AddCanonicInput(canonicInput);
				inputDb.SaveChanges();
			}

			DateTime now = dateTimeProvider.Now;

			var inputProcess = new InputProcess() {
				CanonicInput = canonicInput,
				User = user,
				ProcessDate = now
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


		public void CleanProcessOutputs(string workDirFullPath, int maxFilesCount) {

			int count = inputDb.ProcessOutputs.Count();

			if (count <= maxFilesCount) {
				return;
			}

			int toDelete = Math.Max(maxFilesCount / 4, count - maxFilesCount);

			foreach (var poToDelete in inputDb.ProcessOutputs.OrderBy(po => po.LastOpenDate).Take(toDelete)) {

				string filePath = Path.Combine(workDirFullPath, poToDelete.FileName);

				if (File.Exists(filePath)) {
					try {
						File.Delete(filePath);
					}
					catch (Exception ex) {
						// TODO: log
						continue;  // do not delete from DB
					}
				}

				inputDb.DeleteProcessOutput(poToDelete);

			}

			inputDb.SaveChanges();

		}


	}
}
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Malsys.IO;
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

		public void AddInput(InputBlock input, long outputSize, string userName) {

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
					OutputSize = outputSize
				};
				inputDb.AddCanonicInput(canonicInput);
				inputDb.SaveChanges();
			}

			var inputProcess = new InputProcess() {
				CanonicInput = canonicInput,
				User = user,
				ProcessDate = dateTimeProvider.Now
			};

			inputDb.AddInputProcess(inputProcess);
			inputDb.SaveChanges();
		}

	}
}
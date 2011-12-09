using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Malsys.Web.Entities;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Web.Models.Repositories {
	public class InputProcessesRepository : IInputProcessesRepository {

		private readonly IUsersDb usersDb;
		private readonly IInputDb inputDb;


		public InputProcessesRepository(IUsersDb usersDb, IInputDb inputDb) {

			this.usersDb = usersDb;
			this.inputDb = inputDb;
		}

		public void AddInput(InputBlock input, long outputSize, string userName) {
			throw new NotImplementedException();
		}

	}
}
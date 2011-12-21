using System.Linq;

namespace Malsys.Web.Entities {
	public partial class MalsysDb : IUsersDb, IInputDb {


		#region IUsersDb Members

		IQueryable<User> IUsersDb.Users { get { return this.Users; } }

		public void AddUser(User user) {
			this.Users.AddObject(user);
		}

		public void DeleteUser(User user) {
			this.Users.DeleteObject(user);
		}


		IQueryable<Role> IUsersDb.Roles { get { return this.Roles; } }

		public void AddRole(Role role) {
			this.Roles.AddObject(role);
		}

		public void DeleteRole(Role role) {
			this.Roles.DeleteObject(role);
		}

		#endregion


		#region IInputDb Members

		IQueryable<CanonicInput> IInputDb.CanonicInputs { get { return this.CanonicInputs; } }

		public void AddCanonicInput(CanonicInput canonicInput) {
			this.CanonicInputs.AddObject(canonicInput);
		}

		IQueryable<InputProcess> IInputDb.InputProcesses { get { return this.InputProcesses; } }

		public void AddInputProcess(InputProcess inputProcess) {
			this.InputProcesses.AddObject(inputProcess);
		}

		IQueryable<ProcessOutput> IInputDb.ProcessOutputs { get { return this.ProcessOutputs; } }

		public void AddProcessOutput(ProcessOutput processOutput) {
			this.ProcessOutputs.AddObject(processOutput);
		}

		public void DeleteProcessOutput(ProcessOutput processOutput) {
			this.ProcessOutputs.DeleteObject(processOutput);
		}

		#endregion


	}
}
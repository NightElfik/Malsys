﻿using System.Linq;

namespace Malsys.Web.Entities {
	public partial class MalsysDb : IUsersDb, IInputDb, IFeedbackDb {


		#region IUsersDb Members

		IQueryable<User> IUsersDb.Users { get { return Users; } }

		public void AddUser(User user) {
			Users.AddObject(user);
		}

		public void DeleteUser(User user) {
			Users.DeleteObject(user);
		}


		IQueryable<Role> IUsersDb.Roles { get { return Roles; } }

		public void AddRole(Role role) {
			Roles.AddObject(role);
		}

		public void DeleteRole(Role role) {
			Roles.DeleteObject(role);
		}

		#endregion


		#region IInputDb Members

		IQueryable<CanonicInput> IInputDb.CanonicInputs { get { return CanonicInputs; } }

		public void AddCanonicInput(CanonicInput canonicInput) {
			CanonicInputs.AddObject(canonicInput);
		}

		IQueryable<InputProcess> IInputDb.InputProcesses { get { return InputProcesses; } }

		public void AddInputProcess(InputProcess inputProcess) {
			InputProcesses.AddObject(inputProcess);
		}

		IQueryable<ProcessOutput> IInputDb.ProcessOutputs { get { return ProcessOutputs; } }

		public void AddProcessOutput(ProcessOutput processOutput) {
			ProcessOutputs.AddObject(processOutput);
		}

		public void DeleteProcessOutput(ProcessOutput processOutput) {
			ProcessOutputs.DeleteObject(processOutput);
		}

		#endregion


		#region IFeedbackDb Members

		IQueryable<Feedback> IFeedbackDb.Feedbacks {
			get { return Feedbacks; }
		}

		public void AddFeedback(Feedback feedback) {
			Feedbacks.AddObject(feedback);
		}

		#endregion


	}
}
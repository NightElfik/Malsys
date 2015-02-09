using System.Linq;

namespace Malsys.Web.Entities {
	public partial class MalsysDb : IUsersDb, IInputDb, IFeedbackDb, IDiscusDb, IActionLogDb {


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

		#endregion IUsersDb Members


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

		IQueryable<SavedInput> IInputDb.SavedInputs { get { return SavedInputs; } }

		public void AddSavedInput(SavedInput savedInput) {
			SavedInputs.AddObject(savedInput);
		}

		public void DeleteSavedInput(SavedInput savedInput) {
			SavedInputs.DeleteObject(savedInput);
		}

		IQueryable<Tag> IInputDb.Tags { get { return Tags; } }

		public void AddTag(Tag tag) {
			Tags.AddObject(tag);
		}

		public void DeleteTag(Tag tag) {
			Tags.DeleteObject(tag);
		}

		public IQueryable<SavedInputVote> Votes { get { return SavedInputVotes; } }

		public void AddVote(SavedInputVote vote) {
			SavedInputVotes.AddObject(vote);
		}

		#endregion IInputDb Members


		#region IFeedbackDb Members

		IQueryable<Feedback> IFeedbackDb.Feedbacks {
			get { return Feedbacks; }
		}

		public void AddFeedback(Feedback feedback) {
			Feedbacks.AddObject(feedback);
		}

		#endregion IFeedbackDb Members


		#region IDiscusDb Members

		IQueryable<DiscusCategory> IDiscusDb.DiscusCategories { get { return DiscusCategories; } }

		public void AddDiscusCategory(DiscusCategory discusCategory) {
			DiscusCategories.AddObject(discusCategory);
		}

		IQueryable<DiscusThread> IDiscusDb.DiscusThreads { get { return DiscusThreads; } }

		public void AddDiscusThread(DiscusThread discusThread) {
			DiscusThreads.AddObject(discusThread);
		}

		public void DeleteDiscusThread(DiscusThread discusThread) {
			DiscusThreads.DeleteObject(discusThread);
		}

		IQueryable<DiscusMessage> IDiscusDb.DiscusMessages { get { return DiscusMessages; } }

		public void AddDiscusMessage(DiscusMessage discusMessage) {
			DiscusMessages.AddObject(discusMessage);
		}

		public void DeleteDiscusMessage(DiscusMessage discusMessage) {
			DiscusMessages.DeleteObject(discusMessage);
		}

		#endregion IDiscusDb Members


		#region IActionLogDb Members

		IQueryable<ActionLog> IActionLogDb.ActionLogs { get { return ActionLogs; } }

		public void AddActionLog(ActionLog actionLog) {
			ActionLogs.AddObject(actionLog);
		}

		#endregion IActionLogDb Members


	}
}

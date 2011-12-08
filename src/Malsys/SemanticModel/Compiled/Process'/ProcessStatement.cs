
namespace Malsys.SemanticModel.Compiled {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class ProcessStatement : IInputStatement, ILsystemStatement {

		public readonly string TargetLsystemName;

		public readonly string ProcessConfiName;

		public readonly ImmutableList<ProcessComponentAssignment> ComponentAssignments;


		public ProcessStatement(string targetLsystemName, string processConfiName, ImmutableList<ProcessComponentAssignment> componentAssignments) {

			TargetLsystemName = targetLsystemName;
			ProcessConfiName = processConfiName;
			ComponentAssignments = componentAssignments;
		}



		InputStatementType IInputStatement.StatementType {
			get { return InputStatementType.ProcessStatement; }
		}

		LsystemStatementType ILsystemStatement.StatementType {
			get { return LsystemStatementType.ProcessStatement; }
		}

	}
}

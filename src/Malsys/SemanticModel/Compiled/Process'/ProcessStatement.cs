
namespace Malsys.SemanticModel.Compiled {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class ProcessStatement : IInputStatement, ILsystemStatement {

		public readonly string TargetLsystemName;

		public readonly string ProcessConfiName;

		public readonly ImmutableList<ProcessComponentAssignment> ComponentAssignments;


		public readonly Ast.ProcessStatement AstNode;


		public ProcessStatement(string targetLsystemName, string processConfiName, ImmutableList<ProcessComponentAssignment> componentAssignments,
				Ast.ProcessStatement astNode = null) {

			TargetLsystemName = targetLsystemName;
			ProcessConfiName = processConfiName;
			ComponentAssignments = componentAssignments;

			AstNode = astNode;
		}



		InputStatementType IInputStatement.StatementType {
			get { return InputStatementType.ProcessStatement; }
		}

		LsystemStatementType ILsystemStatement.StatementType {
			get { return LsystemStatementType.ProcessStatement; }
		}

	}
}

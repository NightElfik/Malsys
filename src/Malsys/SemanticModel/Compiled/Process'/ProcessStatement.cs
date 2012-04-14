
namespace Malsys.SemanticModel.Compiled {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class ProcessStatement : IInputStatement {

		public readonly string TargetLsystemName;

		public readonly ImmutableList<IExpression> Arguments;

		public readonly string ProcessConfiName;

		public readonly ImmutableList<ProcessComponentAssignment> ComponentAssignments;

		public readonly ImmutableList<ILsystemStatement> AdditionalLsystemStatements;


		public readonly Ast.ProcessStatement AstNode;


		public ProcessStatement(string targetLsystemName, ImmutableList<IExpression> arguments, string processConfiName,
				ImmutableList<ProcessComponentAssignment> componentAssignments, ImmutableList<ILsystemStatement> additionalLsysStats, Ast.ProcessStatement astNode = null) {

			TargetLsystemName = targetLsystemName;
			Arguments = arguments;
			ProcessConfiName = processConfiName;
			ComponentAssignments = componentAssignments;
			AdditionalLsystemStatements = additionalLsysStats;

			AstNode = astNode;
		}



		InputStatementType IInputStatement.StatementType {
			get { return InputStatementType.ProcessStatement; }
		}

	}
}

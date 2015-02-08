using System.Collections.Generic;

namespace Malsys.SemanticModel.Compiled {
	public class ProcessStatement : IInputStatement {

		public string TargetLsystemName;
		public List<IExpression> Arguments;

		public string ProcessConfiName;
		public List<ProcessComponentAssignment> ComponentAssignments;
		public List<ILsystemStatement> AdditionalLsystemStatements;

		public readonly Ast.ProcessStatement AstNode;


		public ProcessStatement(Ast.ProcessStatement astNode) {
			AstNode = astNode;
		}



		InputStatementType IInputStatement.StatementType {
			get { return InputStatementType.ProcessStatement; }
		}

	}
}

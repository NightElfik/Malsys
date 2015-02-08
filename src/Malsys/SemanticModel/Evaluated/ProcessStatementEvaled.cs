using System.Collections.Generic;
using Malsys.SemanticModel.Compiled;

namespace Malsys.SemanticModel.Evaluated {
	public class ProcessStatementEvaled {

		public string TargetLsystemName;
		public List<IValue> Arguments;
		public string ProcessConfigName;
		public List<ProcessComponentAssignment> ComponentAssignments;
		public List<ILsystemStatement> AdditionalLsystemStatements;

		public readonly Ast.ProcessStatement AstNode;


		public ProcessStatementEvaled(Ast.ProcessStatement astNode) {
			AstNode = astNode;
		}

	}
}

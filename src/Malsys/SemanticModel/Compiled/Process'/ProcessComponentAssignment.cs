// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.

namespace Malsys.SemanticModel.Compiled {
	public class ProcessComponentAssignment {

		public string ComponentTypeName;
		public string ContainerName;

		public readonly Ast.ProcessComponentAssignment AstNode;


		public ProcessComponentAssignment(Ast.ProcessComponentAssignment astNode) {
			AstNode = astNode;
		}
	}
}

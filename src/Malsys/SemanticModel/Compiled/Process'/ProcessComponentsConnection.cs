// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.

namespace Malsys.SemanticModel.Compiled {
	public class ProcessComponentsConnection {

		public bool IsVirtual;
		public string SourceName;
		public string TargetName;
		public string TargetInputName;

		public readonly Ast.ProcessConfigConnection AstNode;


		public ProcessComponentsConnection(Ast.ProcessConfigConnection astNode) {
			AstNode = astNode;
		}

	}
}

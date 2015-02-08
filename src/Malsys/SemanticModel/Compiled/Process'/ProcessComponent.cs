﻿
namespace Malsys.SemanticModel.Compiled {
	public class ProcessComponent {

		public string Name;
		public string TypeName;

		public readonly Ast.ProcessComponent AstNode;


		public ProcessComponent(Ast.ProcessComponent astNode) {
			AstNode = astNode;
		}

	}
}

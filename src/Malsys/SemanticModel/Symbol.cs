using System.Collections.Generic;

namespace Malsys.SemanticModel {
	public class Symbol<T> {

		public string Name;
		public List<T> Arguments;

		public readonly Ast.IAstNode AstNode;


		public Symbol(Ast.IAstNode astNode) {
			AstNode = astNode;
		}
	}
}

using System.Collections.Generic;

namespace Malsys.SemanticModel.Compiled {
	public class RewriteRuleReplacement {
		
		public List<Symbol<IExpression>> Replacement;
		public IExpression Weight;

		public readonly Ast.RewriteRuleReplacement AstNode;


		public RewriteRuleReplacement(Ast.RewriteRuleReplacement astNode) {
			AstNode = astNode;
		}

	}
}

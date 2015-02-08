using System.Collections.Generic;

namespace Malsys.SemanticModel.Compiled.Expressions {
	public class FunctionCall : IExpression {

		public string Name;
		public List<IExpression> Arguments;

		public Ast.IExpressionMember AstNode;


		public FunctionCall(Ast.IExpressionMember astNode) {
			AstNode = astNode;
		}


		public ExpressionType ExpressionType { get { return ExpressionType.FunctionCall; } }

	}
}

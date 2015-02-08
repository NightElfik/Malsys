using System.Collections.Generic;

namespace Malsys.SemanticModel.Compiled.Expressions {
	public class ExpressionValuesArray : List<IExpression>, IExpression {

		public readonly Ast.ExpressionsArray AstNode;


		public ExpressionValuesArray(Ast.ExpressionsArray astNode) {
			AstNode = astNode;
		}

		public ExpressionValuesArray(IEnumerable<IExpression> values, Ast.ExpressionsArray astNode)
			: base(values) {

			AstNode = astNode;
		}


		public ExpressionType ExpressionType { get { return ExpressionType.ExpressionValuesArray; } }

	}
}

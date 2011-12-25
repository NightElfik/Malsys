﻿
namespace Malsys.SemanticModel.Compiled.Expressions {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class ExpressionValuesArray : ImmutableList<IExpression>, IExpression {

		public readonly Ast.ExpressionsArray AstNode;


		public ExpressionValuesArray(Ast.ExpressionsArray astNode)
			: base(ImmutableList<IExpression>.Empty) {

			AstNode = astNode;
		}

		public ExpressionValuesArray(ImmutableList<IExpression> values, Ast.ExpressionsArray astNode)
			: base(values) {

			AstNode = astNode;
		}



		public bool IsEmptyExpression { get { return false; } }


		public void Accept(IExpressionVisitor visitor) {
			visitor.Visit(this);
		}

	}
}

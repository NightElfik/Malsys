/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */

namespace Malsys.SemanticModel.Compiled.Expressions {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class FunctionCall : IExpression {


		public readonly string Name;

		public readonly ImmutableList<IExpression> Arguments;

		public readonly Ast.IExpressionMember AstNode;


		public FunctionCall(string name, ImmutableList<IExpression> args, Ast.IExpressionMember astNode) {

			Name = name;
			Arguments = args;

			AstNode = astNode;
		}


		public ExpressionType ExpressionType { get { return ExpressionType.FunctionCall; } }

	}
}

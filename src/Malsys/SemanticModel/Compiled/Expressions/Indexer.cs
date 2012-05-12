/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */

namespace Malsys.SemanticModel.Compiled.Expressions {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class Indexer : IExpression {


		public readonly IExpression Array;

		public readonly IExpression Index;

		public readonly Ast.ExpressionIndexer AstNode;


		public Indexer(IExpression array, IExpression index, Ast.ExpressionIndexer astNode) {

			Array = array;
			Index = index;

			AstNode = astNode;
		}


		public ExpressionType ExpressionType { get { return ExpressionType.Indexer; } }

	}
}

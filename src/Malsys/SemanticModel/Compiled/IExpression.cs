/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */

namespace Malsys.SemanticModel.Compiled {

	public interface IExpression {

		ExpressionType ExpressionType { get; }

	}


	public enum ExpressionType {

		BinaryOperator,
		Constant,
		EmptyExpression,
		ExpressionValuesArray,
		ExprVariable,
		FunctionCall,
		Indexer,
		UnaryOperator,

	}

}

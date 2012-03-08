using Malsys.SemanticModel.Compiled.Expressions;

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

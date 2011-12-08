using Malsys.SemanticModel.Compiled.Expressions;

namespace Malsys.SemanticModel.Compiled {

	public interface IExpression : IExpressionVisitable {

		bool IsEmptyExpression { get; }

	}


	public interface IExpressionVisitable {

		void Accept(IExpressionVisitor visitor);

	}


	public interface IExpressionVisitor {

		void Visit(BinaryOperator binaryOperator);
		void Visit(Constant constant);
		void Visit(EmptyExpression emptyExpression);
		void Visit(ExpressionValuesArray expressionValuesArray);
		void Visit(ExprVariable variable);
		void Visit(FunctionCall functionCall);
		void Visit(Indexer indexer);
		void Visit(UnaryOperator unaryOperator);
		void Visit(UserFunctionCall userFunction);

	}

}


namespace Malsys.Expressions {
	public interface IExpressionVisitor {
		void Visit(Constant constant);
		void Visit(ExprVariable variable);
		void Visit(ExpressionValuesArray expressionValuesArray);
		void Visit(UnaryOperator unaryOperator);
		void Visit(BinaryOperator binaryOperator);
		void Visit(Indexer indexer);
		void Visit(FunctionCall functionCall);
		void Visit(UserFunctionCall userFunction);
	}
}

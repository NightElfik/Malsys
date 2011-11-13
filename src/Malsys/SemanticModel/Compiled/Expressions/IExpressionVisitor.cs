
namespace Malsys.SemanticModel.Compiled.Expressions {
	public interface IExpressionVisitor {

		void Visit(BinaryOperator binaryOperator);
		void Visit(Constant constant);
		void Visit(ExpressionValuesArray expressionValuesArray);
		void Visit(ExprVariable variable);
		void Visit(FunctionCall functionCall);
		void Visit(Indexer indexer);
		void Visit(UnaryOperator unaryOperator);
		void Visit(UserFunctionCall userFunction);

	}
}

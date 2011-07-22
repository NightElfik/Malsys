
namespace Malsys.Expressions {
	public interface IExpressionVisitor {
		void Visit(Constant constant);
		void Visit(Variable variable);
		void Visit(ExpressionValuesArray expressionValuesArray);
		void Visit(UnaryOperator unaryOperator);
		void Visit(BinaryOperator binaryOperator);
		void Visit(Indexer indexer);
		void Visit(Function function);
		void Visit(UserFunction userFunction);
	}
}

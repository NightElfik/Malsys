
namespace Malsys.Ast {
	public interface IAstVisitor {
		// alphabetically sorted
		void Visit(EmptyStatement emptyStatement);
		void Visit(Expression expression);
		void Visit(ExpressionBracketed expressionBracketed);
		void Visit(ExpressionFunction expressionFunction);
		void Visit(ExpressionIndexer expressionIndexer);
		void Visit(FloatConstant floatConstant);
		void Visit(FunctionDefinition functionDefinition);
		void Visit(Identificator identificator);
		void Visit(Keyword keyword);
		void Visit(Lsystem lsystem);
		void Visit(Operator op);
		void Visit(OptionalParameter optionalParameter);
		void Visit(RewriteRule rewriteRule);
		void Visit(RichExpression richExpression);
		void Visit(Symbol symbol);
		void Visit(SymbolPattern symbolPattern);
		void Visit(SymbolExprArgs symbolWithParams);
		void Visit(ExpressionsArray valuesArray);
		void Visit(VariableDefinition variableDefinition);
	}
}

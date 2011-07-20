
namespace Malsys.Ast {
	public interface IAstVisitor {
		// alphabetically sorted
		void Visit(Expression expression);
		void Visit(ExpressionBracketed expressionBracketed);
		void Visit(ExpressionFunction expressionFunction);
		void Visit(ExpressionIndexer expressionIndexer);
		void Visit(FloatConstant floatConstant);
		void Visit(Identificator identificator);
		void Visit(InputFile inputFile);
		void Visit(Keyword keyword);
		void Visit(Lsystem lsystem);
		void Visit(Operator op);
		void Visit(RewriteRule rewriteRule);
		void Visit(RrCondition condition);
		void Visit(RrContext context);
		void Visit(RrProbability probability);
		void Visit(Symbol symbol);
		void Visit(SymbolPattern symbolPattern);
		void Visit(SymbolWithParams symbolWithParams);
		void Visit(VariableDefinition variableDefinition);
	}
}

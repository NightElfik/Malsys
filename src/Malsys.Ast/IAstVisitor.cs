
namespace Malsys.Ast {
	public interface IAstVisitor {
		void Visit(InputFile inputFile);
		void Visit(Lsystem lsystem);
		void Visit(VariableDefinition variableDefinition);
		void Visit(Keyword keyword);
		void Visit(Identificator identificator);
		void Visit(RewriteRule rewriteRule);
		void Visit(RrContext context);
		void Visit(RrCondition condition);
		void Visit(RrProbability probability);
		void Visit(Symbol symbol);
		void Visit(SymbolPattern symbolPattern);
		void Visit(SymbolWithParams symbolWithParams);
		void Visit(Expression expression);
		void Visit(FloatConstant floatConstant);
		void Visit(ExpressionFunction floatConstant);
		
	}
}

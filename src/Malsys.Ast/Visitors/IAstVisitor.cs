using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Malsys.Ast {
	public interface IAstVisitor {

		// Visit methods sorted by type name.

		void Visit(Comment comment);
		void Visit(EmptyStatement emptyStat);
		void Visit(Expression expr);
		void Visit(ExpressionBracketed bracketedExpr);
		void Visit(ExpressionFunction funExpr);
		void Visit(ExpressionIndexer indexerExpr);
		void Visit(ExpressionsArray arrExpr);
		void Visit(FloatConstant floatConstant);
		void Visit(FunctionDefinition funDef);
		void Visit(Identificator id);
		void Visit<T>(ImmutableListPos<T> tokList) where T : IToken;
		void Visit(InvalidExpression invExpr);
		void Visit(KeywordPos keyword);
		void Visit(Lsystem lsystem);
		void Visit(Operator op);
		void Visit(OptionalParameter optParam);
		void Visit(RewriteRule rewriteRule);
		void Visit(RewriteRuleReplacement rrReplacment);
		void Visit(RichExpression richExpr);
		void Visit<T>(Symbol<T> symbol) where T : IToken;
		void Visit(SymbolsDefinition symbolDef);
		void Visit(VariableDefinition variableDef);

	}
}

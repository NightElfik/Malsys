
namespace Malsys.Ast {
	public interface IAstExpressionVisitor {

		void Visit(ExpressionBracketed bracketedExpr);
		void Visit(ExpressionFunction funExpr);
		void Visit(ExpressionIndexer indexerExpr);
		void Visit(ExpressionsArray arrExpr);
		void Visit(FloatConstant floatConstant);
		void Visit(Identificator variable);
		void Visit(Operator optor);

	}
}

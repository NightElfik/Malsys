
namespace Malsys.Ast {

	/// <summary>
	/// All expression members should be immutable.
	/// </summary>
	public interface IExpressionMember : IToken, IExpressionVisitable {

	}


	public interface IExpressionVisitable {

		void Accept(IExpressionVisitor visitor);

	}


	public interface IExpressionVisitor {

		void Visit(ExpressionBracketed bracketedExpr);
		void Visit(ExpressionFunction funExpr);
		void Visit(ExpressionIndexer indexerExpr);
		void Visit(ExpressionsArray arrExpr);
		void Visit(FloatConstant floatConstant);
		void Visit(Identificator variable);
		void Visit(Operator optor);

	}

}

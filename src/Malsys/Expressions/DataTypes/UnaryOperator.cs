
namespace Malsys.Expressions {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class UnaryOperator : IExpression, IExpressionVisitable {

		public readonly string Syntax;

		public readonly int Precedence;
		public readonly int ActivePrecedence;

		public readonly IExpression Operand;
		public readonly ExpressionValueType OperandType;

		public readonly EvalDelegate Evaluate;


		public UnaryOperator(string syntax, int prec, int activePrec, EvalDelegate evalFunc,
				IExpression operand, ExpressionValueType operandType) {

			Syntax = syntax;
			Precedence = prec;
			ActivePrecedence = activePrec;
			Evaluate = evalFunc;

			Operand = operand;
			OperandType = operandType;
		}

		#region IExpressionVisitable Members

		public void Accept(IExpressionVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}

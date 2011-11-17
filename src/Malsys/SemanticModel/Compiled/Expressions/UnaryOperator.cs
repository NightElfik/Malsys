using System;
using Malsys.SemanticModel.Evaluated;
using Malsys.Evaluators;

namespace Malsys.SemanticModel.Compiled.Expressions {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class UnaryOperator : IExpression, IExpressionVisitable {

		public readonly string Syntax;

		public readonly int Precedence;
		public readonly int ActivePrecedence;

		public readonly IExpression Operand;
		public readonly ExpressionValueType OperandType;

		public readonly Func<ArgsStorage, IValue> Evaluate;


		public UnaryOperator(string syntax, int prec, int activePrec, Func<ArgsStorage, IValue> evalFunc,
				IExpression operand, ExpressionValueType operandType) {

			Syntax = syntax;
			Precedence = prec;
			ActivePrecedence = activePrec;
			Evaluate = evalFunc;

			Operand = operand;
			OperandType = operandType;
		}


		#region IExpression Members

		public bool IsEmptyExpression { get { return false; } }

		#endregion

		#region IExpressionVisitable Members

		public void Accept(IExpressionVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}

using System;
using Malsys.Evaluators;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.SemanticModel.Compiled.Expressions {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class BinaryOperator : IExpression, IExpressionVisitable {

		public readonly string Syntax;

		public readonly int Precedence;
		public readonly int ActivePrecedence;

		public readonly IExpression LeftOperand, RightOperand;
		public readonly ExpressionValueType LeftOperandType, RightOperandType;

		public readonly Func<ArgsStorage, IValue> Evaluate;


		public BinaryOperator(string syntax, int prec, int activePrec, Func<ArgsStorage, IValue> evalFunc,
				IExpression leftOperand, ExpressionValueType leftOperandType, IExpression rightOperand, ExpressionValueType rightOperandType) {

			Syntax = syntax;
			Evaluate = evalFunc;
			Precedence = prec;
			ActivePrecedence = activePrec;

			LeftOperand = leftOperand;
			LeftOperandType = leftOperandType;

			RightOperand = rightOperand;
			RightOperandType = rightOperandType;
		}

		#region IExpressionVisitable Members

		public void Accept(IExpressionVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}

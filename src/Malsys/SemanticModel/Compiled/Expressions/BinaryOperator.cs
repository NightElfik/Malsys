using System;
using Malsys.Evaluators;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.SemanticModel.Compiled.Expressions {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class BinaryOperator : IExpression {

		public readonly string Syntax;

		public readonly int Precedence;
		public readonly int ActivePrecedence;

		public readonly IExpression LeftOperand, RightOperand;
		public readonly ExpressionValueType LeftOperandType, RightOperandType;

		public readonly Func<ArgsStorage, IValue> Evaluate;

		public readonly Ast.Operator AstNode;


		public BinaryOperator(string syntax, int prec, int activePrec, Func<ArgsStorage, IValue> evalFunc, IExpression leftOperand,
				ExpressionValueType leftOperandType, IExpression rightOperand, ExpressionValueType rightOperandType, Ast.Operator astNode) {

			Syntax = syntax;
			Evaluate = evalFunc;
			Precedence = prec;
			ActivePrecedence = activePrec;

			LeftOperand = leftOperand;
			LeftOperandType = leftOperandType;

			RightOperand = rightOperand;
			RightOperandType = rightOperandType;

			AstNode = astNode;
		}



		public bool IsEmptyExpression { get { return false; } }


		public void Accept(IExpressionVisitor visitor) {
			visitor.Visit(this);
		}

	}
}

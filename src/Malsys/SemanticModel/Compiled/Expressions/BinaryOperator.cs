using System;
using Malsys.Evaluators;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.SemanticModel.Compiled.Expressions {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class BinaryOperator : IExpression {

		public readonly string Syntax;

		public readonly int Precedence;
		public readonly int ActivePrecedence;

		public readonly IExpression LeftOperand, RightOperand;
		public readonly ExpressionValueTypeFlags LeftOperandType, RightOperandType;

		public readonly Func<IValue, IValue, IValue> Evaluate;

		public readonly Ast.Operator AstNode;


		public BinaryOperator(string syntax, int prec, int activePrec, Func<IValue, IValue, IValue> evalFunc, IExpression leftOperand,
				ExpressionValueTypeFlags leftOperandType, IExpression rightOperand, ExpressionValueTypeFlags rightOperandType, Ast.Operator astNode) {

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


		public ExpressionType ExpressionType { get { return ExpressionType.BinaryOperator; } }

	}
}

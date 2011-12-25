using System;
using Malsys.Evaluators;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.SemanticModel.Compiled.Expressions {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class UnaryOperator : IExpression {

		public readonly string Syntax;

		public readonly int Precedence;
		public readonly int ActivePrecedence;

		public readonly IExpression Operand;
		public readonly ExpressionValueType OperandType;

		public readonly Func<ArgsStorage, IValue> Evaluate;

		public readonly Ast.Operator AstNode;


		public UnaryOperator(string syntax, int prec, int activePrec, Func<ArgsStorage, IValue> evalFunc,
				IExpression operand, ExpressionValueType operandType, Ast.Operator astNode) {

			Syntax = syntax;
			Precedence = prec;
			ActivePrecedence = activePrec;
			Evaluate = evalFunc;

			Operand = operand;
			OperandType = operandType;

			AstNode = astNode;
		}



		public bool IsEmptyExpression { get { return false; } }


		public void Accept(IExpressionVisitor visitor) {
			visitor.Visit(this);
		}

	}
}

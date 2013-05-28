// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.SemanticModel.Compiled.Expressions {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class UnaryOperator : IExpression {

		public readonly string Syntax;

		public readonly int Precedence;
		public readonly int ActivePrecedence;

		public readonly IExpression Operand;
		public readonly ExpressionValueTypeFlags OperandType;

		public readonly Func<IValue, IValue> Evaluate;

		public readonly Ast.Operator AstNode;


		public UnaryOperator(string syntax, int prec, int activePrec, Func<IValue, IValue> evalFunc,
				IExpression operand, ExpressionValueTypeFlags operandType, Ast.Operator astNode) {

			Syntax = syntax;
			Precedence = prec;
			ActivePrecedence = activePrec;
			Evaluate = evalFunc;

			Operand = operand;
			OperandType = operandType;

			AstNode = astNode;
		}


		public ExpressionType ExpressionType { get { return ExpressionType.UnaryOperator; } }

	}
}

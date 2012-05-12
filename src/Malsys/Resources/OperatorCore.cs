/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System;
using System.Diagnostics.Contracts;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Resources {
	/// <summary>
	/// Core of unary or binary operator.
	/// </summary>
	/// <remarks>
	/// Nearly immutable (only documentation strings can be set later).
	/// </remarks>
	public class OperatorCore {

		public readonly string Syntax;

		/// <summary>
		/// True for unary operators, False for binary operators.
		/// </summary>
		public readonly bool IsUnary;

		/// <summary>
		/// Normal precedence.
		/// </summary>
		/// <remarks>
		/// If precedence and active precedence are the same, operator is right associative.
		/// If active precedence is higher, operator is left associative.
		/// </remarks>
		public readonly int Precedence;

		/// <summary>
		/// Precedence while I am holding this operator and deciding weather I push it on stack or pop some others before push.
		/// </summary>
		/// <remarks>
		/// Thx for the idea to Martin Mareš :).
		/// </remarks>
		public readonly int ActivePrecedence;

		/// <summary>
		/// Type of left operand of unary operator or left operand of binary operator.
		/// </summary>
		public readonly ExpressionValueTypeFlags FirstParamType;

		/// <summary>
		/// Type of right operand of binary operator.
		/// Not used for unary operators.
		/// </summary>
		public readonly ExpressionValueTypeFlags SecondParamType;

		/// <summary>
		/// Unary operator function body.
		/// Only used for unary operators (null for binary).
		/// </summary>
		public readonly Func<IValue, IValue> UnaryEvalFunction;

		/// <summary>
		/// Binary operator function body.
		/// Only used for binary operators (null for unary).
		/// </summary>
		public readonly Func<IValue, IValue, IValue> BinaryEvalFunction;


		public string SummaryDoc { get; private set; }


		/// <summary>
		/// Creates instance of operator core for unary operator.
		/// </summary>
		public OperatorCore(string syntax, int prec, int activePrec, ExpressionValueTypeFlags paramType, Func<IValue, IValue> evalFunc) {

			Contract.Requires<ArgumentNullException>(syntax != null);
			Contract.Requires<ArgumentNullException>(paramType != ExpressionValueTypeFlags.Unknown);
			Contract.Requires<ArgumentNullException>(evalFunc != null);

			IsUnary = true;
			Syntax = syntax;
			Precedence = prec;
			ActivePrecedence = activePrec;
			UnaryEvalFunction = evalFunc;
			BinaryEvalFunction = null;
			FirstParamType = paramType;
			SecondParamType = ExpressionValueTypeFlags.Unknown;
		}

		/// <summary>
		/// Creates instance of operator core for binary operator.
		/// </summary>
		public OperatorCore(string syntax, int prec, int activePrec, ExpressionValueTypeFlags leftParamType,
				ExpressionValueTypeFlags rightParamType, Func<IValue, IValue, IValue> evalFunc) {

			Contract.Requires<ArgumentNullException>(syntax != null);
			Contract.Requires<ArgumentNullException>(leftParamType != ExpressionValueTypeFlags.Unknown);
			Contract.Requires<ArgumentNullException>(rightParamType != ExpressionValueTypeFlags.Unknown);
			Contract.Requires<ArgumentNullException>(evalFunc != null);

			IsUnary = false;
			Syntax = syntax;
			Precedence = prec;
			ActivePrecedence = activePrec;
			UnaryEvalFunction = null;
			BinaryEvalFunction = evalFunc;
			FirstParamType = leftParamType;
			SecondParamType = rightParamType;
		}


		public void SetDocumentation(string summaryDoc) {
			SummaryDoc = summaryDoc;
		}

	}
}

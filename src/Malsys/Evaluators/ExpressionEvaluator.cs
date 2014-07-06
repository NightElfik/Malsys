// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System;
using System.Diagnostics;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Compiled.Expressions;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Evaluators {
	/// <remarks>
	/// All public members are thread safe.
	/// Since this class has no state there is no reason of creating instances.
	/// </remarks>
	public class ExpressionEvaluator : IExpressionEvaluator {


		public static readonly ExpressionEvaluator Instance = new ExpressionEvaluator();


		/// <summary>
		/// Evaluates given expression.
		/// </summary>
		public IValue Evaluate(IExpression expr, IExpressionEvaluatorContext exprMemberProvider) {

			// individual cases in switch are not separate methods to lower call stack and enhance performance
			switch (expr.ExpressionType) {

				case ExpressionType.BinaryOperator:
					var binOp = (BinaryOperator)expr;
					var left = Evaluate(binOp.LeftOperand, exprMemberProvider);
					if (!left.Type.IsCompatibleWith(binOp.LeftOperandType)) {
						throw new EvalException("Failed to evaluate binary operator `{0}`. As left operand excepted {1}, but {2} was given."
							.Fmt(binOp.Syntax, binOp.LeftOperandType.ToTypeString(), left.Type.ToTypeString()));
					}

					var right = Evaluate(binOp.RightOperand, exprMemberProvider);
					if (!right.Type.IsCompatibleWith(binOp.LeftOperandType)) {
						throw new EvalException("Failed to evaluate binary operator `{0}`. As right operand excepted {1}, but {2} was given."
							.Fmt(binOp.Syntax, binOp.LeftOperandType.ToTypeString(), right.Type.ToTypeString()));
					}

					return binOp.Evaluate(left, right);

				case ExpressionType.Constant:
					return (Constant)expr;

				case ExpressionType.EmptyExpression:
					return Constant.NaN;

				case ExpressionType.ExpressionValuesArray:
					var exprValArr = (ExpressionValuesArray)expr;
					var valArr = new IValue[exprValArr.Count];

					for (int i = 0; i < valArr.Length; i++) {
						valArr[i] = Evaluate(exprValArr[i], exprMemberProvider);
					}

					return new ValuesArray(new ImmutableList<IValue>(valArr, true), exprValArr.AstNode);

				case ExpressionType.ExprVariable:
					var exprVar = (ExprVariable)expr;
					IValue varValue;
					if (!exprMemberProvider.TryGetVariableValue(exprVar.Name, out varValue)) {
						throw new EvalException("Unknown variable `{0}`.".Fmt(exprVar.Name));
					}
					return varValue;

				case ExpressionType.FunctionCall:
					var funCall = (FunctionCall)expr;

					#region Extra evaluation of IF function

					if (funCall.Name.Equals("if", StringComparison.CurrentCultureIgnoreCase) && funCall.Arguments.Count == 3) {
						var condition = Evaluate(funCall.Arguments[0], exprMemberProvider);
						if (!condition.IsConstant) {
							throw new EvalException("Failed to evaluate function `{0}`. As first argument excepted {1} but {2} was given."
								.Fmt(funCall.Name, (ExpressionValueType.Constant).ToTypeString(), condition.Type.ToTypeString()));
						}

						IExpression arg;

						if (((Constant)condition).IsTrue) {
							arg = funCall.Arguments[1];
						}
						else {
							arg = funCall.Arguments[2];
						}

						return Evaluate(arg, exprMemberProvider);
					}

					#endregion Extra evaluation of IF function

					// evaluate arguments
					var funCallArgs = funCall.Arguments;
					var funArgs = new IValue[funCallArgs.Count];
					for (int i = 0; i < funArgs.Length; i++) {
						funArgs[i] = Evaluate(funCallArgs[i], exprMemberProvider);
					}

					IValue funValue;
					if (!exprMemberProvider.TryEvaluateFunction(funCall.Name, funArgs, out funValue)) {
						throw new EvalException("Unknown function `{0}`.".Fmt(funCall.Name));
					}
					return funValue;

				case ExpressionType.Indexer:
					var indexer = (Indexer)expr;
					var indexOperand = Evaluate(indexer.Array, exprMemberProvider);
					if (!indexOperand.IsArray) {
						throw new EvalException("Failed to evaluate indexer. As operand excepted {0}, but {1} was given."
							.Fmt(ExpressionValueType.Array.ToTypeString(), indexOperand.Type.ToTypeString()));
					}

					ValuesArray indexArr = (ValuesArray)indexOperand;

					var indexIndex = Evaluate(indexer.Index, exprMemberProvider);
					if (!indexIndex.IsConstant) {
						throw new EvalException("Failed to evaluate indexer. As index excepted {0}, but {1} was given."
							.Fmt(ExpressionValueType.Constant.ToTypeString(), indexIndex.Type.ToTypeString()));
					}

					Constant index = (Constant)indexIndex;
					if (index.IsNaN) {
						throw new EvalException("Failed to evaluate indexer. Index is NaN.");
					}

					int intIndex = index.RoundedIntValue;

					if (intIndex < 0) {
						throw new EvalException("Failed to evaluate indexer, index out of range. Index is zero-based but negative value `{0}` was given."
							.Fmt(intIndex));
					}

					if (intIndex >= indexArr.Length) {
						throw new EvalException("Failed to evaluate indexer, index out of range. Can not index array of length {0} with zero-based index {1}."
							.Fmt(indexArr.Length, intIndex));
					}

					return indexArr[intIndex];

				case ExpressionType.UnaryOperator:
					var unOp = (UnaryOperator)expr;
					var operand = Evaluate(unOp.Operand, exprMemberProvider);

					if (!operand.Type.IsCompatibleWith(unOp.OperandType)) {
						throw new EvalException("Failed to evaluate unary operator `{0}`. As operand excepted {1}, but {2} was given."
							.Fmt(unOp.Syntax, unOp.OperandType.ToTypeString(), operand.Type.ToTypeString()));
					}

					return unOp.Evaluate(operand);

				default:
					Debug.Fail("Unknown expression member type `{0}`.".Fmt(expr.ExpressionType.ToString()));
					return Constant.NaN;
			}

		}

	}
}

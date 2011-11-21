﻿using System.Collections.Generic;
using Malsys.Ast;
using Malsys.Compilers.Expressions;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Compiled.Expressions;
using OperatorCorePos = System.Tuple<Malsys.Compilers.Expressions.OperatorCore, Malsys.Position>;

namespace Malsys.Compilers {
	public partial class ExpressionCompiler {

		class ExpressionCompilerVisitor : Ast.IExpressionVisitor {

			public static readonly IExpression ErrorResult = Constant.NaN;


			private ExpressionCompiler parent;

			private State state;
			private Stack<OperatorCorePos> optorsStack;
			private Stack<IExpression> operandsStack;


			public ExpressionCompilerVisitor(ExpressionCompiler exprComp) {

				parent = exprComp;

				optorsStack = new Stack<OperatorCorePos>();
				operandsStack = new Stack<IExpression>();
			}


			public IExpression Compile(Expression expr) {

				if (expr.IsEmpty) {
					return EmptyExpression.Instance;
				}

				init();

				foreach (var member in expr) {
					member.Accept(this);

					if (state == State.Error) {
						return ErrorResult;
					}
				}

				if (state != State.ExcpectingOperator) {
					parent.logMessage(ExpressionCompilerMessageType.UnexcpectedEndOfExpression, expr.Position);
					return ErrorResult;
				}

				// pop all remaining operators
				while (optorsStack.Count > 0) {
					if (!tryPopOperator()) {
						parent.logMessage(ExpressionCompilerMessageType.TooFewOperands, expr.Position);
						return ErrorResult;
					}
				}

				if (operandsStack.Count != 1) {
					parent.logMessage(ExpressionCompilerMessageType.TooManyOperands, expr.Position);
					return ErrorResult;
				}

				return operandsStack.Pop();
			}

			#region IExpressionVisitor Members

			public void Visit(ExpressionBracketed bracketedExpr) {

				switch (state) {
					case State.ExcpectingOperand:
						operandsStack.Push(parent.CompileExpression(bracketedExpr.Expression));
						state = State.ExcpectingOperator;
						break;

					case State.ExcpectingOperator:
						// Expression (probably in parenthesis) while excpecting binary operator?
						// So directly before it is operand.
						// Lets add implicit multiplication between them.
						operandsStack.Push(parent.CompileExpression(bracketedExpr.Expression));
						// implicit multiplication
						optorsStack.Push(new OperatorCorePos(OperatorCore.Multiply, bracketedExpr.Position.GetBeginPos()));
						state = State.ExcpectingOperator;
						break;

					default:
						badState(bracketedExpr);
						break;
				}
			}

			public void Visit(ExpressionFunction funExpr) {

				switch (state) {
					case State.ExcpectingOperand:
						compileFunctionCall(funExpr);
						state = State.ExcpectingOperator;
						break;

					case State.ExcpectingOperator:
						// Function while excpecting binary operator?
						// So directly before function is operand.
						// Lets add implicit multiplication between them.
						compileFunctionCall(funExpr);
						optorsStack.Push(new OperatorCorePos(OperatorCore.Multiply, funExpr.Position.GetBeginPos()));
						state = State.ExcpectingOperator;
						break;

					default:
						badState(funExpr);
						break;
				}

			}

			public void Visit(ExpressionIndexer indexerExpr) {

				switch (state) {
					case State.ExcpectingOperand:
						parent.logMessage(ExpressionCompilerMessageType.UnexcpectedOperator, indexerExpr.Position, "indexer");
						state = State.Error;
						break;

					case State.ExcpectingOperator:
						if (operandsStack.Count < 1) {
							parent.logMessage(ExpressionCompilerMessageType.TooFewOperands, indexerExpr.Position);
							state = State.Error;
							break;
						}

						// apply indexer on previous operand
						var indexExpr = parent.CompileExpression(indexerExpr.Index);
						operandsStack.Push(new Indexer(operandsStack.Pop(), indexExpr));
						state = State.ExcpectingOperator;
						break;

					default:
						badState(indexerExpr);
						break;
				}
			}

			public void Visit(ExpressionsArray arrExpr) {

				switch (state) {
					case State.ExcpectingOperand:
						var arr = parent.CompileList(arrExpr);
						operandsStack.Push(new ExpressionValuesArray(arr));
						state = State.ExcpectingOperator;
						break;

					case State.ExcpectingOperator:
						parent.logMessage(ExpressionCompilerMessageType.UnexcpectedOperand, arrExpr.Position, "array");
						state = State.Error;
						break;

					default:
						badState(arrExpr);
						break;
				}
			}

			public void Visit(FloatConstant floatConstant) {

				switch (state) {
					case State.ExcpectingOperand:
						operandsStack.Push(new Constant(floatConstant.Value, floatConstant));
						state = State.ExcpectingOperator;
						break;

					case State.ExcpectingOperator:
						parent.logMessage(ExpressionCompilerMessageType.UnexcpectedOperand, floatConstant.Position, floatConstant.ToString());
						state = State.Error;
						break;

					default:
						badState(floatConstant);
						break;
				}
			}

			public void Visit(Identificator variable) {

				switch (state) {
					case State.ExcpectingOperand:
						KnownConstant cnst;
						if (parent.KnownConstants.TryGet(variable.Name, out cnst)) {
							// known constant
							operandsStack.Push(new Constant(cnst.Value));
						}
						else {
							// variable
							operandsStack.Push(new ExprVariable(variable.Name));
						}

						state = State.ExcpectingOperator;
						break;

					case State.ExcpectingOperator:
						parent.logMessage(ExpressionCompilerMessageType.UnexcpectedOperand, variable.Position, variable.Name);
						state = State.Error;
						break;

					default:
						badState(variable);
						break;
				}
			}

			public void Visit(Operator optor) {

				switch (state) {
					case State.ExcpectingOperand:
						// operator while excpecting operand? It must be unary!
						OperatorCore opUnary;
						if (parent.KnownOperators.TryGet(optor.Syntax, 1, out opUnary)) {
							state = tryPushOperator(new OperatorCorePos(opUnary, optor.Position)) ? State.ExcpectingOperand : State.Error;
						}
						else {
							parent.logMessage(ExpressionCompilerMessageType.UnknownUnaryOperator, optor.Position, optor.Syntax);
							state = State.Error;
						}
						break;

					case State.ExcpectingOperator:
						// operator must be binary
						OperatorCore opBinary;
						if (parent.KnownOperators.TryGet(optor.Syntax, 2, out opBinary)) {
							state = tryPushOperator(new OperatorCorePos(opBinary, optor.Position)) ? State.ExcpectingOperand : State.Error;
						}
						else {
							parent.logMessage(ExpressionCompilerMessageType.UnknownBinaryOperator, optor.Position, optor.Syntax);
							state = State.Error;
						}
						break;

					default:
						badState(optor);
						break;
				}
			}

			#endregion


			private void init() {
				optorsStack.Clear();
				operandsStack.Clear();

				state = State.ExcpectingOperand;
			}

			private void badState(IExpressionMember member) {
				parent.logMessage(ExpressionCompilerMessageType.InternalError, member.Position, "Unknown compiler state `{0}`.".Fmt(state));
				state = State.Error;
			}

			private void compileFunctionCall(ExpressionFunction funCall) {

				var rsltArgs = parent.CompileList(funCall.Arguments);

				FunctionCore knownFun;
				if (parent.KnownFunctions.TryGet(funCall.NameId.Name, funCall.Arguments.Length, out knownFun)) {
					operandsStack.Push(new FunctionCall(funCall.NameId.Name, knownFun.EvalFunction, rsltArgs, knownFun.ParamsTypes));
				}
				else {
					operandsStack.Push(new UserFunctionCall(funCall.NameId.Name, rsltArgs));
				}
			}


			/// <summary>
			/// Pushes given operator on operators stack. All operators with lower precedence than given operator's
			/// active precedence are popped end evaluated before the push.
			/// </summary>
			/// <returns>Returns false if there was not enough operands on stack to evaluate popped operators.</returns>
			private bool tryPushOperator(OperatorCorePos op) {

				while (optorsStack.Count > 0 && op.Item1.ActivePrecedence > optorsStack.Peek().Item1.Precedence) {
					if (!tryPopOperator()) {
						// error alredy reported by tryPopOperator()
						return false;
					}
				}

				optorsStack.Push(op);
				return true;
			}

			/// <summary>
			/// Pops an operator from top of the operators stack and creates new expression from it using some operands
			/// from operands stack. Result is saved back on operands stack.
			/// </summary>
			/// <remarks>
			/// Do not check enough operators in operators stack.
			/// Reports appropriate error when false is returned.
			/// </remarks>
			/// <returns>Returns false operation failed (i.e. if there was not enough operands on stack or internal error).</returns>
			private bool tryPopOperator() {

				var opTopPos = optorsStack.Pop();
				var opTop = opTopPos.Item1;

				if (opTop.Arity == 1) {
					if (operandsStack.Count < 1) {
						parent.logMessage(ExpressionCompilerMessageType.TooFewOperands, opTopPos.Item2);
						return false;
					}

					operandsStack.Push(new UnaryOperator(opTop.Syntax, opTop.Precedence, opTop.ActivePrecedence,
						opTop.EvalFunction, operandsStack.Pop(), opTop.ParamsTypes[0]));
				}

				else if (opTop.Arity == 2) {
					if (operandsStack.Count < 2) {
						parent.logMessage(ExpressionCompilerMessageType.TooFewOperands, opTopPos.Item2);
						return false;
					}
					var right = operandsStack.Pop();
					var left = operandsStack.Pop();

					operandsStack.Push(new BinaryOperator(opTop.Syntax, opTop.Precedence, opTop.ActivePrecedence,
						opTop.EvalFunction, left, opTop.ParamsTypes[0], right, opTop.ParamsTypes[1]));
				}

				else {
					parent.logMessage(ExpressionCompilerMessageType.InternalError, opTopPos.Item2, "Operator with unexcpected arity {0}.".Fmt(opTop.Arity));
					return false;
				}

				return true;
			}


			private enum State {
				Error,
				ExcpectingOperand,
				ExcpectingOperator,
			}
		}
	}
}

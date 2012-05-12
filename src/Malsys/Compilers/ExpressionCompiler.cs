/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System.Collections.Generic;
using Malsys.Resources;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Compiled.Expressions;
using OperatorCoreAst = System.Tuple<Malsys.Resources.OperatorCore, Malsys.Ast.Operator>;

namespace Malsys.Compilers {
	/// <remarks>
	/// All public members are thread safe if supplied constants and operators providers are thread safe for reading.
	///
	/// Compiler does implicit multiplication in these cases:
	/// <list type="bullet">
	///	<item><description>number followed by variable: 5 x => 5*x</description></item>
	///	<item><description>variable followed by variable: x y => x*y</description></item>
	///	<item><description>number followed by function call: 5 log(2) => 5*log(2)</description></item>
	///	<item><description>number followed by bracketed expression: 5 (1+2) => 5*(1+2)</description></item>
	/// </list>
	///
	/// Compiling process can be enhanced removing calls to compile sub-expressions (fun arguments, indexer, ...).
	/// The removal can be done inserting some special stop-operator on stack and evaluate sub-expression
	/// in current stacks. At the end just pop all operators till stop-operator found. Remove it and continue.
	/// Easy to implement but I do not have time for it.
	/// </remarks>
	public class ExpressionCompiler : IExpressionCompiler {

		public static readonly IExpression ErrorResult = Constant.NaN;

		protected readonly ICompilerConstantsProvider knownConstants;
		protected readonly IOperatorsProvider knownOperators;


		public ExpressionCompiler(ICompilerConstantsProvider constants, IOperatorsProvider operators) {
			knownConstants = constants;
			knownOperators = operators;
		}


		public IExpression Compile(Ast.Expression expr, IMessageLogger logger) {

			if (expr.IsEmpty) {
				return EmptyExpression.Instance;
			}

			var operandsStack = new Stack<IExpression>();
			var optorsStack = new Stack<OperatorCoreAst>();

			bool state = true;
			const bool expectingOperand = true;
			const bool expectingOperator = false;

			Ast.IExpressionMember previousMember = Ast.EmptyExpression.Instance;

			foreach (var member in expr) {

				// individual cases in switch are not separate methods to lower call stack and enhance performance
				switch (member.MemberType) {

					case Ast.ExpressionMemberType.EmptyExpression:
						break;

					case Ast.ExpressionMemberType.ExpressionBracketed:
						var bracketedExpr = (Ast.ExpressionBracketed)member;
						if (state == expectingOperand) {
							operandsStack.Push(Compile(bracketedExpr.Expression, logger));
							state = expectingOperator;
						}
						else {
							if (previousMember is Ast.FloatConstant) {
								operandsStack.Push(Compile(bracketedExpr.Expression, logger));
								// implicit multiplication in situation like: 5 (...) => 5*(...)
								optorsStack.Push(new OperatorCoreAst(StdOperators.Multiply, new Ast.Operator(null, bracketedExpr.Position.GetBeginPos())));
							}
							else {
								logger.LogMessage(Message.UnexcpectedOperand, bracketedExpr.Position, "bracketed expression");
								return ErrorResult;
							}
						}
						break;

					case Ast.ExpressionMemberType.ExpressionFunction:
						var funExpr = (Ast.ExpressionFunction)member;

						if (state == expectingOperand) {
							operandsStack.Push(new FunctionCall(funExpr.NameId.Name, this.CompileList(funExpr.Arguments, logger), funExpr));
							state = expectingOperator;
						}
						else {
							if (previousMember is Ast.FloatConstant) {
								operandsStack.Push(new FunctionCall(funExpr.NameId.Name, this.CompileList(funExpr.Arguments, logger), funExpr));
								// implicit multiplication in situation like: 5 f(...) => 5*f(...)
								optorsStack.Push(new OperatorCoreAst(StdOperators.Multiply, new Ast.Operator(null, funExpr.Position.GetBeginPos())));
								// still expecting operator
							}
							else {
								logger.LogMessage(Message.UnexcpectedOperand, funExpr.Position, "function");
								return ErrorResult;
							}
						}
						break;

					case Ast.ExpressionMemberType.ExpressionIndexer:
						var indexerExpr = (Ast.ExpressionIndexer)member;
						if (state == expectingOperand) {
							logger.LogMessage(Message.UnexcpectedOperator, indexerExpr.Position, "indexer");
							return ErrorResult;
						}
						else {
							if (operandsStack.Count < 1) {
								logger.LogMessage(Message.TooFewOperands, indexerExpr.Position);
								return ErrorResult;
							}

							// apply indexer on previous operand
							var indexExpr = Compile(indexerExpr.Index, logger);
							operandsStack.Push(new Indexer(operandsStack.Pop(), indexExpr, indexerExpr));
							// still expecting operator
						}
						break;

					case Ast.ExpressionMemberType.ExpressionsArray:
						var arrExpr = (Ast.ExpressionsArray)member;
						if (state == expectingOperand) {
							var arr = this.CompileList(arrExpr, logger);
							operandsStack.Push(new ExpressionValuesArray(arr, arrExpr));
							state = expectingOperator;
						}
						else {
							logger.LogMessage(Message.UnexcpectedOperand, arrExpr.Position, "array");
							return ErrorResult;
						}
						break;

					case Ast.ExpressionMemberType.FloatConstant:
						var floatConstant = (Ast.FloatConstant)member;
						if (state == expectingOperand) {
							operandsStack.Push(new Constant(floatConstant.Value, floatConstant));
							state = expectingOperator;
						}
						else {
							logger.LogMessage(Message.UnexcpectedOperand, floatConstant.Position, floatConstant.ToString());
							return ErrorResult;
						}
						break;

					case Ast.ExpressionMemberType.Identificator:
						var variable = (Ast.Identificator)member;
						compileVariable(variable, operandsStack);

						if (state == expectingOperator) {
							if (previousMember is Ast.FloatConstant || previousMember is Ast.Identificator) {
								// implicit multiplication in situation like: 5 x => 5*x  or  x y => x*y
								optorsStack.Push(new OperatorCoreAst(StdOperators.Multiply, new Ast.Operator(null, variable.Position.GetBeginPos())));
							}
							else {
								logger.LogMessage(Message.UnexcpectedOperand, variable.Position, variable.Name);
								return ErrorResult;
							}
						}
						state = expectingOperator;
						break;

					case Ast.ExpressionMemberType.Operator:
						var optor = (Ast.Operator)member;
						if (state == expectingOperand) {
							// operator while expecting operand? It must be unary!
							OperatorCore opUnary;
							if (knownOperators.TryGet(optor.Syntax, true, out opUnary)) {
								if (!tryPushOperator(new OperatorCoreAst(opUnary, optor), operandsStack, optorsStack, logger)) {
									return ErrorResult;
								}
							}
							else {
								logger.LogMessage(Message.UnknownUnaryOperator, optor.Position, optor.Syntax);
								return ErrorResult;
							}
						}
						else {
							// operator must be binary
							OperatorCore opBinary;
							if (knownOperators.TryGet(optor.Syntax, false, out opBinary)) {
								if (!tryPushOperator(new OperatorCoreAst(opBinary, optor), operandsStack, optorsStack, logger)) {
									return ErrorResult;
								}
							}
							else {
								logger.LogMessage(Message.UnknownBinaryOperator, optor.Position, optor.Syntax);
								return ErrorResult;
							}
						}
						state = expectingOperand;
						break;

					default:
						break;
				}
				previousMember = member;
			}

			if (state != expectingOperator) {
				logger.LogMessage(Message.UnexcpectedEndOfExpression, expr.Position);
				return ErrorResult;
			}

			// pop all remaining operators
			while (optorsStack.Count > 0) {
				if (!tryPopOperator(operandsStack, optorsStack, logger)) {
					logger.LogMessage(Message.TooFewOperands, expr.Position);
					return ErrorResult;
				}
			}

			if (operandsStack.Count != 1) {
				logger.LogMessage(Message.TooManyOperands, expr.Position);
				return ErrorResult;
			}

			return operandsStack.Pop();

		}



		private void compileVariable(Ast.Identificator variable, Stack<IExpression> operandsStack) {

			CompilerConstant cnst;
			if (knownConstants.TryGetConstant(variable.Name, out cnst)) {
				// known constant
				operandsStack.Push(new Constant(cnst.Value, new Ast.FloatConstant(cnst.Value, Ast.ConstantFormat.Float, variable.Position)));
			}
			else {
				// variable
				operandsStack.Push(new ExprVariable(variable.Name, variable));
			}
		}

		/// <summary>
		/// Pushes given operator on operators stack. All operators with lower precedence than given operator's
		/// active precedence are popped end evaluated before the push.
		/// </summary>
		/// <returns>Returns false if there was not enough operands on stack to evaluate popped operators.</returns>
		private bool tryPushOperator(OperatorCoreAst op, Stack<IExpression> operandsStack, Stack<OperatorCoreAst> optorsStack, IMessageLogger logger) {

			while (optorsStack.Count > 0 && op.Item1.ActivePrecedence > optorsStack.Peek().Item1.Precedence) {
				if (!tryPopOperator(operandsStack, optorsStack, logger)) {
					// error already reported by tryPopOperator()
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
		private bool tryPopOperator(Stack<IExpression> operandsStack, Stack<OperatorCoreAst> optorsStack, IMessageLogger logger) {

			var opTopAst = optorsStack.Pop();
			var opTop = opTopAst.Item1;

			if (opTop.IsUnary) {
				if (operandsStack.Count < 1) {
					logger.LogMessage(Message.TooFewOperands, opTopAst.Item2.Position);
					return false;
				}

				operandsStack.Push(new UnaryOperator(opTop.Syntax, opTop.Precedence, opTop.ActivePrecedence,
					opTop.UnaryEvalFunction, operandsStack.Pop(), opTop.FirstParamType, opTopAst.Item2));
			}
			else {
				if (operandsStack.Count < 2) {
					logger.LogMessage(Message.TooFewOperands, opTopAst.Item2.Position);
					return false;
				}
				var right = operandsStack.Pop();
				var left = operandsStack.Pop();

				operandsStack.Push(new BinaryOperator(opTop.Syntax, opTop.Precedence, opTop.ActivePrecedence,
					opTop.BinaryEvalFunction, left, opTop.FirstParamType, right, opTop.SecondParamType, opTopAst.Item2));
			}

			return true;
		}

	}


	public enum Message {

		[Message(MessageType.Error, "Expression compiler internal error. {0}")]
		InternalError,
		[Message(MessageType.Error, "Unexpected end of expression.")]
		UnexcpectedEndOfExpression,
		[Message(MessageType.Error, "Unexpected operand `{0}`, expecting binary operator.")]
		UnexcpectedOperand,
		[Message(MessageType.Error, "Unexpected `{0}` operator, expecting operand.")]
		UnexcpectedOperator,
		[Message(MessageType.Error, "Too few operands in expression.")]
		TooFewOperands,
		[Message(MessageType.Error, "Too many operands in expression.")]
		TooManyOperands,
		[Message(MessageType.Error, "Unknown unary operator `{0}`.")]
		UnknownUnaryOperator,
		[Message(MessageType.Error, "Unknown binary operator `{0}`.")]
		UnknownBinaryOperator,

	}
}

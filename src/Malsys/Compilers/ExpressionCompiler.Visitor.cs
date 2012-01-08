using System.Collections.Generic;
//using Malsys.Ast;
using Malsys.Compilers.Expressions;
using Malsys.Resources;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Compiled.Expressions;
using OperatorCoreAst = System.Tuple<Malsys.Compilers.Expressions.OperatorCore, Malsys.Ast.Operator>;

namespace Malsys.Compilers {
	internal partial class ExpressionCompiler {

		private class Visitor : Ast.IExpressionVisitor {

			public static readonly IExpression ErrorResult = Constant.NaN;

			private IMessageLogger logger;
			private ExpressionCompiler parentExprCompiler;

			private State state;
			private Stack<OperatorCoreAst> optorsStack;
			private Stack<IExpression> operandsStack;

			private Ast.IExpressionMember previousMember;


			public Visitor(ExpressionCompiler parentExpressionCompiler, IMessageLogger logger) {

				this.logger = logger;
				parentExprCompiler = parentExpressionCompiler;

				optorsStack = new Stack<OperatorCoreAst>();
				operandsStack = new Stack<IExpression>();
			}


			public IExpression Compile(Ast.Expression expr) {

				if (expr.IsEmpty) {
					return EmptyExpression.Instance;
				}

				init();

				foreach (var member in expr) {
					member.Accept(this);

					if (state == State.Error) {
						return ErrorResult;
					}

					previousMember = member;
				}

				if (state != State.ExcpectingOperator) {
					logger.LogMessage(Message.UnexcpectedEndOfExpression, expr.Position);
					return ErrorResult;
				}

				// pop all remaining operators
				while (optorsStack.Count > 0) {
					if (!tryPopOperator()) {
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


			#region IExpressionVisitor Members

			public void Visit(Ast.EmptyExpression emptyExpr) {

			}

			public void Visit(Ast.ExpressionBracketed bracketedExpr) {

				switch (state) {
					case State.ExcpectingOperand:
						operandsStack.Push(parentExprCompiler.Compile(bracketedExpr.Expression, logger));
						state = State.ExcpectingOperator;
						break;

					case State.ExcpectingOperator:
						if (previousMember is Ast.FloatConstant) {
							// implicit multiplication
							operandsStack.Push(parentExprCompiler.Compile(bracketedExpr.Expression, logger));
							optorsStack.Push(new OperatorCoreAst(StdOperators.Multiply, new Ast.Operator(null, bracketedExpr.Position.GetBeginPos())));
							//state = State.ExcpectingOperator;
						}
						else {
							logger.LogMessage(Message.UnexcpectedOperand, bracketedExpr.Position, "bracketed expression");
							state = State.Error;
						}
						break;

					default:
						badState(bracketedExpr);
						break;
				}
			}

			public void Visit(Ast.ExpressionFunction funExpr) {

				switch (state) {
					case State.ExcpectingOperand:
						compileFunctionCall(funExpr);
						state = State.ExcpectingOperator;
						break;

					case State.ExcpectingOperator:
						if (previousMember is Ast.FloatConstant) {
							// implicit multiplication
							compileFunctionCall(funExpr);
							optorsStack.Push(new OperatorCoreAst(StdOperators.Multiply, new Ast.Operator(null, funExpr.Position.GetBeginPos())));
							// state = State.ExcpectingOperator;
						}
						else {
							logger.LogMessage(Message.UnexcpectedOperand, funExpr.Position, "function");
							state = State.Error;
						}
						break;

					default:
						badState(funExpr);
						break;
				}

			}

			public void Visit(Ast.ExpressionIndexer indexerExpr) {

				switch (state) {
					case State.ExcpectingOperand:
						logger.LogMessage(Message.UnexcpectedOperator, indexerExpr.Position, "indexer");
						state = State.Error;
						break;

					case State.ExcpectingOperator:
						if (operandsStack.Count < 1) {
							logger.LogMessage(Message.TooFewOperands, indexerExpr.Position);
							state = State.Error;
							break;
						}

						// apply indexer on previous operand
						var indexExpr = parentExprCompiler.Compile(indexerExpr.Index, logger);
						operandsStack.Push(new Indexer(operandsStack.Pop(), indexExpr, indexerExpr));
						state = State.ExcpectingOperator;
						break;

					default:
						badState(indexerExpr);
						break;
				}
			}

			public void Visit(Ast.ExpressionsArray arrExpr) {

				switch (state) {
					case State.ExcpectingOperand:
						var arr = parentExprCompiler.CompileList(arrExpr, logger);
						operandsStack.Push(new ExpressionValuesArray(arr, arrExpr));
						state = State.ExcpectingOperator;
						break;

					case State.ExcpectingOperator:
						logger.LogMessage(Message.UnexcpectedOperand, arrExpr.Position, "array");
						state = State.Error;
						break;

					default:
						badState(arrExpr);
						break;
				}
			}

			public void Visit(Ast.FloatConstant floatConstant) {

				switch (state) {
					case State.ExcpectingOperand:
						operandsStack.Push(new Constant(floatConstant.Value, floatConstant));
						state = State.ExcpectingOperator;
						break;

					case State.ExcpectingOperator:
						logger.LogMessage(Message.UnexcpectedOperand, floatConstant.Position, floatConstant.ToString());
						state = State.Error;
						break;

					default:
						badState(floatConstant);
						break;
				}
			}

			public void Visit(Ast.Identificator variable) {

				switch (state) {
					case State.ExcpectingOperand:
						compileVariable(variable);
						state = State.ExcpectingOperator;
						break;

					case State.ExcpectingOperator:
						if (previousMember is Ast.FloatConstant) {
							// implicit multiplication
							compileVariable(variable);
							optorsStack.Push(new OperatorCoreAst(StdOperators.Multiply, new Ast.Operator(null, variable.Position.GetBeginPos())));
							// state = State.ExcpectingOperator;
						}
						else {
							logger.LogMessage(Message.UnexcpectedOperand, variable.Position, variable.Name);
							state = State.Error;
						}
						break;

					default:
						badState(variable);
						break;
				}
			}

			public void Visit(Ast.Operator optor) {

				switch (state) {
					case State.ExcpectingOperand:
						// operator while expecting operand? It must be unary!
						OperatorCore opUnary;
						if (parentExprCompiler.knownOperators.TryGet(optor.Syntax, 1, out opUnary)) {
							state = tryPushOperator(new OperatorCoreAst(opUnary, optor)) ? State.ExcpectingOperand : State.Error;
						}
						else {
							logger.LogMessage(Message.UnknownUnaryOperator, optor.Position, optor.Syntax);
							state = State.Error;
						}
						break;

					case State.ExcpectingOperator:
						// operator must be binary
						OperatorCore opBinary;
						if (parentExprCompiler.knownOperators.TryGet(optor.Syntax, 2, out opBinary)) {
							state = tryPushOperator(new OperatorCoreAst(opBinary, optor)) ? State.ExcpectingOperand : State.Error;
						}
						else {
							logger.LogMessage(Message.UnknownBinaryOperator, optor.Position, optor.Syntax);
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

				previousMember = Ast.EmptyExpression.Instance;

				state = State.ExcpectingOperand;
			}

			private void badState(Ast.IExpressionMember member) {
				logger.LogMessage(Message.InternalError, member.Position, "Unknown compiler state `{0}`.".Fmt(state));
				state = State.Error;
			}

			private void compileVariable(Ast.Identificator variable) {

				KnownConstant cnst;
				if (parentExprCompiler.knownConstants.TryGet(variable.Name, out cnst)) {
					// known constant
					operandsStack.Push(new Constant(cnst.Value, new Ast.FloatConstant(cnst.Value, Ast.ConstantFormat.Float, variable.Position)));
				}
				else {
					// variable
					operandsStack.Push(new ExprVariable(variable.Name, variable));
				}
			}

			private void compileFunctionCall(Ast.ExpressionFunction funCall) {

				var rsltArgs = parentExprCompiler.CompileList(funCall.Arguments, logger);

				FunctionCore knownFun;
				if (parentExprCompiler.knownFunctions.TryGet(funCall.NameId.Name, funCall.Arguments.Length, out knownFun)) {
					operandsStack.Push(new FunctionCall(funCall.NameId.Name, knownFun.EvalFunction, rsltArgs, knownFun.ParamsTypes, funCall));
				}
				else {
					operandsStack.Push(new UserFunctionCall(funCall.NameId.Name, rsltArgs, funCall));
				}
			}


			/// <summary>
			/// Pushes given operator on operators stack. All operators with lower precedence than given operator's
			/// active precedence are popped end evaluated before the push.
			/// </summary>
			/// <returns>Returns false if there was not enough operands on stack to evaluate popped operators.</returns>
			private bool tryPushOperator(OperatorCoreAst op) {

				while (optorsStack.Count > 0 && op.Item1.ActivePrecedence > optorsStack.Peek().Item1.Precedence) {
					if (!tryPopOperator()) {
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
			private bool tryPopOperator() {

				var opTopAst = optorsStack.Pop();
				var opTop = opTopAst.Item1;

				if (opTop.Arity == 1) {
					if (operandsStack.Count < 1) {
						logger.LogMessage(Message.TooFewOperands, opTopAst.Item2.Position);
						return false;
					}

					operandsStack.Push(new UnaryOperator(opTop.Syntax, opTop.Precedence, opTop.ActivePrecedence,
						opTop.EvalFunction, operandsStack.Pop(), opTop.ParamsTypes[0], opTopAst.Item2));
				}

				else if (opTop.Arity == 2) {
					if (operandsStack.Count < 2) {
						logger.LogMessage(Message.TooFewOperands, opTopAst.Item2.Position);
						return false;
					}
					var right = operandsStack.Pop();
					var left = operandsStack.Pop();

					operandsStack.Push(new BinaryOperator(opTop.Syntax, opTop.Precedence, opTop.ActivePrecedence,
						opTop.EvalFunction, left, opTop.ParamsTypes[0], right, opTop.ParamsTypes[1], opTopAst.Item2));
				}

				else {
					logger.LogMessage(Message.InternalError, opTopAst.Item2.Position, "Operator with unexpected arity {0}.".Fmt(opTop.Arity));
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

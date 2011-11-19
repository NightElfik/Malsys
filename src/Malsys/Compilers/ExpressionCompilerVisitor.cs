using System.Collections.Generic;
using System.Diagnostics;
using Malsys.Ast;
using Malsys.Compilers.Expressions;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Compiled.Expressions;

namespace Malsys.Compilers {
	class ExpressionCompilerVisitor : Ast.IExpressionVisitor {

		public static readonly IExpression ErrorResult = Constant.NaN;


		private MessagesCollection msgs;
		private ExpressionCompiler exprCompiler;

		private State state;
		private Stack<OperatorCore> optorsStack;
		private Stack<IExpression> operandsStack;


		public ExpressionCompilerVisitor(ExpressionCompiler exprComp, MessagesCollection msgsColl) {

			exprCompiler = exprComp;
			msgs = msgsColl;

			optorsStack = new Stack<OperatorCore>();
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
				msgs.AddError("Unexcpected end of expression.", expr.Position);
				return ErrorResult;
			}

			// pop all remaining operators
			while (optorsStack.Count > 0) {
				if (!tryPopOperator()) {
					msgs.AddError("Too few operands in expression.", expr.Position);
					return ErrorResult;
				}
			}

			if (operandsStack.Count != 1) {
				msgs.AddError("Too many operands in expression.", expr.Position);
				return ErrorResult;
			}

			return operandsStack.Pop();
		}

		#region IExpressionVisitor Members

		public void Visit(ExpressionBracketed bracketedExpr) {

			switch (state) {
				case State.ExcpectingOperand:
					operandsStack.Push(exprCompiler.CompileExpression(bracketedExpr.Expression));
					state = State.ExcpectingOperator;
					break;

				case State.ExcpectingOperator:
					// Expression (probably in parenthesis) while excpecting binary operator?
					// So directly before it is operand.
					// Lets add implicit multiplication between them.
					operandsStack.Push(exprCompiler.CompileExpression(bracketedExpr.Expression));
					// implicit multiplication
					optorsStack.Push(OperatorCore.Multiply);
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
					optorsStack.Push(OperatorCore.Multiply);
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
					msgs.AddError("Unexcpected indexer, excpecting operand.", indexerExpr.Position);
					state = State.Error;
					break;

				case State.ExcpectingOperator:
					if (operandsStack.Count < 1) {
						msgs.AddError("Failed to compile indexer. No operand to apply on.", indexerExpr.Position);
						state = State.Error;
						break;
					}

					// apply indexer on previous operand
					var indexExpr = exprCompiler.CompileExpression(indexerExpr.Index);
					operandsStack.Push(new Indexer(operandsStack.Pop(), operandsStack.Pop()));
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
					var arr = exprCompiler.CompileList(arrExpr);
					operandsStack.Push(new ExpressionValuesArray(arr));
					state = State.ExcpectingOperator;
					break;

				case State.ExcpectingOperator:
					msgs.AddError("Unexcpected array, excpecting operator.", arrExpr.Position);
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
					msgs.AddError("Unexcpected constatnt `{0}`, excpecting operator.".Fmt(floatConstant.Value), floatConstant.Position);
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
					if (exprCompiler.KnownConstants.TryGet(variable.Name, out cnst)) {
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
					msgs.AddError("Unexcpected variable `{0}`, excpecting operator.".Fmt(variable.Name), variable.Position);
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
					if (exprCompiler.KnownOperators.TryGet(optor.Syntax, 1, out opUnary)) {
						if (tryPushOperator(opUnary)) {
							state = State.ExcpectingOperand;
						}
						else {
							msgs.AddError("Too few operands.", optor.Position);
							state = State.Error;
						}
					}
					else {
						msgs.AddError("Unknown unary operator `{1}`.".Fmt(optor.Syntax), optor.Position);
						state = State.Error;
					}
					break;

				case State.ExcpectingOperator:
					// operator must be binary
					OperatorCore opBinary;
					if (exprCompiler.KnownOperators.TryGet(optor.Syntax, 2, out opBinary)) {
						if (tryPushOperator(opBinary)) {
							state = State.ExcpectingOperand;
						}
						else {
							msgs.AddError("Too few operands in expression.", optor.Position);
							state = State.Error;
						}
					}
					else {
						msgs.AddError("Unknown binary operator `{0}`.".Fmt(optor.Syntax), optor.Position);
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
			Debug.Fail("Expression compiler internal error.");
			msgs.AddError("Expression compiler internal error.", member.Position);
			state = State.Error;
		}

		private void compileFunctionCall(ExpressionFunction funCall) {

			var rsltArgs = exprCompiler.CompileList(funCall.Arguments);

			FunctionCore knownFun;
			if (exprCompiler.KnownFunctions.TryGet(funCall.NameId.Name, funCall.Arguments.Length, out knownFun)) {
				operandsStack.Push(new FunctionCall(funCall.NameId.Name, knownFun.EvalFunction, rsltArgs, knownFun.ParamsTypes));
			}
			else {
				operandsStack.Push(new UserFunctionCall(funCall.NameId.Name, rsltArgs));
			}
		}

		private bool tryPushOperator(OperatorCore op) {

			while (optorsStack.Count > 0 && op.ActivePrecedence > optorsStack.Peek().Precedence) {
				if (!tryPopOperator()) {
					return false;
				}
			}

			optorsStack.Push(op);
			return true;
		}

		/// <summary>
		/// Pops operator from top of operators stack and creates expression from it and some operands from operands stack.
		/// Result is saved back on operands stack.
		/// Do not check operators stack.
		/// </summary>
		private bool tryPopOperator() {

			var opTop = optorsStack.Pop();
			if (opTop.Arity > operandsStack.Count) {
				return false;
			}

			if (opTop.Arity == 1) {
				operandsStack.Push(new UnaryOperator(opTop.Syntax, opTop.Precedence, opTop.ActivePrecedence,
					opTop.EvalFunction, operandsStack.Pop(), opTop.ParamsTypes[0]));
			}
			else {
				Debug.Assert(opTop.Arity == 2, "Excpected binary operator, but given op has arity {0}.".Fmt(opTop.Arity));

				var right = operandsStack.Pop();
				var left = operandsStack.Pop();

				operandsStack.Push(new BinaryOperator(opTop.Syntax, opTop.Precedence, opTop.ActivePrecedence,
					opTop.EvalFunction, left, opTop.ParamsTypes[0], right, opTop.ParamsTypes[1]));

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

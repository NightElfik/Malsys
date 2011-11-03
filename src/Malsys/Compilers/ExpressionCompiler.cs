using System.Collections.Generic;
using System.Diagnostics;
using Malsys.Ast;
using Malsys.Expressions;

namespace Malsys.Compilers {
	public class ExpressionCompiler {

		public static readonly IExpression ErrorResult = Constant.NaN;


		private MessagesCollection msgs;


		public ExpressionCompiler(MessagesCollection msgsColl) {
			msgs = msgsColl;
		}


		/// <summary>
		/// Compiles expression from AST. If compilation failes an error report is added to messages collection and
		/// ErrorResult constant is returned.
		/// </summary>
		public IExpression CompileExpression(Expression expr) {

			if (expr.IsEmpty) {
				return ErrorResult;
			}

			Stack<OperatorCore> optorsStack = new Stack<OperatorCore>();
			Stack<IExpression> operandsStack = new Stack<IExpression>();

			State state = State.ExcpectingOperand;

			foreach (var member in expr) {
				switch (state) {
					case State.ExcpectingOperand:
						state = handleAsOperand(member, optorsStack, operandsStack);
						break;

					case State.ExcpectingOperator:
						state = handleAsOperator(member, optorsStack, operandsStack);
						break;

					case State.Error:
						return ErrorResult;

					default:
						Debug.Fail("Unknown expression compiler state `{0}`.".Fmt(state));
						msgs.AddError("Expression compiler internal error.", member.Position);
						return ErrorResult;
				}
			}

			// last member could end in error state
			if (state == State.Error) {
				return ErrorResult;
			}

			if (state != State.ExcpectingOperator) {
				msgs.AddError("Unexcpected end of expression.", expr.Position);
				return ErrorResult;
			}

			// pop all remaining operators
			while (optorsStack.Count > 0) {
				if (!tryPopOperator(operandsStack, optorsStack)) {
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

		/// <summary>
		/// Compiles list of expressions from AST using fail-safe compilation on each member.
		/// </summary>
		public ImmutableList<IExpression> CompileFailSafe(ImmutableList<Expression> exprs) {

			var compiledExprs = new IExpression[exprs.Length];

			for (int i = 0; i < exprs.Length; i++) {
				compiledExprs[i] = CompileExpression(exprs[i]);
			}

			return new ImmutableList<IExpression>(compiledExprs, true);
		}


		private State handleAsOperand(IExpressionMember member, Stack<OperatorCore> optorsStack, Stack<IExpression> operandsStack) {

			switch (member.MemberType) {
				case ExpressionMemberType.Constant:
					var fc = (FloatConstant)member;
					operandsStack.Push(fc.Value.ToConst(fc));
					return State.ExcpectingOperator;

				case ExpressionMemberType.Variable:
					Identificator id = (Identificator)member;
					KnownConstant cnst;

					if (KnownConstant.TryGet(id.Name, out cnst)) {
						// known constant
						operandsStack.Push(cnst.Value.ToConst());
					}
					else {
						// variable
						operandsStack.Push(id.Name.ToVar());
					}

					return State.ExcpectingOperator;

				case ExpressionMemberType.Array:

					var exprArr = CompileFailSafe((ExpressionsArray)member);
					operandsStack.Push(new ExpressionValuesArray(exprArr));

					return State.ExcpectingOperator;

				case ExpressionMemberType.Operator:
					// operator while excpecting operand? It must be unary!
					OperatorCore op;

					if (OperatorCore.TryGet(((Operator)member).Syntax, 1, out op)) {
						if (!tryPushOperator(op, operandsStack, optorsStack)) {
							msgs.AddError("Too few operands.", member.Position);
							return State.Error;
						}
					}
					else {
						msgs.AddError("Unknown unary operator `{1}`.".Fmt(((Operator)member).Syntax), member.Position);
						return State.Error;
					}

					return State.ExcpectingOperand;

				case ExpressionMemberType.Indexer:
					msgs.AddError("Unexcpected indexer, excpecting operand.", member.Position);
					return State.Error;

				case ExpressionMemberType.Function:

					var funCall = compileFunctionCallFailSafe((ExpressionFunction)member);
					operandsStack.Push(funCall);

					return State.ExcpectingOperator;

				case ExpressionMemberType.BracketedExpression:

					var expr = CompileExpression(((ExpressionBracketed)member).Expression);
					operandsStack.Push(expr);

					return State.ExcpectingOperator;

				default:
					Debug.Fail("Unhandled type {0} in {1}.".Fmt(member.GetType().Name, typeof(Ast.Expression).Name));
					msgs.AddError("Expression compiler internal error.", member.Position);
					return State.Error;
			}
		}

		private State handleAsOperator(IExpressionMember member, Stack<OperatorCore> optorsStack, Stack<IExpression> operandsStack) {

			switch (member.MemberType) {
				case ExpressionMemberType.Constant:
					msgs.AddError("Unexcpected constatnt `{0}`, excpecting operator.".Fmt(((FloatConstant)member).Value), member.Position);
					return State.Error;

				case ExpressionMemberType.Variable:
					msgs.AddError("Unexcpected variable `{0}`, excpecting operator.".Fmt(((Identificator)member).Name), member.Position);
					return State.Error;

				case ExpressionMemberType.Array:
					msgs.AddError("Unexcpected array, excpecting operator.", member.Position);
					return State.Error;

				case ExpressionMemberType.Operator:
					// operator must be binary
					OperatorCore op;

					if (OperatorCore.TryGet(((Operator)member).Syntax, 2, out op)) {
						if (!tryPushOperator(op, operandsStack, optorsStack)) {
							msgs.AddError("Too few operands in expression.", member.Position);
							return State.Error;
						}
					}
					else {
						msgs.AddError("Unknown binary operator `{0}`.".Fmt(((Operator)member).Syntax), member.Position);
						return State.Error;
					}

					return State.ExcpectingOperand;

				case ExpressionMemberType.Indexer:
					// apply indexer on previous operand
					var indexExpr = CompileExpression(((ExpressionIndexer)member).Index);

					if (operandsStack.Count < 1) {
						msgs.AddError("Failed to compile indexer. No operand to apply on.", member.Position);
						return State.Error;
					}

					operandsStack.Push(new Indexer(operandsStack.Pop(), (IExpression)indexExpr));
					return State.ExcpectingOperator;

				case ExpressionMemberType.Function:
					// Function while excpecting binary operator?
					// So directly before function is operand.
					// Lets add implicit multiplication between them.
					var funCall = compileFunctionCallFailSafe((ExpressionFunction)member);
					operandsStack.Push(funCall);

					// implicit multiplication
					optorsStack.Push(OperatorCore.Multiply);

					return State.ExcpectingOperator;

				case ExpressionMemberType.BracketedExpression:
					// Expression (probably in parenthesis) while excpecting binary operator?
					// So directly before it is operand.
					// Lets add implicit multiplication between them.
					var expr = CompileExpression(((ExpressionBracketed)member).Expression);
					operandsStack.Push(expr);

					// implicit multiplication
					optorsStack.Push(OperatorCore.Multiply);

					return State.ExcpectingOperator;

				default:
					Debug.Fail("Unhandled type {0} in {1}.".Fmt(member.GetType().Name, typeof(Ast.Expression).Name));
					msgs.AddError("Expression compiler internal error.", member.Position);
					return State.Error;
			}
		}


		private bool tryPushOperator(OperatorCore op, Stack<IExpression> operandsStack, Stack<OperatorCore> optorStack) {

			while (optorStack.Count > 0 && op.ActivePrecedence > optorStack.Peek().Precedence) {
				if (!tryPopOperator(operandsStack, optorStack)) {
					return false;
				}
			}

			optorStack.Push(op);
			return true;
		}

		/// <summary>
		/// Pops operator from top of operators stack and creates expression from it and some operands from operands stack.
		/// Result is saved on operands stack.
		/// Do not check operators stack.
		/// </summary>
		private bool tryPopOperator(Stack<IExpression> operandsStack, Stack<OperatorCore> optorStack) {

			var opTop = optorStack.Pop();
			if (opTop.Arity > operandsStack.Count) {
				return false;
			}

			if (opTop.Arity == 1) {
				operandsStack.Push(new UnaryOperator(opTop.Syntax, opTop.Precedence, opTop.ActivePrecedence,
					opTop.EvalFunction, operandsStack.Pop(), opTop.ParamsTypes[0]));
			}
			else {
				Debug.Assert(opTop.Arity == 2, "Excpected binary operator, but given have ariy {0}.".Fmt(opTop.Arity));

				var right = operandsStack.Pop();
				var left = operandsStack.Pop();

				operandsStack.Push(new BinaryOperator(opTop.Syntax, opTop.Precedence, opTop.ActivePrecedence,
					opTop.EvalFunction, left, opTop.ParamsTypes[0], right, opTop.ParamsTypes[1]));

			}

			return true;
		}


		private IExpression compileFunctionCallFailSafe(ExpressionFunction funCall) {

			var rsltArgs = CompileFailSafe(funCall.Arguments);

			FunctionCore fun;
			if (FunctionCore.TryGet(funCall.NameId.Name, funCall.Arguments.Length, out fun)) {
				Debug.Assert(fun.ParametersCount == funCall.Arguments.Length, "Excpected function with {0} params, but it has {1}.".Fmt(funCall.Arguments.Length, fun.ParametersCount));

				return new FunctionCall(funCall.NameId.Name, fun.EvalFunction, rsltArgs, fun.ParamsTypes);
			}
			else {
				return new UserFunctionCall(funCall.NameId.Name, rsltArgs);
			}
		}


		private enum State {
			Error,
			ExcpectingOperand,
			ExcpectingOperator,
		}
	}
}

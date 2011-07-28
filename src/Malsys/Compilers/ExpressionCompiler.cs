using System.Collections.Generic;
using System.Diagnostics;
using Malsys.Ast;
using Malsys.Expressions;

namespace Malsys.Compilers {
	public static class ExpressionCompiler {

		/// <summary>
		/// Tries to compile expression from AST.
		/// Thread safe.
		/// </summary>
		public static bool TryCompile(Expression expr, CompilerParametersInternal prms, out IExpression result) {
			if (!tryCompile(expr, prms, out result)) {
				prms.Messages.AddMessage("Failed to compile expression.", CompilerMessageType.Error, expr.Position);
				result = null;
				return false;
			}

			return true;
		}

		/// <summary>
		/// Tries to compile list of expressions from AST.
		/// Thread safe.
		/// </summary>
		public static bool TryCompile(ImmutableList<Expression> exprs, CompilerParametersInternal prms, out ImmutableList<IExpression> result) {

			var compiledExprs = new IExpression[exprs.Length];

			for (int i = 0; i < exprs.Length; i++) {
				if (!tryCompile(exprs[i], prms, out compiledExprs[i])) {
					prms.Messages.AddMessage("Failed to compile {0}. expression.".Fmt(i), CompilerMessageType.Error, exprs[i].Position);
					result = null;
					return false;
				}
			}

			result = new ImmutableList<IExpression>(compiledExprs, true);
			return true;
		}

		public static bool TryCompileRich(Ast.RichExpression rExprAst, CompilerParametersInternal prms, out RichExpression result) {

			ImmutableList<VariableDefinition> varDefs;
			if (!VariableDefinitionCompiler.TryCompile(rExprAst.VariableDefinitions, prms, out varDefs)) {
				result = null;
				return false;
			}

			IExpression expr;
			if (!TryCompile(rExprAst.Expression, prms, out expr)) {
				result = null;
				return false;
			}

			result = new RichExpression(varDefs, expr);
			return true;
		}



		private static bool tryCompile(Expression expr, CompilerParametersInternal prms, out IExpression result) {

			Stack<OperatorCore> optorsStack = new Stack<OperatorCore>();
			Stack<IExpression> operandsStack = new Stack<IExpression>();

			State state = State.ExcpectingOperand;

			foreach (var member in expr) {
				switch (state) {
					case State.ExcpectingOperand:
						state = handleAsOperand(member, optorsStack, operandsStack, prms);
						break;

					case State.ExcpectingOperator:
						state = handleAsOperator(member, optorsStack, operandsStack, prms);
						break;

					case State.Error:
						result = null;
						return false;

					default:
						Debug.Fail("Unknown expression compiler state `{0}`".Fmt(state));
						prms.Messages.AddMessage("Compiler internal error.", CompilerMessageType.Error, member.Position);
						result = null;
						return false;
				}
			}

			if (state == State.Error) {
				result = null;
				return false;
			}

			if (state != State.ExcpectingOperator) {
				prms.Messages.AddMessage("Unexcpected end of expression", CompilerMessageType.Error, expr.Position);
				result = null;
				return false;
			}

			while (optorsStack.Count > 0) {
				if (!tryPopOperator(operandsStack, optorsStack)) {
					prms.Messages.AddMessage("Too few operands.", CompilerMessageType.Error, expr.Position);
					result = null;
					return false;
				}
			}

			if (operandsStack.Count != 1) {
				prms.Messages.AddMessage("Too many operands.", CompilerMessageType.Error, expr.Position);
				result = null;
				return false;
			}

			result = operandsStack.Pop();
			return true;
		}


		private static State handleAsOperand(IExpressionMember member, Stack<OperatorCore> optorsStack, Stack<IExpression> operandsStack, CompilerParametersInternal prms) {

			switch (member.MemberType) {
				case ExpressionMemberType.Constant:
					operandsStack.Push(((FloatConstant)member).Value.ToConst());
					return State.ExcpectingOperator;

				case ExpressionMemberType.Variable:
					Identificator id = (Identificator)member;
					KnownConstant cnst;

					if (KnownConstant.TryParse(id.Name, out cnst)) {
						// known constant
						operandsStack.Push(cnst.Value.ToConst());
					}
					else {
						// variable
						operandsStack.Push(id.Name.ToVar());
					}

					return State.ExcpectingOperator;

				case ExpressionMemberType.Array:

					var arr = (ExpressionsArray)member;
					IExpression[] resArr = new IExpression[arr.Length];

					for (int i = 0; i < resArr.Length; i++) {
						IExpression val;
						if (TryCompile(arr[i], prms, out val)) {
							resArr[i] = val;
						}
						else {
							return State.Error;
						}
					}

					var resImm = new ImmutableList<IExpression>(resArr, true);
					operandsStack.Push(new ExpressionValuesArray(resImm));
					return State.ExcpectingOperator;

				case ExpressionMemberType.Operator:
					// operator while excpecting operand? It must be unary!
					OperatorCore op;

					if (OperatorCore.TryGet(((Operator)member).Syntax, 1, out op)) {
						if (!tryPushOperator(op, operandsStack, optorsStack)) {
							prms.Messages.AddMessage("Too few operands.", CompilerMessageType.Error, member.Position);
							return State.Error;
						}
					}
					else {
						prms.Messages.AddMessage("Unknown unary operator `{1}`.".Fmt(((Operator)member).Syntax),
							CompilerMessageType.Error, member.Position);
						return State.Error;
					}

					return State.ExcpectingOperand;

				case ExpressionMemberType.Indexer:
					prms.Messages.AddMessage("Unexcpected indexer, excpecting operand.", CompilerMessageType.Error, member.Position);
					return State.Error;

				case ExpressionMemberType.Function:

					var funCall = (ExpressionFunction)member;

					IExpression resultFun;

					if (tryCompileFunctionCall(funCall, prms, out resultFun)) {
						operandsStack.Push(resultFun);
					}
					else {
						return State.Error;
					}

					return State.ExcpectingOperator;

				case ExpressionMemberType.BracketedExpression:

					IExpression exprResult;

					if (TryCompile(((ExpressionBracketed)member).Expression, prms, out exprResult)) {
						operandsStack.Push(exprResult);
					}
					else {
						return State.Error;
					}

					return State.ExcpectingOperator;

				default:
					Debug.Fail("Unhandled type `{0}` in Ast.Expression.".Fmt(member.GetType().ToString()));
					prms.Messages.AddMessage("Compiler internal error.", CompilerMessageType.Error, member.Position);
					return State.Error;
			}
		}

		private static State handleAsOperator(IExpressionMember member, Stack<OperatorCore> optorsStack, Stack<IExpression> operandsStack, CompilerParametersInternal prms) {

			switch (member.MemberType) {
				case ExpressionMemberType.Constant:
					prms.Messages.AddMessage("Unexcpected constatnt, excpecting operator.", CompilerMessageType.Error, member.Position);
					return State.Error;

				case ExpressionMemberType.Variable:
					prms.Messages.AddMessage("Unexcpected variable `{0}`, excpecting operator.".Fmt(((Identificator)member).Name),
						CompilerMessageType.Error, member.Position);
					return State.Error;

				case ExpressionMemberType.Array:
					prms.Messages.AddMessage("Unexcpected array, excpecting operator.", CompilerMessageType.Error, member.Position);
					return State.Error;

				case ExpressionMemberType.Operator:
					// operator must be binary
					OperatorCore op;

					if (OperatorCore.TryGet(((Operator)member).Syntax, 2, out op)) {
						if (!tryPushOperator(op, operandsStack, optorsStack)) {
							prms.Messages.AddMessage("Too few operands.", CompilerMessageType.Error, member.Position);
							return State.Error;
						}
					}
					else {
						prms.Messages.AddMessage("Unknown binary operator `{1}`.".Fmt(((Operator)member).Syntax),
							CompilerMessageType.Error, member.Position);
						return State.Error;
					}

					return State.ExcpectingOperand;

				case ExpressionMemberType.Indexer:
					// apply indexer on previous operand
					var indexer = (ExpressionIndexer)member;

					IExpression indexExpr;
					if (!TryCompile(indexer.Index, prms, out indexExpr)) {
						prms.Messages.AddMessage("Failed to compile indexer's value.", CompilerMessageType.Error, member.Position);
						return State.Error;
					}

					if (operandsStack.Count < 1) {
						prms.Messages.AddMessage("Failed to compile indexer. No operand to apply on.", CompilerMessageType.Error, member.Position);
						return State.Error;
					}

					operandsStack.Push(new Indexer(operandsStack.Pop(), indexExpr));
					return State.ExcpectingOperator;

				case ExpressionMemberType.Function:
					// Function while excpecting binary operator?
					// So directly before function is operand.
					// Lets add implicit multiplication between them.
					var funCall = (ExpressionFunction)member;

					IExpression resultFun;

					if (tryCompileFunctionCall(funCall, prms, out resultFun)) {
						operandsStack.Push(resultFun);
					}
					else {
						return State.Error;
					}

					// implicit multiplication
					optorsStack.Push(OperatorCore.Multiply);

					return State.ExcpectingOperator;

				case ExpressionMemberType.BracketedExpression:
					// Expression (probably in parenthesis) while excpecting binary operator?
					// So directly before it is operand.
					// Lets add implicit multiplication between them.

					IExpression exprResult;

					if (TryCompile(((ExpressionBracketed)member).Expression, prms, out exprResult)) {
						operandsStack.Push(exprResult);
					}
					else {
						return State.Error;
					}

					// implicit multiplication
					optorsStack.Push(OperatorCore.Multiply);

					return State.ExcpectingOperator;

				default:
					Debug.Fail("Unhandled type `{0}` in Ast.Expression.".Fmt(member.GetType().ToString()));
					prms.Messages.AddMessage("Compiler internal error.", CompilerMessageType.Error, member.Position);
					return State.Error;
			}
		}


		private static bool tryPushOperator(OperatorCore op, Stack<IExpression> operandsStack, Stack<OperatorCore> optorStack) {

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
		private static bool tryPopOperator(Stack<IExpression> operandsStack, Stack<OperatorCore> optorStack) {

			var opTop = optorStack.Pop();
			if (opTop.Arity > operandsStack.Count) {
				return false;
			}

			if (opTop.Arity == 1) {
				operandsStack.Push(new UnaryOperator(opTop.Syntax, opTop.Precedence, opTop.ActivePrecedence,
					opTop.EvalFunction, operandsStack.Pop(), opTop.ParamsTypes[0]));
			}
			else {
				Debug.Assert(opTop.Arity == 2, "Excpected binary operator.");

				var right = operandsStack.Pop();
				var left = operandsStack.Pop();

				operandsStack.Push(new BinaryOperator(opTop.Syntax, opTop.Precedence, opTop.ActivePrecedence,
					opTop.EvalFunction, left, opTop.ParamsTypes[0], right, opTop.ParamsTypes[1]));

			}

			return true;
		}


		private static bool tryCompileFunctionCall(ExpressionFunction funCall, CompilerParametersInternal prms, out IExpression result) {

			ImmutableList<IExpression> argsImm;

			if (!TryCompile(funCall.Arguments, prms, out argsImm)) {
				prms.Messages.AddMessage("Failed to compile arguments of function `{0}`.".Fmt(funCall.NameId.Name),
					CompilerMessageType.Error, funCall.Position);
			}

			if (prms.Messages.ErrorOcured) {
				result = null;
				return false;
			}

			FunctionCore fun;
			if (FunctionCore.TryGet(funCall.NameId.Name, funCall.Arguments.Length, out fun)) {
				Debug.Assert(fun.ParametersCount == funCall.Arguments.Length, "Excpected function with {0} params, but it has {1}.".Fmt(funCall.Arguments.Length, fun.ParametersCount));

				result = new FunctionCall(funCall.NameId.Name, fun.EvalFunction, argsImm, fun.ParamsTypes);
			}
			else {
				result = new UserFunctionCall(funCall.NameId.Name, argsImm);
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

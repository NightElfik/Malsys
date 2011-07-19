using System.Collections.Generic;
using System.Diagnostics;
using Malsys.Ast;
using Malsys.Expressions;

namespace Malsys.Compilers {

	public static class ExpressionCompiler {

		public static bool TryCompile(Expression expr, MessagesCollection msgs, out PostfixExpression expression) {

			var postfix = new List<object>();

			if (tryCompile(expr, msgs, postfix)) {
				expression = new PostfixExpression(postfix);
				return true;
			}
			else {
				expression = null;
				return false;
			}
		}


		private static bool tryCompile(Expression expr, MessagesCollection msgs, List<object> postfix) {

			Stack<KnownArithmeticOperator> opStack = new Stack<KnownArithmeticOperator>();

			State state = State.ExcpectingOperand;

			for (int i = 0; i < expr.Members.Count; i++) {
				var member = expr.Members[i];

				if (state == State.ExcpectingOperand) {

					if (member is FloatConstant) {
						postfix.Add(((FloatConstant)member).Value);
						state = State.ExcpectingOperator;
					}

					else if (member is Identificator) {
						Identificator id = (Identificator)member;
						KnownConstant cnst;

						if (KnownConstant.TryParse(id.Name, out cnst)) {
							// known constant
							postfix.Add(cnst.Value);
						}
						else {
							// variable
							postfix.Add(id.Name);
							state = State.ExcpectingOperator;

						}
					}

					else if (member is Operator) {
						// operator while excpecting operand? It must be unary!
						KnownArithmeticOperator op;

						if (KnownArithmeticOperator.TryParse(((Operator)member).Syntax, 1, out op)) {
							pushOperator(op, postfix, opStack);
						}
						else {
							msgs.AddMessage("Unknown unary operator `{1}`.".Fmt(((Operator)member).Syntax),
								CompilerMessageType.Error, member.Position);
							return false;
						}

						// still excpecting operand
					}

					else if (member is ExpressionFunction) {
						var funCall = (ExpressionFunction)member;

						tryCompileFunctionCall(funCall, postfix, msgs);
						if (msgs.ErrorOcured) {
							return false;
						}

						state = State.ExcpectingOperator;
					}

					else if (member is Expression) {
						if (!tryCompile((Expression)member, msgs, postfix)) {
							return false;
						}
					}

					else {
						Debug.Fail("Unhandled type in Ast.Expression.");
						msgs.AddMessage("Compiler error.", CompilerMessageType.Error, member.Position);
						return false;
					}
				}
				else { // excpecting operator

					if (member is FloatConstant) {
						msgs.AddMessage("Unexcpected constatnt, excpecting operator.",
							CompilerMessageType.Error, member.Position);
						return false;
					}

					else if (member is Identificator) {
						msgs.AddMessage("Unexcpected variable `{0}`, excpecting operator.".Fmt(((Identificator)member).Name),
							CompilerMessageType.Error, member.Position);
						return false;
					}

					else if (member is Operator) {
						// operator must be binary
						KnownArithmeticOperator op;

						if (KnownArithmeticOperator.TryParse(((Operator)member).Syntax, 2, out op)) {
							pushOperator(op, postfix, opStack);
						}
						else {
							msgs.AddMessage("Unknown binary operator `{1}`.".Fmt(((Operator)member).Syntax),
								CompilerMessageType.Error, member.Position);
							return false;
						}

						state = State.ExcpectingOperand;
					}

					else if (member is ExpressionFunction) {
						// Function while excpecting binary operator?
						// So directly before function is operand.
						// Lets add implicit multiplication between them.
						var funCall = (ExpressionFunction)member;

						tryCompileFunctionCall(funCall, postfix, msgs);
						if (msgs.ErrorOcured) {
							return false;
						}

						// implicit multiplication
						postfix.Add(KnownArithmeticOperator.Multiply);

						// still excpecting operator
					}

					else if (member is Expression) {
						// Expression (probably in parenthesis) while excpecting binary operator?
						// So directly before it is operand.
						// Lets add implicit multiplication between them.

						if (!tryCompile((Expression)member, msgs, postfix)) {
							return false;
						}

						// implicit multiplication
						postfix.Add(KnownArithmeticOperator.Multiply);

						// still excpecting operator
					}

					else {
						Debug.Fail("Unhandled type in Ast.Expression.");
						msgs.AddMessage("Compiler error.", CompilerMessageType.Error, member.Position);
						return false;
					}
				}
			}

			return true;
		}


		private static void pushOperator(KnownArithmeticOperator op, List<object> postfix, Stack<KnownArithmeticOperator> opStack) {

			while (opStack.Count > 0 && op.ActivePrecedence > opStack.Peek().Precedence) {
				postfix.Add(opStack.Pop());
			}

			opStack.Push(op);
		}

		/// <summary>
		/// Tries to compile given function call into postfix expression and appnds it into given list.
		/// Any messages (like errors) are writen into given messages collection.
		/// </summary>
		private static bool tryCompileFunctionCall(ExpressionFunction funCall, List<object> postfix, MessagesCollection msgs) {

			KnownArithmeticFunction fun;

			if (KnownArithmeticFunction.TryGet(funCall.NameId.Name, funCall.Arity, out fun)) {
				Debug.Assert(fun.Arity == funCall.Arity, "Obtained function has bad arity.");

				for (int i = 0; i < funCall.Arguments.Count; i++) {
					if (!tryCompile(funCall.Arguments[i], msgs, postfix)) {
						msgs.AddMessage("Failed to compile {0}. argument of function `{1}`.".Fmt(i, funCall.NameId.Name),
							CompilerMessageType.Error, funCall.Arguments[i].Position);
					}
				}

				if (msgs.ErrorOcured) {
					return false;
				}

				postfix.Add(fun);
			}
			else {
				var args = new PostfixExpression[funCall.Arguments.Count];
				for (int i = 0; i < args.Length; i++) {
					PostfixExpression expr;
					if (TryCompile(funCall.Arguments[i], msgs, out expr)) {
						args[i] = expr;
					}
					else {
						msgs.AddMessage("Failed to compile {0}. argument of function `{1}`.".Fmt(i, funCall.NameId.Name),
							CompilerMessageType.Error, funCall.Arguments[i].Position);
						return false;
					}
				}

				postfix.Add(new ArithmeticFunction(funCall.NameId.Name, funCall.Arity, args));
			}

			return true;
		}


		private enum State {
			ExcpectingOperator,
			ExcpectingOperand,
		}
	}
}

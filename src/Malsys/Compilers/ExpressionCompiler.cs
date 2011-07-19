using System.Collections.Generic;
using System.Diagnostics;
using Malsys.Ast;
using Malsys.Expressions;

namespace Malsys.Compilers {

	public static class ExpressionCompiler {

		public static PostfixExpression Compile(Expression expr, MessagesCollection msgs) {
			var members = compile(expr, msgs);

			if (msgs.ErrorOcured) {
				return null;
			}
			else {
				return new PostfixExpression(members);
			}
		}


		private static List<object> compile(Expression expr, MessagesCollection msgs) {

			List<object> postfix = new List<object>();
			Stack<ArithmeticOperator> opStack = new Stack<ArithmeticOperator>();

			State state = State.ExcpectingOperand;

			if (state == State.ExcpectingOperand) {
				for (int i = 0; i < expr.Members.Count; i++) {
					var member = expr.Members[i];
					if (member is FloatConstant) {
						postfix.Add(((FloatConstant)member).Value);
						state = State.ExcpectingOperator;
					}
					else if (member is Identificator) {
						postfix.Add(((Identificator)member).Name);
						state = State.ExcpectingOperator;
					}
					else if (member is Operator) { // operator while excpecting operand? It must be unary!
						ArithmeticOperator op;

						if (ArithmeticOperator.TryParse(((Operator)member).Syntax, 1, out op)) {
							pushOperator(op, postfix, opStack);
						}
						else {
							msgs.AddMessage("Unknown unary operator `{1}`.".Fmt(((Operator)member).Syntax),
								CompilerMessageType.Error, member.Position);
							return null;
						}

						// still excpecting operand
					}
					else if (member is ExpressionFunction) {
						var funCall = (ExpressionFunction)member;

						addFunction(funCall, postfix, msgs);
						if (msgs.ErrorOcured) {
							return null;
						}

						state = State.ExcpectingOperator;
					}
					else if (member is Expression) {
						var pfx = compile((Expression)member, msgs);
						if (msgs.ErrorOcured) {
							return null;
						}

						postfix.AddRange(pfx);
					}
					else {
						Debug.Fail("Unhandled type in Ast.Expression.");
						msgs.AddMessage("Compiler error.", CompilerMessageType.Error, expr.Position);
					}
				}
			}
			else { // excpecting operator

			}

			return postfix;
		}


		private static void pushOperator(ArithmeticOperator op, List<object> postfix, Stack<ArithmeticOperator> opStack) {
			while (opStack.Count > 0) {
				ArithmeticOperator opTop = opStack.Peek();
				if (op.ActivePrecedence > opTop.Precedence) {
					postfix.Add(opStack.Pop());
				}
				else {
					break;
				}
			}

			opStack.Push(op);
		}

		private static void addFunction(ExpressionFunction funCall, List<object> postfix, MessagesCollection msgs) {

			var cmpArgs = new List<object>[funCall.Arguments.Count];

			for (int i = 0; i < funCall.Arguments.Count; i++) {
				cmpArgs[i] = compile(funCall.Arguments[i], msgs);

				if (cmpArgs[i].Count == 0 || msgs.ErrorOcured) {
					msgs.AddMessage("Failed to compile {0}. argument of function `{1}`.".Fmt(i, funCall.NameId.Name),
						CompilerMessageType.Error, funCall.Arguments[i].Position);
					return;
				}
			}

			KnownArithmeticFunction fun;

			if (KnownArithmeticFunction.TryGet(funCall.NameId.Name, funCall.Arity, out fun)) {
				Debug.Assert(fun.Arity == funCall.Arity, "Obtained function has bad arity.");

				foreach (var arg in cmpArgs) {
					postfix.AddRange(arg);
				}

				postfix.Add(fun);
			}
			else {
				var args = new PostfixExpression[cmpArgs.Length];
				for (int i = 0; i < args.Length; i++) {
					args[i] = new PostfixExpression(cmpArgs[i]);
				}

				postfix.Add(new ArithmeticFunction(funCall.NameId.Name, funCall.Arity, args));
			}
		}


		private enum State {
			ExcpectingOperator,
			ExcpectingOperand,
		}
	}
}

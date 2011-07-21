using System.Collections.Generic;
using System.Diagnostics;
using Malsys.Ast;
using Malsys.Expressions;

namespace Malsys.Compilers {
	/// <summary>
	/// Compiles expression from AST.
	/// </summary>
	public static class ExpressionCompiler {

		/// <summary>
		/// Tries to compile expression from AST to postfix expression.
		/// </summary>
		public static bool TryCompile(Expression expr, ExpressionCompilerParameters prms, out PostfixExpression expression) {

			var postfix = new List<IPostfixExpressionMember>();

			if (tryCompile(expr, prms, postfix)) {
				expression = new PostfixExpression(postfix);
				return true;
			}
			else {
				expression = null;
				return false;
			}
		}


		private static bool tryCompile(Expression expr, ExpressionCompilerParameters prms, List<IPostfixExpressionMember> postfix) {

			Stack<KnownOperator> opStack = new Stack<KnownOperator>();

			State state = State.ExcpectingOperand;

			for (int i = 0; i < expr.Members.Count; i++) {
				var member = expr.Members[i];

				if (state == State.ExcpectingOperand) {

					if (member.IsConstant) {
						postfix.Add(((FloatConstant)member).Value.ToConst());
						state = State.ExcpectingOperator;
					}

					else if (member.IsVariable) {
						Identificator id = (Identificator)member;
						KnownConstant cnst;

						string name = prms.CaseSensitiveVarsNames ? id.Name : id.Name.ToLower();

						if (KnownConstant.TryParse(name, out cnst)) {
							// known constant
							postfix.Add(cnst.Value.ToConst());
						}
						else {
							// variable
							postfix.Add(id.Name.ToVar());
							state = State.ExcpectingOperator;
						}
					}

					if (member.IsArray) {
						IExpressionValue result;
						if (ValueCompiler.TryCompile((Ast.ValuesArray)member, prms, out result)) {
							postfix.Add((ExpressionValuesArray)result); // safe cast because member is array
						}
						else {
							return false;
						}

						state = State.ExcpectingOperator;
					}

					else if (member.IsOperator) {
						// operator while excpecting operand? It must be unary!
						KnownOperator op;

						if (KnownOperator.TryParse(((Operator)member).Syntax, 1, out op)) {
							pushOperator(op, postfix, opStack);
						}
						else {
							prms.Messages.AddMessage("Unknown unary operator `{1}`.".Fmt(((Operator)member).Syntax),
								CompilerMessageType.Error, member.Position);
							return false;
						}

						// still excpecting operand
					}

					else if (member.IsFunction) {
						var funCall = (ExpressionFunction)member;

						tryCompileFunctionCall(funCall, postfix, prms);
						if (prms.Messages.ErrorOcured) {
							return false;
						}

						state = State.ExcpectingOperator;
					}

					else if (member.IsIndexer) {
						prms.Messages.AddMessage("Unexcpected indexer, excpecting operand.",
							CompilerMessageType.Error, member.Position);
						return false;
					}

					else if (member.IsBracketedExpression) {
						if (!tryCompile(((ExpressionBracketed)member).Expression, prms, postfix)) {
							return false;
						}
					}

					else {
						Debug.Fail("Unhandled type in Ast.Expression.");
						prms.Messages.AddMessage("Compiler internal error.", CompilerMessageType.Error, member.Position);
						return false;
					}
				}
				else { // excpecting operator

					if (member.IsConstant) {
						prms.Messages.AddMessage("Unexcpected constatnt, excpecting operator.",
							CompilerMessageType.Error, member.Position);
						return false;
					}

					else if (member.IsVariable) {
						prms.Messages.AddMessage("Unexcpected variable `{0}`, excpecting operator.".Fmt(((Identificator)member).Name),
							CompilerMessageType.Error, member.Position);
						return false;
					}

					else if (member.IsOperator) {
						// operator must be binary
						KnownOperator op;

						if (KnownOperator.TryParse(((Operator)member).Syntax, 2, out op)) {
							pushOperator(op, postfix, opStack);
						}
						else {
							prms.Messages.AddMessage("Unknown binary operator `{1}`.".Fmt(((Operator)member).Syntax),
								CompilerMessageType.Error, member.Position);
							return false;
						}

						state = State.ExcpectingOperand;
					}

					else if (member.IsIndexer) {
						// apply indexer on previous operand
						var indexer = (ExpressionIndexer)member;

						PostfixExpression indexExpr;
						if (!TryCompile(indexer.Index, prms, out indexExpr)) {
							prms.Messages.AddMessage("Failed to compile indexer's value.", CompilerMessageType.Error, member.Position);
							return false;
						}

						// still excpecting operator
					}

					else if (member.IsFunction) {
						// Function while excpecting binary operator?
						// So directly before function is operand.
						// Lets add implicit multiplication between them.
						var funCall = (ExpressionFunction)member;

						tryCompileFunctionCall(funCall, postfix, prms);
						if (prms.Messages.ErrorOcured) {
							return false;
						}

						// implicit multiplication
						postfix.Add(KnownOperator.Multiply);

						// still excpecting operator
					}

					else if (member.IsBracketedExpression) {
						// Expression (probably in parenthesis) while excpecting binary operator?
						// So directly before it is operand.
						// Lets add implicit multiplication between them.

						if (!tryCompile(((ExpressionBracketed)member).Expression, prms, postfix)) {
							return false;
						}

						// implicit multiplication
						postfix.Add(KnownOperator.Multiply);

						// still excpecting operator
					}

					else {
						Debug.Fail("Unhandled type in Ast.Expression.");
						prms.Messages.AddMessage("Compiler internal error.", CompilerMessageType.Error, member.Position);
						return false;
					}
				}
			}

			return true;
		}


		private static void pushOperator(KnownOperator op, List<IPostfixExpressionMember> postfix, Stack<KnownOperator> opStack) {

			while (opStack.Count > 0 && op.ActivePrecedence > opStack.Peek().Precedence) {
				postfix.Add(opStack.Pop());
			}

			opStack.Push(op);
		}

		/// <summary>
		/// Tries to compile given function call into postfix expression and appnds it into given list.
		/// Any messages (like errors) are writen into given messages collection.
		/// </summary>
		private static bool tryCompileFunctionCall(ExpressionFunction funCall, List<IPostfixExpressionMember> postfix, ExpressionCompilerParameters prms) {

			KnownFunction fun;
			string name = prms.CaseSensitiveFunsNames ? funCall.NameId.Name : funCall.NameId.Name.ToLower();

			if (KnownFunction.TryGet(name, funCall.Arity, out fun)) {
				Debug.Assert(fun.Arity == funCall.Arity, "Obtained function has bad arity.");

				for (int i = 0; i < funCall.Arguments.Count; i++) {
					bool err = false;

					if (funCall.Arguments[i].IsExpression) {
						if (!tryCompile((Expression)funCall.Arguments[i], prms, postfix)) {
							err = true;
						}
					}
					else {  // array
						IExpressionValue arr;
						if (ValueCompiler.TryCompile(funCall.Arguments[i], prms, out arr)) {
							postfix.Add((ExpressionValuesArray)arr); // safe cast because funCall.Arguments[i] is array
						}
						else {
							err = true;
						}
					}

					if (err) {
						prms.Messages.AddMessage("Failed to compile {0}. argument of function `{1}`.".Fmt(i, funCall.NameId.Name),
							CompilerMessageType.Error, funCall.Arguments[i].Position);
					}
				}

				if (prms.Messages.ErrorOcured) {
					return false;
				}

				postfix.Add(fun);
			}
			else {
				var args = new IExpressionValue[funCall.Arguments.Count];
				for (int i = 0; i < args.Length; i++) {
					IExpressionValue expr;
					if (ValueCompiler.TryCompile(funCall.Arguments[i], prms, out expr)) {
						args[i] = expr;
					}
					else {
						prms.Messages.AddMessage("Failed to compile {0}. argument of function `{1}`.".Fmt(i, funCall.NameId.Name),
							CompilerMessageType.Error, funCall.Arguments[i].Position);
						return false;
					}
				}

				postfix.Add(new UnknownFunction(funCall.NameId.Name, funCall.Arity, args));
			}

			return true;
		}


		private enum State {
			ExcpectingOperator,
			ExcpectingOperand,
		}
	}
}

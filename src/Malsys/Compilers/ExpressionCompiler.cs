using System.Collections.Generic;
using System.Diagnostics;
using Malsys.Ast;
using Malsys.Expressions;

namespace Malsys.Compilers {
	public static class ExpressionCompiler {

		/// <summary>
		/// Tries to compile expression from AST.
		/// </summary>
		public static bool TryCompile(Expression expr, ExpressionCompilerParameters prms, out IExpression expression) {

			Stack<OperatorCore> optorsStack = new Stack<OperatorCore>();
			Stack<IExpression> operandsStack = new Stack<IExpression>();

			State state = State.ExcpectingOperand;
			expression = null;

			foreach (var member in expr.Members) {

				if (state == State.ExcpectingOperand) {

					if (member.IsConstant) {
						operandsStack.Push(((FloatConstant)member).Value.ToConst());
						state = State.ExcpectingOperator;
					}

					else if (member.IsVariable) {
						Identificator id = (Identificator)member;
						KnownConstant cnst;

						string name = prms.CaseSensitiveVarsNames ? id.Name : id.Name.ToLower();

						if (KnownConstant.TryParse(name, out cnst)) {
							// known constant
							operandsStack.Push(cnst.Value.ToConst());
						}
						else {
							// variable
							operandsStack.Push(name.ToVar());
						}

						state = State.ExcpectingOperator;
					}

					if (member.IsArray) {
						ExpressionValuesArray resultArr;

						if (ValueCompiler.TryCompile((Ast.ValuesArray)member, prms, out resultArr)) {
							operandsStack.Push(resultArr);
						}
						else {
							return false;
						}

						state = State.ExcpectingOperator;
					}

					else if (member.IsOperator) {
						// operator while excpecting operand? It must be unary!
						OperatorCore op;

						if (OperatorCore.TryGet(((Operator)member).Syntax, 1, out op)) {
							if (!tryPushOperator(op, operandsStack, optorsStack)) {
								prms.Messages.AddMessage("Too few operands.", CompilerMessageType.Error, member.Position);
								return false;
							}
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

						IExpression resultFun;

						if (tryCompileFunctionCall(funCall, prms, out resultFun)) {
							operandsStack.Push(resultFun);
						}
						else {
							return false;
						}

						state = State.ExcpectingOperator;
					}

					else if (member.IsIndexer) {
						prms.Messages.AddMessage("Unexcpected indexer, excpecting operand.", CompilerMessageType.Error, member.Position);
						return false;
					}

					else if (member.IsBracketedExpression) {
						IExpression exprResult;

						if (TryCompile(((ExpressionBracketed)member).Expression, prms, out exprResult)) {
							operandsStack.Push(exprResult);
						}
						else {
							return false;
						}

						state = State.ExcpectingOperator;
					}

					else {
						Debug.Fail("Unhandled type in Ast.Expression.");
						prms.Messages.AddMessage("Compiler internal error.", CompilerMessageType.Error, member.Position);
						return false;
					}
				}
				else { // excpecting operator

					if (member.IsConstant) {
						prms.Messages.AddMessage("Unexcpected constatnt, excpecting operator.", CompilerMessageType.Error, member.Position);
						return false;
					}

					else if (member.IsVariable) {
						prms.Messages.AddMessage("Unexcpected variable `{0}`, excpecting operator.".Fmt(((Identificator)member).Name),
							CompilerMessageType.Error, member.Position);
						return false;
					}

					else if (member.IsOperator) {
						// operator must be binary
						OperatorCore op;

						if (OperatorCore.TryGet(((Operator)member).Syntax, 2, out op)) {
							if (!tryPushOperator(op, operandsStack, optorsStack)) {
								prms.Messages.AddMessage("Too few operands.", CompilerMessageType.Error, member.Position);
								return false;
							}
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

						IExpression indexExpr;
						if (!TryCompile(indexer.Index, prms, out indexExpr)) {
							prms.Messages.AddMessage("Failed to compile indexer's value.", CompilerMessageType.Error, member.Position);
							return false;
						}

						if (operandsStack.Count < 1) {
							prms.Messages.AddMessage("Failed to compile indexer. No operand to apply on.", CompilerMessageType.Error, member.Position);
							return false;
						}

						operandsStack.Push(new Indexer(operandsStack.Pop(), indexExpr));
						// still excpecting operator
					}

					else if (member.IsFunction) {
						// Function while excpecting binary operator?
						// So directly before function is operand.
						// Lets add implicit multiplication between them.
						var funCall = (ExpressionFunction)member;

						IExpression resultFun;

						if (tryCompileFunctionCall(funCall, prms, out resultFun)) {
							operandsStack.Push(resultFun);
						}
						else {
							return false;
						}

						// implicit multiplication
						optorsStack.Push(OperatorCore.Multiply);

						// still excpecting operator
					}

					else if (member.IsBracketedExpression) {
						// Expression (probably in parenthesis) while excpecting binary operator?
						// So directly before it is operand.
						// Lets add implicit multiplication between them.

						IExpression exprResult;

						if (TryCompile(((ExpressionBracketed)member).Expression, prms, out exprResult)) {
							operandsStack.Push(exprResult);
						}
						else {
							return false;
						}

						// implicit multiplication
						optorsStack.Push(OperatorCore.Multiply);

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


		private static bool tryPushOperator(OperatorCore op, Stack<IExpression> operandsStack, Stack<OperatorCore> optorStack) {

			while (optorStack.Count > 0 && op.ActivePrecedence > optorStack.Peek().Precedence) {

				var opTop = optorStack.Peek();
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
			}

			optorStack.Push(op);
			return true;
		}


		private static bool tryCompileFunctionCall(ExpressionFunction funCall, ExpressionCompilerParameters prms, out IExpression result) {

			FunctionCore fun;
			string name = prms.CaseSensitiveFunsNames ? funCall.NameId.Name : funCall.NameId.Name.ToLower();

			IExpression[] args = new IExpression[funCall.ArgumentsCount];

			for (int i = 0; i < funCall.ArgumentsCount; i++) {
				if (!ValueCompiler.TryCompile(funCall.arguments[i], prms, out args[i])) {
					prms.Messages.AddMessage("Failed to compile {0}. argument of function `{1}`.".Fmt(i, funCall.NameId.Name),
						CompilerMessageType.Error, funCall.arguments[i].Position);
				}
			}

			if (prms.Messages.ErrorOcured) {
				result = null;
				return false;
			}

			if (FunctionCore.TryGet(name, funCall.ArgumentsCount, out fun)) {
				Debug.Assert(fun.ParametersCount == funCall.ArgumentsCount, "Excpected function with {0} params, but it has {1}.".Fmt(funCall.ArgumentsCount, fun.ParametersCount));

				result = new Function(name, fun.EvalFunction, args, fun.ParamsTypes);
			}
			else {
				result = new UserFunction(name, args);
			}

			return true;
		}


		private enum State {
			ExcpectingOperator,
			ExcpectingOperand,
		}
	}
}

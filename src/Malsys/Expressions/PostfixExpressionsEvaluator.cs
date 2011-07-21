using System;
using System.Collections.Generic;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;
using VarMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.Expressions.IValue>;
using FunMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.UserFunction>;

namespace Malsys.Expressions {
	public static class PostfixExpressionsEvaluator {
		/// <summary>
		/// Args storage for each thread to avoid unnecessary locking.
		/// </summary>
		[ThreadStatic]
		private static ArgsStorage argsStorage = new ArgsStorage();


		public static IValue Evaluate(PostfixExpression expr, VarMap variables, FunMap functions) {
			Stack<IValue> stack = new Stack<IValue>(4);

			foreach (var mbr in expr.GetMembers()) {
				if (mbr.IsConstant) {
					stack.Push((Constant)mbr);
				}

				else if (mbr.IsVariable) {
					Variable var = (Variable)mbr;
					var maybeValue = MapModule.TryFind(var.Name, variables);

					if (OptionModule.IsSome(maybeValue)) {
						stack.Push(maybeValue.Value);
					}
					else {
						throw new EvalException("Unknown variable `{0}`.".Fmt(mbr));
					}
				}

				else if (mbr.IsEvaluable) {
					IEvaluable evaluable = (IEvaluable)mbr;

					if (stack.Count < evaluable.Arity) {
						throw new EvalException("Failed to evaluate {0} `{1}` (arty {2}) with only {3} arguments.".Fmt(
							evaluable.Name, evaluable.Syntax, evaluable.Arity, stack.Count));
					}

					argsStorage.TakeArgs(evaluable.Arity, stack);

					stack.Push(evaluable.Evaluate(argsStorage));
				}

				else if (mbr.IsUnknownFunction) {
					UnknownFunction fun = (UnknownFunction)mbr;
					var mybeFunction = MapModule.TryFind(fun.Name, functions);

					if (OptionModule.IsSome(mybeFunction)) {

					}
					else {
						throw new EvalException("Unknown function `{0}`.".Fmt(mbr));
					}
				}
#if DEBUG
				else {
					throw new InvalidOperationException("Type `{0}` is not supported in postfix expression.".Fmt(mbr.GetType().Name));
				}
#endif
			}

			if (stack.Count != 1) {
				throw new EvalException("Too much operands to evaluate expression.");
			}

			return stack.Pop();
		}
	}
}

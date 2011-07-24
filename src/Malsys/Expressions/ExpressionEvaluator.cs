using System;
using Microsoft.FSharp.Collections;
using FunMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.FunctionDefinition>;
using VarMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.Expressions.IValue>;

namespace Malsys.Expressions {
	public static class ExpressionEvaluator {

		[ThreadStatic]
		private static ExpressionEvalVisitor evalVisitor;


		/// <summary>
		/// Evaluates expression with no defined variables nor functions.
		/// Thread safe.
		/// </summary>
		public static IValue Evaluate(IExpression expr) {
			return Evaluate(expr, MapModule.Empty<string, IValue>(), MapModule.Empty<string, FunctionDefinition>());
		}

		/// <summary>
		/// Evaluates expression with given variables and functions.
		/// Thread safe.
		/// </summary>
		public static IValue Evaluate(IExpression expr, VarMap variables, FunMap functions) {

			if (evalVisitor == null) {
				evalVisitor = new ExpressionEvalVisitor();
			}

			return evalVisitor.Evaluate(expr, variables, functions);
		}
	}
}

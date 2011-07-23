using System;
using Microsoft.FSharp.Collections;
using FunMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.FunctionDefinition>;
using VarMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.Expressions.IValue>;

namespace Malsys.Expressions {
	public static class ExpressionEvaluator {

		[ThreadStatic]
		private static ExpressionEvalVisitor evalVisitor = new ExpressionEvalVisitor();

		/// <summary>
		/// Evaluates expression with no defined variables nor functions.
		/// </summary>
		public static IValue Evaluate(IExpression expr) {
			return Evaluate(expr, MapModule.Empty<string, IValue>(), MapModule.Empty<string, FunctionDefinition>());
		}

		public static IValue Evaluate(IExpression expr, VarMap variables, FunMap functions) {

			return evalVisitor.Evaluate(expr, variables, functions);
		}
	}
}

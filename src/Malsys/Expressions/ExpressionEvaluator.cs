using System;
using FunMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.FunctionDefinition>;
using VarMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.Expressions.IValue>;

namespace Malsys.Expressions {
	public static class ExpressionEvaluator {

		[ThreadStatic]
		private static ExpressionEvalVisitor evalVisitor = new ExpressionEvalVisitor();

		public static IValue Evaluate(IExpression expr, VarMap variables, FunMap functions) {

			return evalVisitor.Evaluate(expr, variables, functions);
		}
	}
}

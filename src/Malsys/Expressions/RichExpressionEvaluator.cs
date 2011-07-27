using FunMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.FunctionDefinition>;
using VarMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.Expressions.IValue>;

namespace Malsys.Expressions {
	public static class RichExpressionEvaluator {

		public static IValue Evaluate(RichExpression richExpr, VarMap vars, FunMap funs) {
			for (int i = 0; i < richExpr.VariableDefinitions.Length; i++) {
				vars = VariableDefinitionEvaluator.EvaluateAndAdd(richExpr.VariableDefinitions[i], vars, funs);
			}

			return ExpressionEvaluator.Evaluate(richExpr.Expression, vars, funs);
		}
	}
}

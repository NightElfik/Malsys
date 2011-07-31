using FunMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.FunctionDefinition>;
using VarMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.Expressions.IValue>;

namespace Malsys.Expressions {
	public static class RichExpressionEvaluator {

		public static IValue Evaluate(RichExpression richExpr, VarMap vars, FunMap funs) {
			foreach (var varDef in richExpr.VariableDefinitions) {
				vars = VariableDefinitionEvaluator.EvaluateAndAdd(varDef, vars, funs);
			}

			return ExpressionEvaluator.Evaluate(richExpr.Expression, vars, funs);
		}
	}
}

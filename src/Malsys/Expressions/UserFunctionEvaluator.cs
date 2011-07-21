using System;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;
using VarMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.Expressions.IValue>;
using FunMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.UserFunction>;

namespace Malsys.Expressions {
	public static class UserFunctionEvaluator {

		public static IValue Evaluate(UserFunction fun, VarMap variables, FunMap functions) {

			foreach (var varDef in fun.VariableDefinitions) {
				var varValue = ExpressionValueEvaluator.Evaluate(varDef.Value, variables, functions);
				variables = MapModule.Add(varDef.Name, varValue, variables);
			}

			return PostfixExpressionsEvaluator.Evaluate(fun.Expression, variables, functions);
		}
	}
}

using System.Diagnostics;
using Microsoft.FSharp.Collections;
using FunMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.FunctionDefinition>;
using VarMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.Expressions.IValue>;

namespace Malsys.Expressions {
	public static class UserFunctionEvaluator {

		public static IValue Evaluate(FunctionDefinition fun, ArgsStorage args, VarMap vars, FunMap funs) {

			Debug.Assert(fun.ParametersCount >= args.ArgsCount, "Too many arguments given to function.");
			Debug.Assert(args.ArgsCount + fun.OptionalParamsCount >= fun.ParametersCount, "Too few arguments given to function.");

			// eval parameters
			for (int i = 0; i < fun.ParametersCount; i++) {
				if (i < args.ArgsCount) {
					vars = MapModule.Add(fun.GetParamName(i), args[i], vars);
				}
				else {
					var optParamVal = ExpressionEvaluator.Evaluate(fun.GetOptionalParamValue(i), vars, funs);
					vars = MapModule.Add(fun.GetParamName(i), optParamVal, vars);
				}
			}

			// eval local variables
			for (int i = 0; i < fun.LocalVariableDefsCount; i++) {
				vars = VariableDefinitionEvaluator.Evaluate(fun.GetVariableDefinition(i), vars, funs);
			}

			return ExpressionEvaluator.Evaluate(fun.Expression, vars, funs);
		}
	}
}

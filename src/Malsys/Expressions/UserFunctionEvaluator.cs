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
				IValue value = i < args.ArgsCount ? args[i] : fun.GetOptionalParamValue(i);
				vars = MapModule.Add(fun.ParametersNames[i], value, vars);
			}

			return RichExpressionEvaluator.Evaluate(fun, vars, funs);
		}
	}
}

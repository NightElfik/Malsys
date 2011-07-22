using Microsoft.FSharp.Collections;
using FunMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.FunctionDefinition>;
using VarMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.Expressions.IValue>;

namespace Malsys.Expressions {
	public static class VariableDefinitionEvaluator {

		/// <summary>
		/// Evaluates given variable definition and adds it to returned variables map.
		/// </summary>
		public static VarMap Evaluate(VariableDefinition varDef, VarMap vars, FunMap funs) {

			var value = ExpressionEvaluator.Evaluate(varDef.Value, vars, funs);
			return MapModule.Add(varDef.Name, value, vars);
		}
	}
}

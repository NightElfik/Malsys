using Microsoft.FSharp.Collections;
using FunMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.FunctionDefinition>;
using VarMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.Expressions.IValue>;

namespace Malsys.Expressions {
	public static class VariableDefinitionEvaluator {

		/// <summary>
		/// Evaluates given variable definition and adds it to returned variables map.
		/// </summary>
		public static VarMap EvaluateAndAdd(this VariableDefinition<IExpression> varDef, VarMap vars, FunMap funs) {

			var var = Evaluate(varDef, vars, funs);
			return MapModule.Add(var.Name, var.Value, vars);
		}

		public static VariableDefinition<IValue> Evaluate(this VariableDefinition<IExpression> varDef) {

			var value = ExpressionEvaluator.Evaluate(varDef.Value);
			return new VariableDefinition<IValue>(varDef.Name, value);
		}

		public static VariableDefinition<IValue> Evaluate(this VariableDefinition<IExpression> varDef, VarMap vars, FunMap funs) {

			var value = ExpressionEvaluator.Evaluate(varDef.Value, vars, funs);
			return new VariableDefinition<IValue>(varDef.Name, value);
		}
	}
}

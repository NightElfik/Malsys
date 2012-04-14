using System.Collections.Generic;
using System.Linq;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Evaluated;
using Microsoft.FSharp.Collections;
using StringInt = System.Tuple<string, int>;

namespace Malsys.Evaluators {
	public class ExpressionEvaluatorContext : IExpressionEvaluatorContext {

		protected readonly IExpressionEvaluator exprEvaluator;


		protected readonly FSharpMap<string, VariableInfo> varMap;

		protected readonly FSharpMap<StringInt, FunctionInfo> funMap;



		public ExpressionEvaluatorContext() {

			exprEvaluator = Malsys.Evaluators.ExpressionEvaluator.Instance;
			varMap = MapModule.Empty<string, VariableInfo>();
			funMap = MapModule.Empty<StringInt, FunctionInfo>();

		}

		public ExpressionEvaluatorContext(IExpressionEvaluator expressionEvaluator) {

			exprEvaluator = expressionEvaluator;
			varMap = MapModule.Empty<string, VariableInfo>();
			funMap = MapModule.Empty<StringInt, FunctionInfo>();


		}

		private ExpressionEvaluatorContext(IExpressionEvaluator expressionEvaluator,
				FSharpMap<string, VariableInfo> variablesMap, FSharpMap<StringInt, FunctionInfo> functionsMap) {

			exprEvaluator = expressionEvaluator;
			varMap = variablesMap;
			funMap = functionsMap;

		}


		#region IExpressionEvaluatorContext Members

		public IExpressionEvaluator ExpressionEvaluator { get { return exprEvaluator; } }

		public IValue Evaluate(IExpression expr) {
			return exprEvaluator.Evaluate(expr, this);
		}

		public bool TryGetVariableValue(string name, out IValue value) {

			VariableInfo variable;
			if (varMap.TryGetValue(name, out variable)) {
				value = variable.ValueFunc();
				if (value == null) {
					throw new EvalException("Variable `{0}` was evaluated to null.".Fmt(name));
				}
				return true;
			}
			else {
				value = null;
				return false;
			}

		}

		public bool TryEvaluateFunction(string name, IValue[] args, out IValue value) {

			FunctionInfo fun;
			if (!funMap.TryGetValue(new StringInt(name, args.Length), out fun)) {
				if (!funMap.TryGetValue(new StringInt(name, FunctionInfo.AnyParamsCount), out fun)) {
					value = null;
					return false;
				}
			}

			for (int i = 0; i < args.Length; i++) {
				if (!args[i].Type.IsCompatibleWith(fun.GetParameterType(i))) {
					throw new EvalException("Failed to evaluate function `{0}`. As {1}. argument excepted {2}, but {3} was given."
						.Fmt(name, i + 1, fun.GetParameterType(i).ToTypeString(), args[i].Type.ToTypeString()));
				}
			}

			value = fun.FunctionBody(args, this);
			return true;

		}

		public IExpressionEvaluatorContext AddVariable(VariableInfo variable, bool rewrite = true) {

			if (rewrite || !varMap.ContainsKey(variable.Name)) {
				return new ExpressionEvaluatorContext(exprEvaluator, varMap.Add(variable.Name, variable), funMap);
			}
			else {
				return this;  // we do not want to rewrite and map already contains variable with the same name
			}

		}

		public IExpressionEvaluatorContext AddFunction(FunctionInfo function, bool rewrite = true) {

			var key = new StringInt(function.Name, function.ParamsCount);
			if (rewrite || !funMap.ContainsKey(key)) {
				return new ExpressionEvaluatorContext(exprEvaluator, varMap, funMap.Add(key, function));
			}
			else {
				return this;  // we do not want to rewrite and map already contains function with the same signature
			}

		}

		public IEnumerable<VariableInfo> GetAllStoredVariables() {
			return varMap.Select(kvp => kvp.Value);
		}

		public IEnumerable<FunctionInfo> GetAllStoredFunctions() {
			return funMap.Select(kvp => kvp.Value);
		}


		public IExpressionEvaluatorContext MergeWith(IExpressionEvaluatorContext other) {

			if (other is ExpressionEvaluatorContext) {
				// fast merging, we can use private members
				var otherEec = (ExpressionEvaluatorContext)other;
				return new ExpressionEvaluatorContext(exprEvaluator, varMap.AddRange(otherEec.varMap), funMap.AddRange(otherEec.funMap));
			}
			else {
				// slow merging, we can use only interface methods
				var vars = varMap;
				foreach (var variable in other.GetAllStoredVariables()) {
					vars = vars.Add(variable.Name, variable);
				}

				var funs = funMap;
				foreach (var function in other.GetAllStoredFunctions()) {
					funs = funs.Add(new StringInt(function.Name, function.ParamsCount), function);
				}

				return new ExpressionEvaluatorContext(exprEvaluator, vars, funs);
			}
		}

		#endregion


	}
}

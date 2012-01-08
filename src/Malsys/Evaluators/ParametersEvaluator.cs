using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Evaluated;
using ConstsMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Evaluated.IValue>;
using FunsMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Compiled.FunctionEvaledParams>;

namespace Malsys.Evaluators {
	internal class ParametersEvaluator : IParametersEvaluator {

		private IExpressionEvaluator exprEvaluator;


		public ParametersEvaluator(IExpressionEvaluator expressionEvaluator) {
			exprEvaluator = expressionEvaluator;
		}


		public ImmutableList<OptionalParameterEvaled> Evaluate(ImmutableList<OptionalParameter> optPrms, ConstsMap consts, FunsMap funs) {

			var result = new OptionalParameterEvaled[optPrms.Length];

			for (int i = 0; i < result.Length; i++) {
				var currParam = optPrms[i];

				if (currParam.IsOptional) {
					result[i] = new OptionalParameterEvaled(currParam.Name, exprEvaluator.Evaluate(currParam.DefaultValue, consts, funs));
				}
				else {
					result[i] = new OptionalParameterEvaled(currParam.Name);
				}
			}

			return new ImmutableList<OptionalParameterEvaled>(result, true);
		}
	}
}

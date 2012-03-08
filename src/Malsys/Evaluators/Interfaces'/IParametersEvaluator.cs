using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Evaluators {
	public interface IParametersEvaluator {

		ImmutableList<OptionalParameterEvaled> Evaluate(ImmutableList<OptionalParameter> optPrms, IExpressionEvaluatorContext exprEvalCtxt);

	}
}

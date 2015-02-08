using System.Collections.Generic;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Evaluators {
	public interface IParametersEvaluator {

		List<OptionalParameterEvaled> Evaluate(IEnumerable<OptionalParameter> optPrms, IExpressionEvaluatorContext exprEvalCtxt);

	}
}

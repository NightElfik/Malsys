using System.Collections.Generic;
// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Evaluators {
	public interface IParametersEvaluator {

		List<OptionalParameterEvaled> Evaluate(IEnumerable<OptionalParameter> optPrms, IExpressionEvaluatorContext exprEvalCtxt);

	}
}

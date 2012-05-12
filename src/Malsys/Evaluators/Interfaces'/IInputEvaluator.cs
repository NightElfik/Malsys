/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Evaluators {
	public interface IInputEvaluator {

		InputBlockEvaled Evaluate(SemanticModel.Compiled.InputBlock input, IExpressionEvaluatorContext exprEvalCtxt, IMessageLogger logger);

	}
}

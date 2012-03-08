using Malsys.SemanticModel.Evaluated;

namespace Malsys.Evaluators {
	public interface IInputEvaluator {

		InputBlockEvaled Evaluate(SemanticModel.Compiled.InputBlock input, IExpressionEvaluatorContext exprEvalCtxt);

	}
}

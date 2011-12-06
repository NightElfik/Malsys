using Malsys.SemanticModel.Evaluated;

namespace Malsys.Evaluators {
	public interface IInputEvaluator {

		InputBlock Evaluate(SemanticModel.Compiled.InputBlock input);

	}
}

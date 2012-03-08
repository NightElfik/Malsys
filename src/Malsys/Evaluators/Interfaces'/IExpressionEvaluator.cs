using Malsys.SemanticModel;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Evaluators {

	public interface IExpressionEvaluator {

		/// <summary>
		/// Evaluates given expression.
		/// </summary>
		/// <remarks>
		/// If this method is using given IExpressionEvaluatorContext for variables and functions evaluation,
		/// these functions will likely call Evaluate method on expression evaluator from IExpressionEvaluatorContext
		/// which can be the same instance as this. So this method must be able to handle its recursive calls.
		/// The best way how to avoid any problems is to do the method pure (state-less, no usage of global variables).
		/// This will also causes thread safety of the method.
		/// </remarks>
		IValue Evaluate(IExpression expr, IExpressionEvaluatorContext exprEvalCtxt);

	}

}

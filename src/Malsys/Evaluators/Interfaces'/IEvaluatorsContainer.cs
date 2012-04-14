using System.Collections.Generic;
using Malsys.SemanticModel.Evaluated;
using Malsys.SemanticModel.Compiled;

namespace Malsys.Evaluators {

	public interface IEvaluatorsContainer {

		IExpressionEvaluatorContext ExpressionEvaluatorContext { get; }

		T Resolve<T>();

		IInputEvaluator ResolveInputEvaluator();

		ILsystemEvaluator ResolveLsystemEvaluator();

	}

	public static class IEvaluatorsContainerExtensions {

		public static InputBlockEvaled EvaluateInput(this IEvaluatorsContainer container, InputBlock input, IMessageLogger logger) {
			return container.ResolveInputEvaluator().Evaluate(input, container.ExpressionEvaluatorContext, logger);
		}

		public static InputBlockEvaled EvaluateInput(this IEvaluatorsContainer container, InputBlock input,
			   IExpressionEvaluatorContext exprEvalCtxt, IMessageLogger logger) {

			return container.ResolveInputEvaluator().Evaluate(input, exprEvalCtxt, logger);
		}


	}

}

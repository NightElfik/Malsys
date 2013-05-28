using Malsys.SemanticModel.Compiled;
// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using Malsys.SemanticModel.Evaluated;

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

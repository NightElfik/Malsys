using System.Collections.Generic;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Evaluators {

	public interface IEvaluatorsContainer {

		IExpressionEvaluatorContext ExpressionEvaluatorContext { get; }

		T Resolve<T>();

		IInputEvaluator ResolveInputEvaluator();

		ILsystemEvaluator ResolveLsystemEvaluator();

	}

	public static class IEvaluatorsContainerExtensions {

		public static InputBlockEvaled EvaluateInput(this IEvaluatorsContainer container, SemanticModel.Compiled.InputBlock input) {
			return container.ResolveInputEvaluator().Evaluate(input, container.ExpressionEvaluatorContext);
		}

		public static InputBlockEvaled EvaluateInput(this IEvaluatorsContainer container, SemanticModel.Compiled.InputBlock input,
			   IExpressionEvaluatorContext exprEvalCtxt) {

			return container.ResolveInputEvaluator().Evaluate(input, exprEvalCtxt);
		}

		public static InputBlockEvaled TryEvaluateInput(this IEvaluatorsContainer container, SemanticModel.Compiled.InputBlock input,
				IExpressionEvaluatorContext exprEvalCtxt, IMessageLogger logger) {

			try {
				return container.ResolveInputEvaluator().Evaluate(input, exprEvalCtxt);
			}
			catch (EvalException ex) {
				logger.LogMessage(Message.EvalFailed, ex.GetFullMessage());
				return null;
			}
		}

		public static LsystemEvaled EvaluateLsystem(this IEvaluatorsContainer container, SemanticModel.Compiled.LsystemEvaledParams input,
				IList<IValue> arguments, IExpressionEvaluatorContext exprEvalCtxt) {

			return container.ResolveLsystemEvaluator().Evaluate(input, arguments, exprEvalCtxt);
		}


		public enum Message {

			[Message(MessageType.Error, "Evaluation failed. {0}")]
			EvalFailed,

		}

	}

}

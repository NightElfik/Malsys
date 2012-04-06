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

		public static InputBlockEvaled EvaluateInput(this IEvaluatorsContainer container, InputBlock input) {
			return container.ResolveInputEvaluator().Evaluate(input, container.ExpressionEvaluatorContext);
		}

		public static InputBlockEvaled EvaluateInput(this IEvaluatorsContainer container, InputBlock input,
			   IExpressionEvaluatorContext exprEvalCtxt) {

			return container.ResolveInputEvaluator().Evaluate(input, exprEvalCtxt);
		}

		public static InputBlockEvaled TryEvaluateInput(this IEvaluatorsContainer container, InputBlock input,
				IExpressionEvaluatorContext exprEvalCtxt, IMessageLogger logger) {

			try {
				return container.ResolveInputEvaluator().Evaluate(input, exprEvalCtxt);
			}
			catch (EvalException ex) {
				logger.LogMessage(Message.LsystemEvalFailed, input.SourceName, ex.GetFullMessage());
				return null;
			}
		}

		public static LsystemEvaled TryEvaluateLsystem(this IEvaluatorsContainer container, LsystemEvaledParams input, IList<IValue> arguments,
				IExpressionEvaluatorContext exprEvalCtxt, IBaseLsystemResolver baseResolver, IMessageLogger logger) {

			try {
				using (var errBlock = logger.StartErrorLoggingBlock()) {
					var result = container.ResolveLsystemEvaluator().Evaluate(input, arguments, exprEvalCtxt, baseResolver, logger);
					if (errBlock.ErrorOccurred) {
						return null;
					}
					return result;
				}
			}
			catch (EvalException ex) {
				logger.LogMessage(Message.LsystemEvalFailed, input.Name, ex.GetFullMessage());
				return null;
			}

		}


		public enum Message {

			[Message(MessageType.Error, "Evaluation of input `{0}` failed. {1}")]
			InputEvalFailed,

			[Message(MessageType.Error, "Evaluation of L-system `{0}` failed. {1}")]
			LsystemEvalFailed,

		}

	}

}

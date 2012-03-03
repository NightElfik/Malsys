using System.Collections.Generic;
using Malsys.SemanticModel.Evaluated;
using ConstsMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Evaluated.IValue>;
using FunsMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Compiled.FunctionEvaledParams>;

namespace Malsys.Evaluators {
	public interface IEvaluatorsContainer {

		T Resolve<T>();

		IInputEvaluator ResolveInputEvaluator();

		ILsystemEvaluator ResolveLsystemEvaluator();

		IExpressionEvaluator ResolveExpressionEvaluator();

	}

	public static class IEvaluatorsContainerExtensions {

		public static InputBlock EvaluateInput(this IEvaluatorsContainer container, SemanticModel.Compiled.InputBlock input) {
			return container.ResolveInputEvaluator().Evaluate(input);
		}

		public static InputBlock TryEvaluateInput(this IEvaluatorsContainer container, SemanticModel.Compiled.InputBlock input, IMessageLogger logger) {
			try {
				return container.ResolveInputEvaluator().Evaluate(input);
			}
			catch (EvalException ex) {
				logger.LogMessage(Message.EvalFailed, ex.GetFullMessage());
				return null;
			}
		}

		public static LsystemEvaled EvaluateLsystem(this IEvaluatorsContainer container, SemanticModel.Compiled.LsystemEvaledParams input,
				IList<IValue> arguments, ConstsMap consts, FunsMap funs) {
			return container.ResolveLsystemEvaluator().Evaluate(input, arguments, consts, funs);
		}

		public static IValue EvaluateExpression(this IEvaluatorsContainer container, SemanticModel.Compiled.IExpression expression) {
			return container.ResolveExpressionEvaluator().Evaluate(expression);
		}

		public enum Message {

			[Message(MessageType.Error, "Evaluation failed. {0}")]
			EvalFailed,

		}

	}
}

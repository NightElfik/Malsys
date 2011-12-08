using Autofac;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Evaluators {
	/// <summary>
	/// All evaluators in this container have single-instance lifetime.
	/// </summary>
	public class EvaluatorsContainer {

		protected IContainer container;


		public EvaluatorsContainer() {

			var builder = new ContainerBuilder();

			builder.RegisterType<InputEvaluator>().As<IInputEvaluator>().SingleInstance();
			builder.RegisterType<LsystemEvaluator>().As<ILsystemEvaluator>().SingleInstance();
			builder.RegisterType<SymbolEvaluator>().As<ISymbolEvaluator>().SingleInstance();
			builder.RegisterType<ParametersEvaluator>().As<IParametersEvaluator>().SingleInstance();
			builder.RegisterType<ExpressionEvaluator>().As<IExpressionEvaluator>().SingleInstance();

			container = builder.Build();
		}


		/// <summary>
		/// Can be used to resolve other evaluators.
		/// </summary>
		public virtual T Resolve<T>() {
			return container.Resolve<T>();
		}

		public IInputEvaluator ResolveInputEvaluator() {
			return container.Resolve<IInputEvaluator>();
		}

		public IExpressionEvaluator ResolveExpressionEvaluator() {
			return container.Resolve<IExpressionEvaluator>();
		}

	}


	public static class EvaluatorsContainerExtensions {

		public static InputBlock EvaluateInput(this EvaluatorsContainer container, SemanticModel.Compiled.InputBlock input) {
			return container.ResolveInputEvaluator().Evaluate(input);
		}

		public static IValue EvaluateExpression(this EvaluatorsContainer container, SemanticModel.Compiled.IExpression input) {
			return container.ResolveExpressionEvaluator().Evaluate(input);
		}

	}
}

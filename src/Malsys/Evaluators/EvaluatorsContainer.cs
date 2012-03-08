using Autofac;

namespace Malsys.Evaluators {
	/// <summary>
	/// All evaluators in this container have single-instance lifetime.
	/// </summary>
	public class EvaluatorsContainer : IEvaluatorsContainer {


		protected IContainer container;


		public EvaluatorsContainer(IExpressionEvaluatorContext exprEvalCtxt) {

			ExpressionEvaluatorContext = exprEvalCtxt;

			var builder = new ContainerBuilder();

			builder.Register(x => exprEvalCtxt).As<IExpressionEvaluatorContext>().SingleInstance();

			builder.RegisterType<InputEvaluator>().As<IInputEvaluator>().SingleInstance();
			builder.RegisterType<LsystemEvaluator>().As<ILsystemEvaluator>().SingleInstance();
			builder.RegisterType<SymbolEvaluator>().As<ISymbolEvaluator>().SingleInstance();
			builder.RegisterType<ParametersEvaluator>().As<IParametersEvaluator>().SingleInstance();

			container = builder.Build();
		}


		public IExpressionEvaluatorContext ExpressionEvaluatorContext { get; private set; }


		/// <summary>
		/// Can be used to resolve other evaluators.
		/// </summary>
		public T Resolve<T>() {
			return container.Resolve<T>();
		}

		public IInputEvaluator ResolveInputEvaluator() {
			return container.Resolve<IInputEvaluator>();
		}

		public ILsystemEvaluator ResolveLsystemEvaluator() {
			return container.Resolve<ILsystemEvaluator>();
		}

	}


}

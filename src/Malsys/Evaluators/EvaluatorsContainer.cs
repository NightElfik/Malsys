/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using Autofac;

namespace Malsys.Evaluators {
	/// <summary>
	/// IoC container for evaluators.
	/// To extend it you can inherit this class and override the Resolve{T} method
	/// or you can implement new implementation if the IEvaluatorsContainer interface.
	/// </summary>
	/// <remarks>
	/// All evaluators in this container have single-instance lifetime.
	/// </remarks>
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
		public virtual T Resolve<T>() {
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

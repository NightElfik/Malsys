using System.Collections.Generic;
using Autofac;
using Malsys.SemanticModel.Evaluated;
using ConstsMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Evaluated.IValue>;
using FunsMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Compiled.FunctionEvaledParams>;

namespace Malsys.Evaluators {
	/// <summary>
	/// All evaluators in this container have single-instance lifetime.
	/// </summary>
	public class EvaluatorsContainer : IEvaluatorsContainer {

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
		public T Resolve<T>() {
			return container.Resolve<T>();
		}

		public IInputEvaluator ResolveInputEvaluator() {
			return container.Resolve<IInputEvaluator>();
		}

		public ILsystemEvaluator ResolveLsystemEvaluator() {
			return container.Resolve<ILsystemEvaluator>();
		}

		public IExpressionEvaluator ResolveExpressionEvaluator() {
			return container.Resolve<IExpressionEvaluator>();
		}

	}


}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Evaluators {
	public class MalsysEvaluator {

		private IContainer container;


		public MalsysEvaluator() {

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

		public IExpressionEvaluator ResolveExpressionEvaluator() {
			return container.Resolve<IExpressionEvaluator>();
		}


		public InputBlock Evaluate(SemanticModel.Compiled.InputBlock input) {
			return ResolveInputEvaluator().Evaluate(input);
		}

	}
}

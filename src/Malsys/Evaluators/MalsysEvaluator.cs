using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;

namespace Malsys.Evaluators {
	public class MalsysEvaluator {


		private IContainer container;


		public MalsysEvaluator() {

			var builder = new ContainerBuilder();

			builder.RegisterType<InputEvaluator>().As<IInputEvaluator>().InstancePerLifetimeScope();
			builder.RegisterType<LsystemEvaluator>().As<ILsystemEvaluator>().InstancePerLifetimeScope();
			builder.RegisterType<SymbolEvaluator>().As<ISymbolEvaluator>().InstancePerLifetimeScope();
			builder.RegisterType<ParametersEvaluator>().As<IParametersEvaluator>().InstancePerLifetimeScope();
			builder.RegisterType<ExpressionEvaluator>().As<IExpressionEvaluator>().InstancePerLifetimeScope();

			container = builder.Build();
		}

		public MalsysEvaluator(IContainer cont) {
			container = cont;
		}


		public T Resolve<T>() {
			return container.Resolve<T>();
		}



	}
}

using Autofac;
using Malsys.Resources;

namespace Malsys.Compilers {
	/// <summary>
	/// All compilers in this container have single-instance lifetime.
	/// </summary>
	public class CompilersContainer : ICompilersContainer {

		private static readonly KnownConstOpProvider defaultKnownStuffProvider;


		static CompilersContainer() {

			defaultKnownStuffProvider = new KnownConstOpProvider();
			defaultKnownStuffProvider.LoadConstants(typeof(StdConstants));
			defaultKnownStuffProvider.LoadOperators(typeof(StdOperators));

		}


		protected IContainer container;



		/// <summary>
		/// Creates container with standard constants and operators.
		/// </summary>
		internal CompilersContainer() {
			buildContainer(defaultKnownStuffProvider, defaultKnownStuffProvider);
		}

		public CompilersContainer(IKnownConstantsProvider knownConstantsProvider, IKnownOperatorsProvider knownOperatorsProvider) {
			buildContainer(knownConstantsProvider, knownOperatorsProvider);
		}

		/// <summary>
		/// Can be used to resolve other sub-compilers.
		/// </summary>
		public virtual T Resolve<T>() {
			return container.Resolve<T>();
		}

		public IInputCompiler ResolveInputCompiler() {
			return container.Resolve<IInputCompiler>();
		}

		public IExpressionCompiler ResolveExpressionCompiler() {
			return container.Resolve<IExpressionCompiler>();
		}


		private void buildContainer(IKnownConstantsProvider knownConstantsProvider, IKnownOperatorsProvider knownOperatorsProvider) {

			var builder = new ContainerBuilder();

			builder.Register(x => knownConstantsProvider).As<IKnownConstantsProvider>().SingleInstance();
			builder.Register(x => knownOperatorsProvider).As<IKnownOperatorsProvider>().SingleInstance();

			builder.RegisterType<InputCompiler>().As<IInputCompiler>().SingleInstance();
			builder.RegisterType<ConstantDefCompiler>().As<IConstantDefinitionCompiler>().SingleInstance();
			builder.RegisterType<FunctionDefCompiler>().As<IFunctionDefinitionCompiler>().SingleInstance();
			builder.RegisterType<LsystemCompiler>().As<ILsystemCompiler>().SingleInstance();
			builder.RegisterType<ParametersCompiler>().As<IParametersCompiler>().SingleInstance();
			builder.RegisterType<RewriteRuleCompiler>().As<IRewriteRuleCompiler>().SingleInstance();
			builder.RegisterType<SymbolsCompiler>().As<ISymbolCompiler>().SingleInstance();
			builder.RegisterType<ProcessStatementsCompiler>().As<IProcessStatementsCompiler>().SingleInstance();
			builder.RegisterType<ExpressionCompiler>().As<IExpressionCompiler>().SingleInstance();

			container = builder.Build();

		}
	}


}

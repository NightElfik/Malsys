using Autofac;

namespace Malsys.Compilers {
	/// <summary>
	/// IoC container for compilers.
	/// To extend it you can inherit this class and override the Resolve{T} method
	/// or you can implement new implementation if the ICompilersContainer interface.
	/// </summary>
	/// <remarks>
	/// All compilers in this container have single-instance lifetime.
	/// </remarks>
	public class CompilersContainer : ICompilersContainer {


		protected IContainer container;


		public CompilersContainer(ICompilerConstantsProvider knownConstantsProvider, IOperatorsProvider knownOperatorsProvider) {
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


		private void buildContainer(ICompilerConstantsProvider knownConstantsProvider, IOperatorsProvider knownOperatorsProvider) {

			var builder = new ContainerBuilder();

			builder.Register(x => knownConstantsProvider).As<ICompilerConstantsProvider>().SingleInstance();
			builder.Register(x => knownOperatorsProvider).As<IOperatorsProvider>().SingleInstance();

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

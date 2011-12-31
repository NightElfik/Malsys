using System;
using Autofac;
using Malsys.Compilers.Expressions;
using Malsys.Parsing;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Compiled.Expressions;
using Microsoft.FSharp.Text.Lexing;
using System.IO;
using Malsys.Resources;

namespace Malsys.Compilers {
	/// <summary>
	/// All compilers in this container have single-instance lifetime.
	/// </summary>
	public class CompilersContainer {

		private static readonly KnownConstFunOpProvider defaultKnownStuffProvider;


		static CompilersContainer() {

			defaultKnownStuffProvider = new KnownConstFunOpProvider();
			defaultKnownStuffProvider.LoadFromType(typeof(StdConstants));
			defaultKnownStuffProvider.LoadFromType(typeof(StdFunctions));
			defaultKnownStuffProvider.LoadFromType(typeof(StdOperators));

		}


		protected IContainer container;



		/// <summary>
		/// Creates container with default constants, functions and operators.
		/// </summary>
		internal CompilersContainer() {
			buildContainer(defaultKnownStuffProvider, defaultKnownStuffProvider, defaultKnownStuffProvider);
		}

		public CompilersContainer(IKnownConstantsProvider knownConstantsProvider, IKnownFunctionsProvider knownFunctionsProvider,
				IKnownOperatorsProvider knownOperatorsProvider) {

			buildContainer(knownConstantsProvider, knownFunctionsProvider, knownOperatorsProvider);
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


		private void buildContainer(IKnownConstantsProvider knownConstantsProvider, IKnownFunctionsProvider knownFunctionsProvider,
				IKnownOperatorsProvider knownOperatorsProvider) {

			var builder = new ContainerBuilder();

			builder.Register(x => knownConstantsProvider).As<IKnownConstantsProvider>().SingleInstance();
			builder.Register(x => knownFunctionsProvider).As<IKnownFunctionsProvider>().SingleInstance();
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


	public static class CompilersContainerExtensions {

		public static InputBlock CompileInput(this CompilersContainer container, LexBuffer<char> lexBuff, string sourceName, IMessageLogger logger) {

			Ast.InputBlock parsedInput;

			try {
				parsedInput = ParserUtils.ParseInputNoComents(lexBuff, logger, sourceName);
			}
			catch (Exception ex) {
				logger.LogMessage(Message.ParsingFailed, ex.Message);
				return new InputBlock(sourceName, ImmutableList<IInputStatement>.Empty);
			}

			return container.ResolveInputCompiler().Compile(parsedInput, logger);
		}

		public static InputBlock CompileInput(this CompilersContainer container, string strInput, string sourceName, IMessageLogger logger) {

			return CompileInput(container, LexBuffer<char>.FromString(strInput), sourceName, logger);

		}

		public static InputBlock CompileInput(this CompilersContainer container, TextReader inputReader, string sourceName, IMessageLogger logger) {

			return CompileInput(container, LexBuffer<char>.FromTextReader(inputReader), sourceName, logger);

		}

		public static IExpression CompileExpression(this CompilersContainer container, string strInput, string sourceName, IMessageLogger logger) {

			var lexBuff = LexBuffer<char>.FromString(strInput);

			Ast.Expression parsedInput;

			try {
				parsedInput = ParserUtils.ParseExpression(lexBuff, logger, sourceName);
			}
			catch (Exception ex) {
				logger.LogMessage(Message.ParsingFailed, ex.Message);
				return EmptyExpression.Instance;
			}

			return container.ResolveExpressionCompiler().Compile(parsedInput, logger);
		}



		public enum Message {

			[Message(MessageType.Error, "Parsing failed. {0}")]
			ParsingFailed,

			[Message(MessageType.Info, "{0}")]
			CompilationTime,

		}
	}
}

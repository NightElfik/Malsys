using System;
using System.Collections.Generic;
using System.Diagnostics;
using Autofac;
using Malsys.Compilers.Expressions;
using Malsys.Parsing;
using Malsys.SemanticModel.Compiled;
using Microsoft.FSharp.Text.Lexing;

namespace Malsys.Compilers {
	public class MalsysCompiler {

		private IContainer container;


		public MalsysCompiler() {

			var builder = new ContainerBuilder();

			var knownStuffProvider = new KnownConstFunOpProvider();
			knownStuffProvider.LoadFromType(typeof(KnownConstant));
			knownStuffProvider.LoadFromType(typeof(FunctionCore));
			knownStuffProvider.LoadFromType(typeof(OperatorCore));
			builder.Register(x => knownStuffProvider).As<IKnownConstantsProvider>().SingleInstance();
			builder.Register(x => knownStuffProvider).As<IKnownFunctionsProvider>().SingleInstance();
			builder.Register(x => knownStuffProvider).As<IKnownOperatorsProvider>().SingleInstance();

			builder.RegisterType<InputCompiler>().As<IInputCompiler>().SingleInstance();
			builder.RegisterType<ConstantDefCompiler>().As<IConstantDefinitionCompiler>().SingleInstance();
			builder.RegisterType<FunctionDefCompiler>().As<IFunctionDefinitionCompiler>().SingleInstance();
			builder.RegisterType<LsystemCompiler>().As<ILsystemCompiler>().SingleInstance();
			builder.RegisterType<ParametersCompiler>().As<IParametersCompiler>().SingleInstance();
			builder.RegisterType<RewriteRuleCompiler>().As<IRewriteRuleCompiler>().SingleInstance();
			builder.RegisterType<SymbolsCompiler>().As<ISymbolCompiler>().SingleInstance();
			builder.RegisterType<ExpressionCompiler>().As<IExpressionCompiler>().SingleInstance();

			container = builder.Build();
		}

		/// <summary>
		/// Can be used to resolve other sub-compilers like expression compiler.
		/// </summary>
		public T Resolve<T>() {
			return container.Resolve<T>();
		}

		public IInputCompiler ResolveInputCompiler() {
			return container.Resolve<IInputCompiler>();
		}


		public InputBlock CompileFromString(string strInput, string sourceName, IMessageLogger logger) {

			var lexBuff = LexBuffer<char>.FromString(strInput);

			Ast.InputBlock parsedInput;

			try {
				parsedInput = ParserUtils.ParseInputNoComents(lexBuff, logger, sourceName);
			}
			catch (Exception ex) {
				logger.LogMessage(Message.ParsingFailed, ex.Message);
				return new InputBlock(sourceName, ImmutableList<IInputStatement>.Empty);
			}

			return CompileFromAst(parsedInput, logger);
		}

		public InputBlock CompileFromAst(Ast.InputBlock parsedInput, IMessageLogger logger) {

			var sw = new Stopwatch();
			sw.Start();

			InputBlock result = ResolveInputCompiler().Compile(parsedInput, logger);

			sw.Stop();
			logger.LogMessage(Message.CompilationTime, sw.Elapsed.ToString());

			return result;
		}

		public enum Message {

			[Message(MessageType.Error, "Parsing failed. {0}")]
			ParsingFailed,

			[Message(MessageType.Info, "{0}")]
			CompilationTime,

		}

	}
}

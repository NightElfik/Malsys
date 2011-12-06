using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Malsys.SemanticModel.Compiled;
using Microsoft.FSharp.Text.Lexing;
using Malsys.Parsing;
using Autofac;
using Malsys.Compilers.Expressions;
using System.Diagnostics;

namespace Malsys.Compilers {
	public class MalsysCompiler {

		private IContainer container;


		public MalsysCompiler(IContainer cont) {
			container = cont;
		}

		public MalsysCompiler(MessageLogger messageLogger) {

			var builder = new ContainerBuilder();

			var knownStuffProvider = new KnownConstFunOpProvider();
			knownStuffProvider.LoadFromType(typeof(KnownConstant));
			knownStuffProvider.LoadFromType(typeof(FunctionCore));
			knownStuffProvider.LoadFromType(typeof(OperatorCore));

			builder.Register(x => messageLogger).As<MessageLogger>().SingleInstance();

			builder.Register(x => knownStuffProvider).As<IKnownConstantsProvider>().SingleInstance();

			builder.RegisterType<InputCompiler>().As<IInputCompiler>().InstancePerLifetimeScope();
			builder.RegisterType<ConstantDefCompiler>().As<IConstantDefinitionCompiler>().InstancePerLifetimeScope();
			builder.RegisterType<FunctionDefCompiler>().As<IFunctionDefinitionCompiler>().InstancePerLifetimeScope();
			builder.RegisterType<LsystemCompiler>().As<ILsystemCompiler>().InstancePerLifetimeScope();
			builder.RegisterType<ParametersCompiler>().As<IParametersCompiler>().InstancePerLifetimeScope();
			builder.RegisterType<RewriteRuleCompiler>().As<IRewriteRuleCompiler>().InstancePerLifetimeScope();
			builder.RegisterType<SymbolsCompiler>().As<ISymbolCompiler>().InstancePerLifetimeScope();
			builder.RegisterType<ExpressionCompiler>().As<IExpressionCompiler>().InstancePerLifetimeScope();

			container = builder.Build();
		}


		public T Resolve<T>() {
			return container.Resolve<T>();
		}

		public IInputCompiler ResolveInputCompiler() {
			return container.Resolve<IInputCompiler>();
		}

		public MessageLogger ResolveMessageLogger() {
			return container.Resolve<MessageLogger>();
		}


		public InputBlock CompileFromString(string strInput, string sourceName) {

			var logger = ResolveMessageLogger();

			var lexBuff = LexBuffer<char>.FromString(strInput);
			logger.DefaultSourceName = sourceName;
			var comments = new List<Ast.Comment>();

			Ast.InputBlock parsedInput;

			try {
				parsedInput = ParserUtils.ParseInput(comments, lexBuff, logger, sourceName);
			}
			catch (Exception ex) {
				logger.LogMessage(Message.ParsingFailed, Position.Unknown);
				return new InputBlock(sourceName, ImmutableList<IInputStatement>.Empty);
			}

			return CompileFromAst(parsedInput);
		}

		public InputBlock CompileFromAst(Ast.InputBlock parsedInput) {

			var sw = new Stopwatch();
			sw.Start();

			InputBlock result;

			using (var lifetime = container.BeginLifetimeScope()) {
				var compiler = ResolveInputCompiler();

				result = compiler.Compile(parsedInput);
			}

			sw.Stop();
			var logger = ResolveMessageLogger();
			logger.LogMessage<MalsysCompiler>("CompilationTime", MessageType.Info, sw.Elapsed.ToString());

			return result;
		}

		public enum Message {
			[Message(MessageType.Error, "Parsing failed. {0}")]
			ParsingFailed
		}

	}
}

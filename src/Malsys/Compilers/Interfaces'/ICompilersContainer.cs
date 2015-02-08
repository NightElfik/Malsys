using System;
using System.Collections.Generic;
using System.IO;
using Malsys.Parsing;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Compiled.Expressions;
using Microsoft.FSharp.Text.Lexing;

namespace Malsys.Compilers {
	public interface ICompilersContainer {

		/// <summary>
		/// Can be used to resolve other sub-compilers.
		/// </summary>
		T Resolve<T>();

		IInputCompiler ResolveInputCompiler();

		IExpressionCompiler ResolveExpressionCompiler();

	}


	public static class ICompilersContainerExtensions {

		public static InputBlock CompileInput(this ICompilersContainer container, LexBuffer<char> lexBuff, string sourceName, IMessageLogger logger) {

			Ast.InputBlock parsedInput;

			try {
				parsedInput = ParserUtils.ParseInputNoComents(lexBuff, logger, sourceName);
			}
			catch (Exception ex) {
				logger.LogMessage(Message.ParsingFailed, ex.Message);
				return new InputBlock(null) {
					SourceName = sourceName,
					Statements = new List<IInputStatement>(),
				};
			}

			return container.ResolveInputCompiler().Compile(parsedInput, logger);
		}

		public static InputBlock CompileInput(this ICompilersContainer container, string strInput, string sourceName, IMessageLogger logger) {

			return CompileInput(container, LexBuffer<char>.FromString(strInput), sourceName, logger);

		}

		public static InputBlock CompileInput(this ICompilersContainer container, TextReader inputReader, string sourceName, IMessageLogger logger) {

			return CompileInput(container, LexBuffer<char>.FromTextReader(inputReader), sourceName, logger);

		}

		public static IExpression CompileExpression(this ICompilersContainer container, string strInput, string sourceName, IMessageLogger logger) {

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

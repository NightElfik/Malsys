using System;
using System.IO;
using Malsys.Parsing;
using Microsoft.FSharp.Text.Lexing;
using Malsys.Ast;

namespace Malsys.Compilers {

	public static class LsystemCompiler {

		public static Result CompileFromFile(string filePath, Parameters parameters) {
			using (StreamReader reader = new StreamReader(filePath)) {
				return CompileFromTextReader(reader, filePath, parameters);
			}
		}

		public static Result CompileFromTextReader(TextReader reader, string sourceName, Parameters parameters) {
			var lexBuff = LexBuffer<char>.FromTextReader(reader);
			var rslt = new Result();

			compile(lexBuff, parameters, rslt);

			return rslt;
		}

		public static Result CompileFromString(string str, string sourceName, Parameters parameters) {
			var lexBuff = LexBuffer<char>.FromString(str);
			var rslt = new Result();

			compile(lexBuff, parameters, rslt);

			return rslt;
		}


		private static void compile(LexBuffer<char> lexBuff, Parameters parameters, Result result) {
			try {
				//var ast = ParserUtils.parse(lexBuff);
			}
			catch (LexerException ex) {
				result.Messages.AddMessage(new CompilerMessage("Failed to parse input: " + ex.LexerMessage, CompilerMessageType.Error,
					ex.BeginPosition.FileName, new Ast.Position(ex.BeginPosition, ex.EndPosition)));
				return;
			}
			catch (ParserException<char> ex) {
				result.Messages.AddMessage(new CompilerMessage("Failed to parse input: " + ex.ParserMessage, CompilerMessageType.Error,
					ex.ErrorContext.ParseState.ResultRange.Item1.FileName, new Ast.Position(ex.ErrorContext.ParseState.ResultRange)));
				return;
			}
			catch (Exception ex) {
				result.Messages.AddMessage(new CompilerMessage("Failed to parse input: " + ex.Message, CompilerMessageType.Error,
					lexBuff.EndPos.FileName, Ast.Position.Unknown));
			}


		}

		private static void compileLsystem(Lsystem lSystemAst, Parameters parameters, Result result) {

		}



		public class Parameters {

		}

		public class Result {
			public MessagesCollection Messages { get; private set; }

			public Result() {
				Messages = new MessagesCollection();
			}
		}
	}
}

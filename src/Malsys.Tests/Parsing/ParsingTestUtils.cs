using System.Collections.Generic;
using Malsys.Compilers;
using Malsys.IO;
using Malsys.Parsing;
using Malsys.SourceCode.Printers;
using Microsoft.FSharp.Text.Lexing;
using System;

namespace Malsys.Tests.Parsing {
	class ParsingTestUtils {
		public static string ParseLsystemAndCanonicPrintAst(string input) {

			var lexBuff = LexBuffer<char>.FromString(input);
			var msgs = new MessagesCollection();
			var comments = new List<Ast.Comment>();
			var lsys = ParserUtils.ParseLsystemStatements(comments, lexBuff, msgs, "testInput");

			if (msgs.ErrorOcured) {
				throw new Exception(msgs.ToString());
			}

			var writer = new IndentStringWriter();
			var printer = new CanonicAstPrinter(writer);
			printer.Visit(lsys);

			return writer.GetResult();
		}
	}
}

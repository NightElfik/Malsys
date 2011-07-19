using System;
using Microsoft.FSharp.Text.Lexing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Malsys.Parsing;

namespace Malsys.Tests.Parsing {
	/// <summary>
	/// Thiese tests do not test anything concrete, they are just for debuging of parser.
	/// </summary>
	[TestClass]
	public class DebugTests {
		[TestMethod]
		public void ParserDebugTest() {
			string testInput = @"
lsystem LsysName {
	//< lctx(t) < A(_,n,a,m,e) > r(c)t(x,t) > ?{let a = 10; a*a^b-c} :{let b = x; n} -> r(a*b,c*d+4)e(f)PLAc;
	lc txt < a > rctxt -> replac;
	//let a = 1+2^-f(a,b,c);
}";
			var lexBuff = LexBuffer<char>.FromString(testInput);
			var lsys = ParserUtils.parse(lexBuff, "testInput");

		}

		[TestMethod]
		public void LexerDebugTest() {
			string testInput = @"
lsystem LsysName {
	let a = 1+2^-f(a,b,c);
}";
			Console.WriteLine(testInput);
			Console.WriteLine();

			var lexbuff = LexBuffer<char>.FromString(testInput);

			Parser.token tok;
			do {
				tok = Lexer.tokenize(lexbuff);
				Console.Write(tok.Tag);
				Console.Write("\t");
				Console.Write(Parser.token_to_string(tok));
				Console.WriteLine();
			} while (tok != Parser.token.EOF);
		}
	}
}

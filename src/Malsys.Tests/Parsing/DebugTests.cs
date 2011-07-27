using System;
using Malsys.Parsing;
using Microsoft.FSharp.Text.Lexing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
	/*{ lctx(t) } A(_,n,a,m,e) { r(c)t(x,t) } ?{let a = 10;let b = b; a*a^b-c} :{let b = x; n} ->
		{let r = pi; let e = e*e } r(a*b,c*d+4)e(f)PLAc;*/

	let a = 1+2^-f(a,b,c);
	let pi = 3.14159;
	let τ = {{a},2,{}};
}";
			var lexBuff = LexBuffer<char>.FromString(testInput);
			var lsys = ParserUtils.parseLsystemStatements(lexBuff, "testInput");

		}

		[TestMethod]
		public void LexerDebugTest() {
			string testInput = @"
lsystem Unikód {
	let pi = 3.14159;
	//{ lctx(t) } A(_,n,a,m,e) { r(c)t(x,t) } ?{let a = 10; a*a^b-c} :{let b = x; n} -> r(a*b,c*d+4)e(f)PLAc;
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

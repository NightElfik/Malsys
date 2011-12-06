using System;
using Malsys.IO;
using Malsys.Parsing;
using Malsys.SourceCode.Printers;
using Microsoft.FSharp.Text.Lexing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Malsys.Tests.Parsing {
	[TestClass]
	public class FunctionDefinitionTests {

		[TestMethod]
		public void FunNameTests() {
			doTestAutoident("fun f() {", "return 0;", "}");
			doTestAutoident("fun f'() {", "return 0;", "}");
			doTestAutoident("fun function() {", "return 0;", "}");
			doTestAutoident("fun _() {", "return 0;", "}");
			doTestAutoident("fun _f() {", "return 0;", "}");
			doTestAutoident("fun f_() {", "return 0;", "}");
			doTestAutoident("fun {0}() {{".Fmt(CharHelper.Pi), "return 0;", "}");
		}

		[TestMethod]
		public void FunParamsTests() {
			doTestAutoident("fun f(x) {", "return 0;", "}");
			doTestAutoident("fun f(x, y) {", "return 0;", "}");
			doTestAutoident("fun f(x, _, z) {", "return 0;", "}");
			doTestAutoident("fun f(x'') {", "return 0;", "}");
			doTestAutoident("fun f(x = 0) {", "return 0;", "}");
			doTestAutoident("fun f(x = 0, y = 0) {", "return 0;", "}");
			doTestAutoident("fun f(y = {}) {", "return 0;", "}");
			doTestAutoident("fun f(y = { - 1, 1}) {", "return 0;", "}");
			doTestAutoident("fun f(a, b, c = d) {", "return 0;", "}");
		}

		[TestMethod]
		public void FunReturnValueTests() {
			doTestAutoident("fun f() {", "return 1 + 1;", "}");
			doTestAutoident("fun f() {", "return {};", "}");
			doTestAutoident("fun f() {", "return {{{}}};", "}");
			doTestAutoident("fun f() {", "return {log(e),  - 1}[1];", "}");
		}

		[TestMethod]
		public void FunLoalVariableTests() {
			doTestAutoident("fun f() {", "let x = 0;", "return 0;", "}");
			doTestAutoident("fun f() {", "let x = 0;", "let y = 0;", "return 0;", "}");
		}

		[TestMethod]
		public void WhitespaceIndependencyTests() {
			doTestAutoidentOutput("fun f(){return 0;}", "fun f() {", "return 0;", "}");
			doTestAutoidentOutput("fun f(){let x=0;return 0;}", "fun f() {", "let x = 0;", "return 0;", "}");
			doTestAutoidentOutput("\n\nfun\tf(){let\tx=0;\n\nreturn\t0\n\t\n;}", "fun f() {", "let x = 0;", "return 0;", "}");
		}


		private void doTestAutoident(params string[] inputLines) {
			for (int i = 1; i < inputLines.Length - 1; i++) {
				inputLines[i] = "\t" + inputLines[i];
			}

			string input = string.Join(Environment.NewLine, inputLines);
			doTest(input, input);
		}

		private void doTestAutoidentOutput(string input, params string[] outputLines) {
			for (int i = 1; i < outputLines.Length - 1; i++) {
				outputLines[i] = "\t" + outputLines[i];
			}

			string output = string.Join(Environment.NewLine, outputLines);
			doTest(input, output);
		}

		private void doTest(string input, string excpected) {

			var lexBuff = LexBuffer<char>.FromString(input);
			var msgs = new MessageLogger();
			var inBlock = ParserUtils.ParseInputNoComents(lexBuff, msgs, "testInput");

			var writer = new IndentStringWriter();
			new CanonicAstPrinter(writer).Print(inBlock);

			string actual = writer.GetResult().TrimEnd();

			if (msgs.ErrorOcured) {
				Console.WriteLine("in: " + input);
				Console.WriteLine("exc: " + excpected);
				Console.WriteLine("act: " + actual);
				Assert.Fail(msgs.ToString());
			}

			Assert.AreEqual(excpected, actual);
		}
	}
}

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.FSharp.Text.Lexing;
using Malsys.Compilers;
using Malsys.Parsing;
using Malsys.IO;
using Malsys.SourceCode.Printers;

namespace Malsys.Tests.Parsing {
	[TestClass]
	public class FunctionDefinitionTests {

		[TestMethod]
		public void FunNameTests() {
			doTest("fun f() {", "0", "}");
			doTest("fun f'() {", "0", "}");
			doTest("fun function() {", "0", "}");
			doTest("fun _() {", "0", "}");
			doTest("fun _f() {", "0", "}");
			doTest("fun f_() {", "0", "}");
			doTest("fun {0}() {{".Fmt(CharHelper.Pi), "0", "}");
		}

		[TestMethod]
		public void FunParamsTests() {
			doTest("fun f(x) {", "0", "}");
			doTest("fun f(x, y) {", "0", "}");
			doTest("fun f(x, _, z) {", "0", "}");
			doTest("fun f(x'') {", "0", "}");
			doTest("fun f(x = 0) {", "0", "}");
			doTest("fun f(x = 0, y = 0) {", "0", "}");
			doTest("fun f(y = {}) {", "0", "}");
			doTest("fun f(y = {-1, 1}) {", "0", "}");
			doTest("fun f(a, b, c = d) {", "0", "}");
			doTest("fun f(a, b = x, c) {", "0", "}");
		}

		[TestMethod]
		public void FunReturnValueTests() {
			doTest("fun f() {", "1+1", "}");
			doTest("fun f() {", "{}", "}");
			doTest("fun f() {", "{{{}}}", "}");
			doTest("fun f() {", "{log(e), -1}[1]", "}");
		}

		[TestMethod]
		public void FunLoalVariableTests() {
			doTest("fun f() {", "let x = 0;", "0", "}");
			doTest("fun f() {", "let x = 0;", "let y = 0;", "0", "}");
		}


		private void doTest(params string[] inputLines) {
			for (int i = 1; i < inputLines.Length - 1; i++) {
				inputLines[i] = "\t" + inputLines[i];
			}

			string input = string.Join(Environment.NewLine, inputLines);
			doTest(input, input);
		}

		private void doTest(string input, string excpected) {

			var lexBuff = LexBuffer<char>.FromString(input);
			var msgs = new MessagesCollection();
			var varDef = ParserUtils.ParseFunDef(lexBuff, msgs, "testInput");

			if (msgs.ErrorOcured) {
				throw new Exception(msgs.ToString());
			}

			var writer = new IndentStringWriter();
			var printer = new CanonicAstPrinter(writer);
			printer.Visit(varDef);

			string actual = writer.GetResult().TrimEnd();

			Assert.AreEqual(excpected, actual);
		}
	}
}

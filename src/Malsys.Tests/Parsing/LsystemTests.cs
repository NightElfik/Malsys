﻿using System;
using System.Globalization;
using System.Threading;
using Malsys.Compilers;
using Malsys.IO;
using Malsys.Parsing;
using Malsys.SourceCode.Printers;
using Microsoft.FSharp.Text.Lexing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Malsys.Tests.Parsing {
	[TestClass]
	public class LsystemTests {

		[TestInitialize]
		public void InitTest() {
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
		}

		[TestMethod]
		public void ParamsTests() {
			doTest("lsystem lsys {}");
			doTest("lsystem lsys() {}", "lsystem lsys {}");
			doTest("lsystem l' ( \n ) {  \n}", "lsystem l' {}");
			doTest("lsystem l(x) {}");
			doTest("lsystem l(x, y) {}");
			doTest("lsystem l(x, _, z) {}");
			doTest("lsystem l(x'') {}");
			doTest("lsystem l(x = 0) {}");
			doTest("lsystem l(x = 0, y = 0) {}");
			doTest("lsystem l(y = {}) {}");
			doTest("lsystem l(y = { - 1, 1}) {}");
			doTest("lsystem l(a, b, c = d) {}");
		}

		[TestMethod]
		public void FunVarDefsTests() {
			doTestAutoident("lsystem l {", "let x = 0;", "}");
			doTestAutoident("lsystem l {", "fun f() {", "\treturn 0;", "}", "}");
			doTestAutoidentOutput("lsystem l{fun f(){let x=0;return x;}}", "lsystem l {", "fun f() {", "\tlet x = 0;", "\treturn x;", "}", "}");
		}

		[TestMethod]
		public void RewriteRuleStatementsTests() {
			doTestAutoident("lsystem l {", "rewrite X", "\tto nothing;", "}");
			doTestAutoident("lsystem l {", "rewrite X", "\twith x = 0", "\tto nothing;", "}");
			doTestAutoident("lsystem l {", "rewrite X", "\twith x = 0, y = {x}", "\tto nothing;", "}");
			doTestAutoident("lsystem l {", "rewrite X", "\twhere x > 0", "\tto nothing;", "}");
			doTestAutoident("lsystem l {", "rewrite X", "\tto nothing weight 0;", "}");
			doTestAutoident("lsystem l {", "rewrite X", "\tto nothing weight 0", "\tor to nothing weight 0;", "}");
			doTestAutoident("lsystem l {", "rewrite X",
				"\twith x = 0, y = x + 1, z = {x, z}",
				"\twhere x > 0 && z[0] == x",
				"\tto XYZ weight 0",
				"\tor to nothing weight 0",
				"\tor to +-* weight 0;", "}");
		}

		[TestMethod]
		public void RewriteRuleContextTests() {
			doTestAutoidentOutput("lsystem l {rewrite {} X to nothing;}", "lsystem l {", "rewrite X", "\tto nothing;", "}");
			doTestAutoident("lsystem l {", "rewrite {L(x)cTx(_, x)T} X", "\tto nothing;", "}");

			doTestAutoidentOutput("lsystem l {rewrite X {} to nothing;}", "lsystem l {", "rewrite X", "\tto nothing;", "}");
			doTestAutoident("lsystem l {", "rewrite X {R(x)cTx(_, x)T}", "\tto nothing;", "}");

			doTestAutoident("lsystem l {", "rewrite {L} X {R}", "\tto nothing;", "}");
			doTestAutoident("lsystem l {", "rewrite {L(_)} X(_, _) {R(_)}", "\tto nothing;", "}");
		}

		[TestMethod]
		public void RewriteRulePatternTests() {
			doTestAutoident("lsystem l {", "rewrite x", "\tto nothing;", "}");
			doTestAutoident("lsystem l {", "rewrite A", "\tto nothing;", "}");
			doTestAutoident("lsystem l {", "rewrite +", "\tto nothing;", "}");
			doTestAutoident("lsystem l {", "rewrite |", "\tto nothing;", "}");
			doTestAutoidentOutput("lsystem l {rewrite X() to nothing;}", "lsystem l {", "rewrite X", "\tto nothing;", "}");
			doTestAutoident("lsystem l {", "rewrite X(a, b, c')", "\tto nothing;", "}");
			doTestAutoident("lsystem l {", "rewrite X(_, x)", "\tto nothing;", "}");
		}

		[TestMethod]
		public void RewriteRuleReplacementTests() {
			doTestAutoident("lsystem l {", "rewrite X", "\tto +-A*C;", "}");
			doTestAutoident("lsystem l {", "rewrite X", "\tto +(a)-(b + c)A( - d)*(1 * 2)C(C);", "}");
			doTestAutoident("lsystem l {", "rewrite X", "\tto &(func(a));", "}");
			doTestAutoident("lsystem l {", "rewrite X", "\tto &({1 & 2});", "}");
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

		private void doTest(string input, string excpected = null) {

			if (excpected == null) {
				excpected = input;
			}

			var lexBuff = LexBuffer<char>.FromString(input);
			var msgs = new MessagesCollection();
			var lsystem = ParserUtils.ParseLsystem(lexBuff, msgs, "testInput");

			var writer = new IndentStringWriter();
			var printer = new CanonicAstPrinter(writer);
			if (lsystem != null) {
				printer.Visit(lsystem);
			}

			string actual = writer.GetResult().Trim();

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
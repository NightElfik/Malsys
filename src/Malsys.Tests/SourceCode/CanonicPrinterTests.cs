using System;
using System.IO;
using System.Linq;
using Malsys.IO;
using Malsys.Resources;
using Malsys.SourceCode.Printers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Malsys.Tests.SourceCode {
	/// <summary>
	/// Source code printer tests are important because source code printer is used for canonicalization of inputs.
	/// </summary>
	[TestClass]
	public class CanonicPrinterTests {

		[TestMethod]
		public void GlobalConstTests() {
			doTestAutoident("let a = 0;");
			doTestAutoident("let b = {1, 2};");
			doTestAutoident("let c = -2;");
			doTestAutoident("let d = 0x123ABC;");
			doTestAutoident("let e = 0b101010111;");
			doTestAutoident("let f = 0o1234567;");
			doTestAutoident("let g = #123DEF;");
		}

		[TestMethod]
		public void GlobalFunTests() {
			doTestAutoident(
				"fun a(b, p = 7) {",
				"return 0;",
				"}");
			doTestAutoident(
				"fun a(b, p = 7) {",
				"let x = (b + (-2));",
				"let c = {1, 2, 3}[2];",
				"return c;",
				"}");
		}

		[TestMethod]
		public void LsystemInheritanceTests() {
			doTestAutoident("lsystem a {", "}");
			doTestAutoident("lsystem b extends a {", "}");
			doTestAutoident("lsystem c(x) extends b(1, 1), a(x) {", "}");
		}

		[TestMethod]
		public void AbstractLsystemTests() {
			doTestAutoident("abstract lsystem c {", "}");
		}

		[TestMethod]
		public void LsystemParamsTests() {
			doTestAutoident("lsystem l(a, b = 20) {", "}");
		}

		[TestMethod]
		public void LsystemLetTests() {
			doTestAutoident(
				"lsystem l {",
				"let x = 10;",
				"let y = {0};",
				"}");
		}

		[TestMethod]
		public void LsystemSetTests() {
			doTestAutoident(
				"lsystem l {",
				"set x = 10;",
				"set y = f(a, b, c);",
				"}");
		}

		[TestMethod]
		public void LsystemSetSymbolsTests() {
			doTestAutoident(
				"lsystem l {",
				"set symbols x = A B;",
				"}");
			doTestAutoident(
				"lsystem l {",
				"set symbols x = A(x) B(1, 2, {3});",
				"}");
		}

		[TestMethod]
		public void LsystemFunTests() {
			doTestAutoident(
				"lsystem l {",
				"fun f() {",
				"\treturn 20;",
				"}",
				"}");
			doTestAutoident(
				"lsystem l {",
				"fun f(x, y = 20) {",
				"\tlet x = 0;",
				"\treturn 20;",
				"}",
				"}");
		}

		[TestMethod]
		public void LsystemRewriteTests() {
			doTestAutoident(
				"lsystem l {",
				"rewrite a",
				"to b;",
				"}");
			doTestAutoident(
				"lsystem l {",
				"rewrite {b c(x)} a(y, z) {d e}",
				"to b;",
				"}");
			doTestAutoident(
				"lsystem l {",
				"rewrite a",
				"where (1 > 2)",
				"to b;",
				"}");
			doTestAutoident(
				"lsystem l {",
				"rewrite a",
				"with x = 20, y = 55",
				"to b;",
				"}");
			doTestAutoident(
				"lsystem l {",
				"rewrite a",
				"to b",
				"or to c;",
				"}");
			doTestAutoident(
				"lsystem l {",
				"rewrite a",
				"to b weight 10",
				"or to c weight 0;",
				"}");
			doTestAutoident(
				"lsystem l {",
				"rewrite a",
				"to nothing;",
				"}");
		}

		[TestMethod]
		public void LsystemInterpretTests() {
			doTestAutoident(
				"lsystem l {",
				"interpret a as b;",
				"}");
			doTestAutoident(
				"lsystem l {",
				"interpret a(a, b = 1) as c(x, y, 5);",
				"}");
		}


		[TestMethod]
		public void ProcessStatementTests() {
			doTestAutoident("process all with a;");
			doTestAutoident("process LsystemName with Configuration;");
		}

		[TestMethod]
		public void ProcessStatementParamsTests() {
			doTestAutoident("process all(0, 1, 2, 3) with a;");
			doTestAutoident("process LsystemName(0, {1}) with Configuration;");
		}

		[TestMethod]
		public void ProcessStatementUseTests() {
			doTestAutoident(
				"process all with Configuration",
				"use SvgRewriter as Rewriter",
				"use v as w;");
		}

		[TestMethod]
		public void ProcessStatementLsysStatementsTests() {
			doTestAutoident(
				"process all with Configuration",
				"set a = 10",
				"set b = 20;");
			doTestAutoident(
				"process all with Configuration",
				"set symbols a = A B(20);");
			doTestAutoident(
				"process all with Configuration",
				"rewrite {a(x)} a(y) {a}",
				"where (x < y)",
				"to b(a)",
				"interpret a b c(x, y = 10) as b(y);");
		}


		[TestMethod]
		public void ProcessConfigTests() {
			doTestAutoident(
				"configuration c {",
				"}");
			doTestAutoident(
				"configuration c {",
				"component a typeof x;",
				"}");
			doTestAutoident(
				"configuration c {",
				"container a typeof x default y;",
				"}");
			doTestAutoident(
				"configuration c {",
				"component a typeof x;",
				"container b typeof x default y;",
				"connect a to b.c;",
				"}");
			doTestAutoident(
				"configuration c {",
				"virtual connect a to b.c;",
				"}");
		}

		[TestMethod]
		public void VirtualConnProcessConfigTests() {
			doTestAutoident(
				"configuration c {",
				"virtual connect a to b.c;",
				"}");
		}


		[TestMethod]
		public void StdLibTests() {

			string stdLib;
			var logger = new MessageLogger();

			using (Stream stream = new ResourcesReader().GetResourceStream(ResourcesHelper.StdLibResourceName)) {
				using (TextReader reader = new StreamReader(stream)) {
					stdLib = reader.ReadToEnd();
				}
			}

			var inputEvaled = TestUtils.EvaluateInput(stdLib);

			var writer = new IndentStringWriter();
			new CanonicPrinter(writer).Print(inputEvaled);
			var actual = writer.GetResult().TrimEnd();

			var inputPrintedEvaled = TestUtils.EvaluateInput(actual);

			Assert.AreEqual(inputEvaled.Lsystems.Count, inputPrintedEvaled.Lsystems.Count);
			Assert.AreEqual(inputEvaled.ProcessConfigurations.Count, inputPrintedEvaled.ProcessConfigurations.Count);
			Assert.AreEqual(inputEvaled.ProcessStatements.Count, inputPrintedEvaled.ProcessStatements.Count);
			Assert.AreEqual(inputEvaled.ExpressionEvaluatorContext.GetAllStoredVariables().Count(), inputPrintedEvaled.ExpressionEvaluatorContext.GetAllStoredVariables().Count());
			Assert.AreEqual(inputEvaled.ExpressionEvaluatorContext.GetAllStoredFunctions().Count(), inputPrintedEvaled.ExpressionEvaluatorContext.GetAllStoredFunctions().Count());
		}


		private void autoIndent(string[] lines) {
			for (int i = 1; i < lines.Length - 1; i++) {
				if (lines[i].StartsWith("to ") || lines[i].StartsWith("or ") || lines[i].StartsWith("with ") || lines[i].StartsWith("where ")) {
					lines[i] = "\t" + lines[i];
				}
				lines[i] = "\t" + lines[i];
			}

			if (lines.Length > 1 && lines[lines.Length - 1] != "}") {
				lines[lines.Length - 1] = "\t" + lines[lines.Length - 1];
			}
		}

		private void doTestAutoident(params string[] inputLines) {
			autoIndent(inputLines);

			string input = string.Join(Environment.NewLine, inputLines);
			doTest(input, input);
		}

		private void doTest(string input, string expected) {

			var inputEvaled = TestUtils.EvaluateInput(input);

			var writer = new IndentStringWriter();
			new CanonicPrinter(writer).Print(inputEvaled);
			var actual = writer.GetResult().TrimEnd();

			Assert.AreEqual(expected, actual);
		}
	}
}

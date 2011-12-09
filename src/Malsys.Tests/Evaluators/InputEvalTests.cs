using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Malsys.Tests.Evaluators {
	[TestClass]
	public class InputEvalTests {

		[TestMethod]
		public void GlobalConstTests() {
			doTestAutoidentOutput(
				"let a = 1 - 2 + 1; let c = 2 + a; let b = a * c + 1;",
				"let a = 0;",
				"let b = 1;",
				"let c = 2;");
		}

		[TestMethod]
		public void ConstRedefinitionTests() {
			doTestAutoidentOutput(
				"let a = 2; let b = a * a; let a = 0;",
				"let a = 0;",
				"let b = 4;");
		}

		[TestMethod]
		public void GlobalFunTests() {
			doTestAutoidentOutput(
				"let x = 5; fun b() { return 2; } fun a(p = b() + x) { return 0; } ",
				"let x = 5;",
				"fun a(p = 7) {",
				"return 0;",
				"}",
				"fun b() {",
				"return 2;",
				"}");
		}

		[TestMethod]
		public void FunRedefinitionTests() {
			doTestAutoidentOutput(
				"fun a() { return 2; } let a = a(); fun a(x) { return x; }",
				"let a = 2;",
				"fun a(x) {",
				"return x;",
				"}");
		}

		[TestMethod]
		public void LsystemTests() {
			doTestAutoidentOutput(
				"lsystem l { let x = 10; }",
				"lsystem l {",
				"let x = 10;",
				"}");
			doTestAutoidentOutput(
				"lsystem l { fun f() { return 3; } }",
				"lsystem l {",
				"fun f() {",
				"return 3;",
				"}",
				"}");
			doTestAutoidentOutput(
				"let x = 5; lsystem l(q, p = 10-x) { }",
				"let x = 5;",
				"lsystem l(q, p = 5) {",
				"}");
		}

		[TestMethod]
		public void LsystemRedefinitionTests() {
			doTestAutoidentOutput(
				"lsystem b {} lsystem a {} lsystem b(x) {}",
				"lsystem a {", "}",
				"lsystem b(x) {", "}");
		}

		[TestMethod]
		public void ProcessConfigTests() {
			doTestAutoidentOutput(
				"configuration c { }",
				"configuration c {",
				"}");
			doTestAutoidentOutput(
				"configuration c { component b typeof x; component a typeof x; }",
				"configuration c {",
				"component a typeof x;",
				"component b typeof x;",
				"}");
			doTestAutoidentOutput(
				"configuration c { container b typeof x default y; container a typeof x default y; }",
				"configuration c {",
				"container a typeof x default y;",
				"container b typeof x default y;",
				"}");
			doTestAutoidentOutput(
				"configuration c { component a typeof x; component b typeof x; connect b to b.c; connect a to b.c; }",
				"configuration c {",
				"component a typeof x;",
				"component b typeof x;",
				"connect a to b.c;",
				"connect b to b.c;",
				"}");
			doTestAutoidentOutput(
				"configuration c { container b typeof x default y; component a typeof x; connect a to b.c; }",
				"configuration c {",
				"component a typeof x;",
				"container b typeof x default y;",
				"connect a to b.c;",
				"}");
		}

		[TestMethod]
		public void ProcessStatementTests() {
			doTestAutoidentOutput(
				"process this with a; process x with c;",
				"process this with a;",
				"process x with c;");
			doTestAutoidentOutput(
				"process with a; process x with c;",
				"process this with a;",
				"process x with c;");
		}

		[TestMethod]
		public void InputTests() {
			doTestAutoidentOutput(
				"process l with c; fun f() {return 2;} configuration c { } lsystem l(p=f()) {} let x = f() + 8; let a = x - 8 - f();",
				"let a = 0;",
				"let x = 10;",
				"fun f() {",
				"return 2;",
				"}",
				"lsystem l(p = 2) {",
				"}",
				"configuration c {",
				"}",
				"process l with c;");
		}


		private void doTestAutoidentOutput(string input, params string[] outputLines) {

			int indentLevel = 0;

			for (int i = 0; i < outputLines.Length; i++) {
				if (outputLines[i].StartsWith("}")) {
					indentLevel--;
				}

				outputLines[i] = new string('\t', indentLevel) + outputLines[i];

				if (outputLines[i].EndsWith("{")) {
					indentLevel++;
				}
			}

			string output = string.Join(Environment.NewLine, outputLines);
			doTest(input, output);
		}

		private void doTest(string input, string excepted) {

			var inputEvaled = CompilerUtils.EvaluateLsystem(input);
			var actual = CompilerUtils.Print(inputEvaled).TrimEnd();
			Assert.AreEqual(excepted, actual);
		}

	}
}

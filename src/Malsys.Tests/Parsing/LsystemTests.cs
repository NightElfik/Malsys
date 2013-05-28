// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Malsys.Tests.Parsing {
	[TestClass]
	public class LsystemTests {

		[TestMethod]
		public void ParamsTests() {
			doTestAutoident("lsystem lsys {", "}");
			doTestAutoidentOutput("lsystem lsys() {}", "lsystem lsys {", "}");
			doTestAutoidentOutput("lsystem l' ( \n ) {  \n}", "lsystem l' {", "}");
			doTestAutoident("lsystem l(x) {", "}");
			doTestAutoident("lsystem l(x, y) {", "}");
			doTestAutoident("lsystem l(x, _, z) {", "}");
			doTestAutoident("lsystem l(x'') {", "}");
			doTestAutoident("lsystem l(x = 0) {", "}");
			doTestAutoident("lsystem l(x = 0, y = 0) {", "}");
			doTestAutoident("lsystem l(y = {}) {", "}");
			doTestAutoident("lsystem l(y = { - 1, 1}) {", "}");
			doTestAutoident("lsystem l(a, b, c = d) {", "}");
		}

		[TestMethod]
		public void FunVarDefsTests() {
			doTestAutoident("lsystem l {", "let x = 0;", "}");
			doTestAutoident("lsystem l {", "fun f() {", "\treturn 0;", "}", "}");
			doTestAutoidentOutput("lsystem l{fun f(){let x=0;return x;}}", "lsystem l {", "fun f() {", "\tlet x = 0;", "\treturn x;", "}", "}");
		}

		[TestMethod]
		public void CompValAssignTests() {
			doTestAutoident("lsystem l {", "set x = 20;", "}");
			doTestAutoident("lsystem l {", "set abcd' = {1, 2};", "}");
		}

		[TestMethod]
		public void CompSymAssignTests() {
			doTestAutoident("lsystem l {", "set symbols x = x;", "}");
			doTestAutoident("lsystem l {", "set symbols abcd' = +(a + 0) X( - 1) *(1 * 1);", "}");
		}

		[TestMethod]
		public void RewriteRuleStatementsTests() {
			doTestAutoident("lsystem l {", "rewrite X", "to nothing;", "}");
			doTestAutoident("lsystem l {", "rewrite X", "with x = 0", "to nothing;", "}");
			doTestAutoident("lsystem l {", "rewrite X", "with x = 0, y = {x}", "to nothing;", "}");
			doTestAutoident("lsystem l {", "rewrite X", "where x > 0", "to nothing;", "}");
			doTestAutoident("lsystem l {", "rewrite X", "to nothing weight 0;", "}");
			doTestAutoident("lsystem l {", "rewrite X", "to nothing weight 0", "or to nothing weight 0;", "}");
			doTestAutoident("lsystem l {", "rewrite X",
				"with x = 0, y = x + 1, z = {x, z}",
				"where x > 0 && z[0] == x",
				"to X Y Z weight 0",
				"or to nothing weight 0",
				"or to + - * weight 0;", "}");
		}

		[TestMethod]
		public void RewriteRuleContextTests() {
			doTestAutoidentOutput("lsystem l {rewrite {} X to nothing;}", "lsystem l {", "rewrite X", "to nothing;", "}");
			doTestAutoident("lsystem l {", "rewrite {L(x) c T x(_, x) T} X", "to nothing;", "}");

			doTestAutoidentOutput("lsystem l {rewrite X {} to nothing;}", "lsystem l {", "rewrite X", "to nothing;", "}");
			doTestAutoident("lsystem l {", "rewrite X {R(x) c T x(_, x) T}", "to nothing;", "}");

			doTestAutoident("lsystem l {", "rewrite {L} X {R}", "to nothing;", "}");
			doTestAutoident("lsystem l {", "rewrite {L(_)} X(_, _) {R(_)}", "to nothing;", "}");
		}

		[TestMethod]
		public void RewriteRulePatternTests() {
			doTestAutoident("lsystem l {", "rewrite x", "to nothing;", "}");
			doTestAutoident("lsystem l {", "rewrite A", "to nothing;", "}");
			doTestAutoident("lsystem l {", "rewrite +", "to nothing;", "}");
			doTestAutoident("lsystem l {", "rewrite |", "to nothing;", "}");
			doTestAutoidentOutput("lsystem l {rewrite X() to nothing;}", "lsystem l {", "rewrite X", "to nothing;", "}");
			doTestAutoident("lsystem l {", "rewrite X(a, b, c')", "to nothing;", "}");
			doTestAutoident("lsystem l {", "rewrite X(_, x)", "to nothing;", "}");
		}

		[TestMethod]
		public void RewriteRuleReplacementTests() {
			doTestAutoident("lsystem l {", "rewrite X", "to + - A * C;", "}");
			doTestAutoident("lsystem l {", "rewrite X", "to +(a) -(b + c) A( - d) *(1 * 2) C(C);", "}");
			doTestAutoident("lsystem l {", "rewrite X", "to &(func(a));", "}");
			doTestAutoident("lsystem l {", "rewrite X", "to &({1 & 2});", "}");
		}

		[TestMethod]
		public void SymbolsInterpretationsTests() {
			doTestAutoident("lsystem l {", "interpret X as Action;", "}");
			doTestAutoident("lsystem l {", "interpret X Y Z as Action;", "}");
			doTestAutoident("lsystem l {", "interpret X as Action(0);", "}");
			doTestAutoident("lsystem l {", "interpret X Y Z as Action(0, 1, 2);", "interpret x as Action(x);", "}");
		}

		[TestMethod]
		public void SymbolsInterpretationsArgsTests() {
			doTestAutoident("lsystem l {", "interpret X(a) as Action;", "}");
			doTestAutoident("lsystem l {", "interpret X Y Z(a, b = 20) as Action(a, b);", "}");
		}

		[TestMethod]
		public void SymbolsInterpretationsLsystemTests() {
			doTestAutoident("lsystem l {", "interpret X(a) as lsystem Action;", "}");
			doTestAutoident("lsystem l {", "interpret X Y Z(a, b = 20) as lsystem Action(a, b);", "}");
		}

		[TestMethod]
		public void LsystemInheritanceTests() {
			doTestAutoident("lsystem l extends x {", "}");
			doTestAutoident("lsystem l extends x, y {", "}");
			doTestAutoidentOutput("lsystem l() extends a(), b() { }", "lsystem l extends a, b {", "}");
			doTestAutoident("lsystem l extends x(a, b), y {", "}");
			doTestAutoident("lsystem l extends x(a, b), y(d, e), z {", "}");
		}

		[TestMethod]
		public void AbstractLsystemTests() {
			doTestAutoident("abstract lsystem l {", "}");
		}


		private void autoIndent(string[] lines) {
			for (int i = 1; i < lines.Length - 1; i++) {
				if (lines[i].StartsWith("to ") || lines[i].StartsWith("or ") || lines[i].StartsWith("with ") || lines[i].StartsWith("where ")) {
					lines[i] = "\t" + lines[i];
				}
				lines[i] = "\t" + lines[i];
			}
		}

		private void doTestAutoident(params string[] inputLines) {
			autoIndent(inputLines);

			string input = string.Join(Environment.NewLine, inputLines);
			doTest(input, input);
		}

		private void doTestAutoidentOutput(string input, params string[] outputLines) {
			autoIndent(outputLines);

			string output = string.Join(Environment.NewLine, outputLines);
			doTest(input, output);
		}

		private void doTest(string input, string expected = null) {

			if (expected == null) {
				expected = input;
			}

			string actual = TestUtils.Print(TestUtils.ParseLsystem(input)).TrimEnd();
			Assert.AreEqual(expected, actual);
		}

	}
}

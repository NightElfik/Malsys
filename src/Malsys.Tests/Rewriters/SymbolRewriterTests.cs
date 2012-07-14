/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System;
using System.Collections.Generic;
using Malsys.Evaluators;
using Malsys.IO;
using Malsys.Processing;
using Malsys.Processing.Components.Common;
using Malsys.Processing.Components.Rewriters;
using Malsys.Processing.Output;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;
using Malsys.SourceCode.Printers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Malsys.Tests.Rewriters {
	[TestClass]
	public class SymbolRewriterTests {

		[TestMethod]
		public void EmptyInputTests() {
			doTest("", "", "", "");
		}

		[TestMethod]
		public void NoRewriteRulesTests() {
			doTest("", "x", "x");
			doTest("", "a B c", "a B c");
			doTest("", "a(30) B c(1 + 2)", "a(30) B c(3)");
		}

		[TestMethod]
		public void NothingOnRightSideTests() {
			doTest("rewrite x to nothing;",
				"x x x x x",
				"");
			doTest("rewrite x to ;",
				"x A x B x C x",
				"A B C");
			doTest("rewrite x to ;",
				"x(20) x(1, 1)",
				"");
		}

		[TestMethod]
		public void PatternVarsTests() {
			doTest("rewrite x(x) to x(x*x);",
				"x(-1) x(2)",
				"x(1) x(4)",
				"x(1) x(16)");
			doTest("rewrite x(a, b, c, d, e) to x(a, b, c, d, e);",
				"x(0, 0)",
				"x(0, 0, NaN, NaN, NaN)");
		}

		[TestMethod]
		public void LeftContextTests() {
			doTest("rewrite {y} x to nothing;",
				"x y x y x x",
				"x y y x",
				"x y y");
			doTest("rewrite {x x} x to nothing;",
				"x(1) x x x x(0)",
				"x(1) x",
				"x(1) x");
			doTest("rewrite {a b c} x to z;",
				"b c x a b c x c x b c x",
				"b c x a b c z c x b c x");

			doTest("rewrite {a(a)} x(x) to x(a + x);",
				"x(1) a(2) x(3)",
				"x(1) a(2) x(5)",
				"x(1) a(2) x(7)");
			doTest("rewrite {a(a) b(b)} x(x) to x(a + b + x);",
				"a(2) b x(3)",
				"a(2) b x(NaN)");

			doTest("rewrite {x(x)} y(y) to x(y) y(x + y);",
				"x(1) y(1)",
				"x(1) x(1) y(2)",
				"x(1) x(1) x(2) y(3)",
				"x(1) x(1) x(2) x(3) y(5)",
				"x(1) x(1) x(2) x(3) x(5) y(8)");

			doTest("rewrite {x(a, b, c, x)} x(d, e, f) to x(a + d, b + e, c + f, x);",
				"x(1, 2, 3) x(-1, -2, -3)",
				"x(1, 2, 3) x(0, 0, 0, NaN)");
		}

		[TestMethod]
		public void LeftContextAdvancedTests() {
			// [ opens branch and ] closes it

			doTest("rewrite { y } x to nothing;",
				"x y [ a ] [ x y [ a ] x x ]",
				"x y [ a ] [ y [ a ] x ]",
				"x y [ a ] [ y [ a ] ]");

			// I is ignored symbol by context
			doTest("rewrite { y } x to nothing;",
				"x I y I [ a ] I [ x y I [ a ] I x I x ]",
				"x I y I [ a ] I [ y I [ a ] I I x ]",
				"x I y I [ a ] I [ y I [ a ] I I ]");
		}

		[TestMethod]
		public void RightContextAdvancedTests() {
			// [ opens branch and ] closes it

			doTest("rewrite x { y } to nothing;",
				"x [ a ] y [ x [ a ] x y x ]",
				"[ a ] y [ x [ a ] y x ]",
				"[ a ] y [ [ a ] y x ]");

			// I is ignored symbol by context
			doTest("rewrite x { y } to nothing;",
				"x I [ a ] y [ x I [ a ] x I y x ]",
				"I [ a ] y [ x I [ a ] I y x ]",
				"I [ a ] y [ I [ a ] I y x ]");
		}

		[TestMethod]
		public void RightContextTests() {
			doTest("rewrite x {y} to nothing;",
				"x y x x y x",
				"y x y x",
				"y y x");
			doTest("rewrite x {x x} to nothing;",
				"x(1) x x x x(0)",
				"x x(0)",
				"x x(0)");
			doTest("rewrite x {a b c} to z;",
				"b c x a b c x c x b c x",
				"b c z a b c x c x b c x");

			doTest("rewrite x(x) {a(a)} to x(a + x);",
				"x(1) a(2) x(3)",
				"x(3) a(2) x(3)",
				"x(5) a(2) x(3)");
			doTest("rewrite x(x) {a(a) b(b)} to x(a + b + x);",
				"x(3) a(2) b",
				"x(NaN) a(2) b");

			doTest("rewrite y(y) {x(x)} to y(x + y) x(y);",
				"y(1) x(1)",
				"y(2) x(1) x(1)",
				"y(3) x(2) x(1) x(1)",
				"y(5) x(3) x(2) x(1) x(1)",
				"y(8) x(5) x(3) x(2) x(1) x(1)");

			doTest("rewrite x(d, e, f) {x(a, b, c, x)} to x(a + d, b + e, c + f, x);",
				"x(1, 2, 3) x(-1, -2, -3)",
				"x(0, 0, 0, NaN) x(-1, -2, -3)");
		}

		[TestMethod]
		public void LocalVarsTests() {
			doTest("rewrite x with a = 10 to x(a);",
				"x",
				"x(10)");
			doTest("rewrite x(a) with y = a + a to x(y);",
				"x(2)",
				"x(4)");
			doTest("rewrite x(a) with a = 10 to x(a);",
				"x(2)",
				"x(10)");
		}

		[TestMethod]
		public void ConditionTests() {
			doTest("rewrite x(x) where x == 0 to x;",
				"x(-1) x(0) x(1)",
				"x(-1) x x(1)");
			doTest("rewrite {x(xl)} x(x) {x(xr)} where xr + x + xl == 0 to a;",
				"x(-1) x(0) x(1) x(1) x(-2)",
				"x(-1) a x(1) a x(-2)");
			doTest("rewrite x where false to nothing;",
				"x x",
				"x x");
			doTest("rewrite x where NaN to nothing;",
				"x x",
				"x x");
			doTest("rewrite x where {2} to nothing;",
				"x x",
				"x x");
		}

		[TestMethod]
		public void AnabaenaCatenulaTests() {
			doTest("rewrite A to A B; rewrite B to A;",
				"A",
				"A B",
				"A B A",
				"A B A A B",
				"A B A A B A B A",
				"A B A A B A B A A B A A B",
				"A B A A B A B A A B A A B A B A A B A B A");
		}

		[TestMethod]
		public void SignalPropagationTests() {
			doTest("rewrite {A} a to A; rewrite A to a;",
				"A a a a a",
				"a A a a a",
				"a a A a a",
				"a a a A a",
				"a a a a A",
				"a a a a a");

			doTest("rewrite a {A} to A; rewrite A to a;",
				"a a a a A",
				"a a a A a",
				"a a A a a",
				"a A a a a",
				"A a a a a",
				"a a a a a");
		}

		/// <summary>
		/// From The Algorithmic Beauty of Plants by Przemyslaw Prusinkiewicz and Aristid Lindenmayer, p. 32.
		/// </summary>
		[TestMethod]
		public void PropagationOfAcropetalSignal() {
			doTest("rewrite {B} A to B;",
				"B [ A ] A [ A ] A [ A ] A",
				"B [ B ] B [ A ] A [ A ] A",
				"B [ B ] B [ B ] B [ A ] A",
				"B [ B ] B [ B ] B [ B ] B");
		}

		/// <summary>
		/// From The Algorithmic Beauty of Plants by Przemyslaw Prusinkiewicz and Aristid Lindenmayer, p. 32.
		/// </summary>
		[TestMethod]
		public void PropagationOfBasipetalSignal() {
			doTest("rewrite A {B} to B;",
				"A [ A ] A [ A ] A [ A ] B",
				"A [ A ] A [ A ] B [ A ] B",
				"A [ A ] B [ A ] B [ A ] B",
				"B [ A ] B [ A ] B [ A ] B");
		}



		private void doTest(string rewriteRules, string inputSymbols, params string[] excpectedIterations) {

			string input = "lsystem l {{ set symbols axiom = {0}; {1} }}".Fmt(inputSymbols, rewriteRules);

			var inBlockEvaled = TestUtils.EvaluateInput(input);

			if (inBlockEvaled.Lsystems.Count != 1) {
				Assert.Fail("L-system not defined.");
			}

			var evaluator = new EvaluatorsContainer(TestUtils.ExpressionEvaluatorContext);
			var lsysEvaluator = evaluator.ResolveLsystemEvaluator();
			var lsystem = TestUtils.EvaluateLsystem(inBlockEvaled.Lsystems["l"]);

			var fm = new FileOutputProvider("./");
			var context = new ProcessContext(lsystem, fm, inBlockEvaled, evaluator,
				TestUtils.ExpressionEvaluatorContext, null, TimeSpan.MaxValue, null);

			var rewriter = new SymbolRewriter();
			rewriter.Reset();
			rewriter.ContextIgnore = new ImmutableList<Symbol<IValue>>(new Symbol<IValue>("I"));
			rewriter.StartBranchSymbols = new ImmutableList<Symbol<IValue>>(new Symbol<IValue>("["));
			rewriter.EndBranchSymbols = new ImmutableList<Symbol<IValue>>(new Symbol<IValue>("]"));

			rewriter.Initialize(context);
			rewriter.SymbolProvider = new SymbolProvider(lsystem.ComponentSymbolsAssigns["axiom"]);

			var symBuff = new SymbolsMemoryBuffer();
			symBuff.Reset();
			int iterations = excpectedIterations.Length;

			for (int i = 0; i < iterations; i++) {

				var symbolsBuffer = new List<Symbol<IValue>>();

				rewriter.BeginProcessing(false);
				foreach (var sym in rewriter) {
					symbolsBuffer.Add(sym);
				}
				rewriter.EndProcessing();

				var writer = new IndentStringWriter();
				var printer = new CanonicPrinter(writer);
				foreach (var sym in symbolsBuffer) {
					printer.Print(sym);
					writer.Write(" ");
				}
				string actual = writer.GetResult().TrimEnd();
				if (excpectedIterations[i] != actual) {
					Console.WriteLine("iteration: " + i);
					Console.WriteLine("out: " + actual);
					Console.WriteLine("excepted: " + excpectedIterations[i]);
				}
				Assert.AreEqual(excpectedIterations[i], actual);

				rewriter.SymbolProvider = new SymbolProvider(symbolsBuffer);
			}

			rewriter.Cleanup();

		}
	}
}

using System;
using System.Collections.Generic;
using Malsys.Compilers;
using Malsys.Expressions;
using Malsys.IO;
using Malsys.Parsing;
using Malsys.Processing.Components;
using Malsys.Processing.Components.Interpreters;
using Malsys.SourceCode.Printers;
using Microsoft.FSharp.Text.Lexing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Malsys.Processing;

namespace Malsys.Tests.Rewriters {
	public static class GenericRewriterTests {

		public static void EmptyInputTests(IRewriter rewriter) {
			doTest(rewriter, "", "", "", "");
		}

		public static void NoRewriteRulesTests(IRewriter rewriter) {
			doTest(rewriter, "", "x", "x");
			doTest(rewriter, "", "a B c", "a B c");
			doTest(rewriter, "", "a(30) B c(1 + 2)", "a(30) B c(3)");
		}

		public static void NothingOnRightSideTests(IRewriter rewriter) {
			doTest(rewriter,
				"rewrite x to nothing;",
				"x x x x x",
				"");
			doTest(rewriter,
				"rewrite x to ;",
				"x A x B x C x",
				"A B C");
			doTest(rewriter,
				"rewrite x to ;",
				"x(20) x(1, 1)",
				"");
		}

		public static void PatternVarsTests(IRewriter rewriter) {
			doTest(rewriter,
				"rewrite x(x) to x(x*x);",
				"x(-1) x(2)",
				"x(1) x(4)",
				"x(1) x(16)");
			doTest(rewriter,
				"rewrite x(a, b, c, d, ee) to x(a, b, c, d, ee);",
				"x(0, 0)",
				"x(0, 0, NaN, NaN, NaN)");
		}

		public static void LeftContextTests(IRewriter rewriter) {
			doTest(rewriter,
				"rewrite {y} x to nothing;",
				"x y x y x x",
				"x y y x",
				"x y y");
			doTest(rewriter,
				"rewrite {x x} x to nothing;",
				"x(1) x x x x(0)",
				"x(1) x",
				"x(1) x");
			doTest(rewriter,
				"rewrite {a b c} x to z;",
				"b c x a b c x c x b c x",
				"b c x a b c z c x b c x");

			doTest(rewriter,
				"rewrite {a(a)} x(x) to x(a + x);",
				"x(1) a(2) x(3)",
				"x(1) a(2) x(5)",
				"x(1) a(2) x(7)");
			doTest(rewriter,
				"rewrite {a(a) b(b)} x(x) to x(a + b + x);",
				"a(2) b x(3)",
				"a(2) b x(NaN)");

			doTest(rewriter,
				"rewrite {x(x)} y(y) to x(y) y(x + y);",
				"x(1) y(1)",
				"x(1) x(1) y(2)",
				"x(1) x(1) x(2) y(3)",
				"x(1) x(1) x(2) x(3) y(5)",
				"x(1) x(1) x(2) x(3) x(5) y(8)");

			doTest(rewriter, "rewrite {x(a, b, c, x)} x(d, ee, f) to x(a + d, b + ee, c + f, x);",
				"x(1, 2, 3) x(-1, -2, -3)",
				"x(1, 2, 3) x(0, 0, 0, NaN)");
		}

		public static void RightContextTests(IRewriter rewriter) {
			doTest(rewriter,
				"rewrite x {y} to nothing;",
				"x y x x y x",
				"y x y x",
				"y y x");
			doTest(rewriter,
				"rewrite x {x x} to nothing;",
				"x(1) x x x x(0)",
				"x x(0)",
				"x x(0)");
			doTest(rewriter,
				"rewrite x {a b c} to z;",
				"b c x a b c x c x b c x",
				"b c z a b c x c x b c x");

			doTest(rewriter,
				"rewrite x(x) {a(a)} to x(a + x);",
				"x(1) a(2) x(3)",
				"x(3) a(2) x(3)",
				"x(5) a(2) x(3)");
			doTest(rewriter,
				"rewrite x(x) {a(a) b(b)} to x(a + b + x);",
				"x(3) a(2) b",
				"x(NaN) a(2) b");

			doTest(rewriter,
				"rewrite y(y) {x(x)} to y(x + y) x(y);",
				"y(1) x(1)",
				"y(2) x(1) x(1)",
				"y(3) x(2) x(1) x(1)",
				"y(5) x(3) x(2) x(1) x(1)",
				"y(8) x(5) x(3) x(2) x(1) x(1)");

			doTest(rewriter, "rewrite x(d, ee, f) {x(a, b, c, x)} to x(a + d, b + ee, c + f, x);",
				"x(1, 2, 3) x(-1, -2, -3)",
				"x(0, 0, 0, NaN) x(-1, -2, -3)");
		}

		public static void LocalVarsTests(IRewriter rewriter) {
			doTest(rewriter,
				"rewrite x with a = 10 to x(a);",
				"x",
				"x(10)");
			doTest(rewriter,
				"rewrite x(a) with a = 10 to x(a);",
				"x(2)",
				"x(10)");
		}

		public static void ConditionTests(IRewriter rewriter) {
			doTest(rewriter,
				"rewrite x(x) where x == 0 to x;",
				"x(-1) x(0) x(1)",
				"x(-1) x x(1)");
			doTest(rewriter,
				"rewrite {x(xl)} x(x) {x(xr)} where xr + x + xl == 0 to a;",
				"x(-1) x(0) x(1) x(1) x(-2)",
				"x(-1) a x(1) a x(-2)");
			doTest(rewriter,
				"rewrite x where false to nothing;",
				"x x",
				"x x");
			doTest(rewriter,
				"rewrite x where NaN to nothing;",
				"x x",
				"x x");
			doTest(rewriter,
				"rewrite x where {2} to nothing;",
				"x x",
				"x x");
		}

		public static void AnabaenaCatenulaTests(IRewriter rewriter) {
			doTest(rewriter,
				"rewrite A to A B; rewrite B to A;",
				"A",
				"A B",
				"A B A",
				"A B A A B",
				"A B A A B A B A",
				"A B A A B A B A A B A A B",
				"A B A A B A B A A B A A B A B A A B A B A");
		}

		public static void SignalPropagationTests(IRewriter rewriter) {
			doTest(rewriter,
				"rewrite {A} a to A; rewrite A to a;",
				"A a a a a",
				"a A a a a",
				"a a A a a",
				"a a a A a",
				"a a a a A",
				"a a a a a");

			doTest(rewriter,
				"rewrite a {A} to A; rewrite A to a;",
				"a a a a A",
				"a a a A a",
				"a a A a a",
				"a A a a a",
				"A a a a a",
				"a a a a a");
		}



		private static void doTest(IRewriter rewriter, string rewriteRules, string inputSymbols, params string[] excpectedIterations) {
			string input = "lsystem l {{ set axiom = {0}; {1} }}".Fmt(inputSymbols, rewriteRules);

			var lexBuff = LexBuffer<char>.FromString(input);
			var msgs = new MessagesCollection();
			var lsystemAst = ParserUtils.ParseLsystem(lexBuff, msgs, "testInput");

			if (msgs.ErrorOcured) {
				Console.WriteLine("lsys: " + input);
				Assert.Fail("Failed to parse L-system. " + msgs.ToString());
			}

			var compiler = new LsystemCompiler(new InputCompiler(msgs));
			var lsystem = compiler.Compile(lsystemAst);

			if (!lsystem) {
				Assert.Fail("Failed to compile L-system." + msgs.ToString());
			}

			if (lsystem.Result.Symbols.Count != 1) {
				Assert.Fail("Excpected 1 symbol definition.");
			}

			var data = new InputData();
			data.Add(lsystem);
			var context = new ProcessContext("l", null, data);

			var symBuff = new SymbolsBuffer();
			rewriter.OutputProcessor = symBuff;
			rewriter.Context = context;

			int iterations = excpectedIterations.Length;
			IEnumerable<Symbol<IValue>> axiom = lsystem.Result.Symbols["axiom"].Evaluate();

			for (int i = 0; i < iterations; i++) {

				rewriter.BeginProcessing();
				foreach (var sym in axiom) {
					rewriter.ProcessSymbol(sym);
				}
				rewriter.EndProcessing();

				var result = symBuff.GetAndClear();

				var writer = new IndentStringWriter();
				var printer = new CanonicPrinter(writer);
				foreach (var sym in result) {
					printer.Print(sym);
					writer.Write(" ");
				}
				string actual = writer.GetResult().TrimEnd();
				if (excpectedIterations[i] != actual) {
					Console.WriteLine("iteration: " + i);
					var w = new IndentStringWriter();
					var p = new CanonicPrinter(w);
					foreach (var sym in axiom) {
						p.Print(sym);
						w.Write(" ");
					}
					Console.WriteLine("in: " + w.GetResult().TrimEnd());
					Console.WriteLine("out: " + actual);
					Console.WriteLine("excpected: " + excpectedIterations[i]);
				}
				Assert.AreEqual(excpectedIterations[i], actual);

				axiom = result;
			}

		}
	}
}

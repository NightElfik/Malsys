using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Malsys.Rewriters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.FSharp.Text.Lexing;
using Malsys.Compilers;
using Malsys.Parsing;
using Malsys.IO;
using Malsys.SourceCode.Printers;
using Microsoft.FSharp.Collections;
using Malsys.Expressions;

namespace Malsys.Tests.Rewriters {
	public static class GenericRewriterTests {
		public static void RunAllTests(IRewriter rewriter) {
			EmptyInputTests(rewriter);
			NoRewriteRulesTests(rewriter);
			NothingOnRightSideTests(rewriter);
		}

		public static void EmptyInputTests(IRewriter rewriter) {
			doTest("", "", "", rewriter);
		}

		public static void NoRewriteRulesTests(IRewriter rewriter) {
			doTest("", "x", "x", rewriter);
			doTest("", "a B c", "a B c", rewriter);
			doTest("", "a(30) B c(1 + 2)", "a(30) B c(3)", rewriter);
		}

		public static void NothingOnRightSideTests(IRewriter rewriter) {
			doTest("rewrite x to nothing;", "x x x x x", "", rewriter);
			doTest("rewrite x to ;", "x A x B x C x", "A B C", rewriter);
			doTest("rewrite x to ; rewrite y to nothing;", "x(20) y(1)", "", rewriter);
		}


		private static void doTest(string rewriteRules, string InputSymbols, string excpectedSymbols, IRewriter rewriter) {
			string input = "lsystem l {{ set axiom = {0}; {1} }}".Fmt(InputSymbols, rewriteRules);

			var lexBuff = LexBuffer<char>.FromString(input);
			var msgs = new MessagesCollection();
			var lsystemAst = ParserUtils.ParseLsystem(lexBuff, msgs, "testInput");

			if (msgs.ErrorOcured) {
				Assert.Fail("Failed to parse L-system. " + msgs.ToString());
			}

			var compiler = new LsystemCompiler(new InputCompiler(msgs));
			var lsystem = compiler.Compile(lsystemAst);

			if (!lsystem) {
				Assert.Fail("Failed to compile L-system." + msgs.ToString());
			}

			if (lsystem.Result.Symbols.Length != 1) {
				Assert.Fail("Excpected 1 symbol definition.");
			}

			var rrulesDict = RewriterUtils.CreateRrulesMap(lsystem.Result.RewriteRules);
			var axiom = lsystem.Result.Symbols[0].Value.Evaluate();

			rewriter.Initialize(rrulesDict,
				MapModule.Empty<string, Malsys.Expressions.IValue>(),
				MapModule.Empty<string, Malsys.FunctionDefinition>(),
				new Random());
			var result = rewriter.Rewrite(axiom);

			var writer = new IndentStringWriter();
			var printer = new CanonicPrinter(writer);
			foreach (var sym in result) {
				printer.Print(sym);
				writer.Write(" ");
			}

			string actual = writer.GetResult().TrimEnd();

			Assert.AreEqual(excpectedSymbols, actual);
		}
	}
}

using System;
using System.Collections.Generic;
using Malsys.Compilers;
using Malsys.Expressions;
using Malsys.Interpreters;
using Malsys.IO;
using Malsys.Parsing;
using Malsys.Rewriters;
using Malsys.Rewriters.Iterators;
using Malsys.SourceCode.Printers;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Text.Lexing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Malsys.Tests.Rewriters {
	public static class GenericRewriterIteratorTests {

		public static void EmptyInputTests(IRewriterIterator rewIt) {
			doTest(rewIt, 0, "", "", "");
			doTest(rewIt, 1, "", "", "");
			doTest(rewIt, 8, "", "", "");
		}

		public static void ManyItersTests(IRewriterIterator rewIt) {
			doTest(rewIt, 1000,
				"rewrite x(x) to x(x + 1);",
				"x(0)",
				"x(1000)");
		}

		public static void FibTests(IRewriterIterator rewIt) {
			doTest(rewIt, 42,
				"rewrite a(a) {b(b)} to a(b); rewrite {a(a)} b(b) to b(a + b);",
				"a(0) b(1)",
				"a(267914296) b(433494437)");
		}


		private static void doTest(IRewriterIterator rewIt, int iterations, string rewriteRules,
				string inputSymbols, string excpected) {
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

			if (lsystem.Result.Symbols.Length != 1) {
				Assert.Fail("Excpected 1 symbol definition.");
			}

			var symBuff = new SymbolsBuffer();
			var rrulesDict = RewriterUtils.CreateRrulesMap(lsystem.Result.RewriteRules);

			var rewriter = new SymbolRewriter();
			rewriter.Initialize(rrulesDict,
				MapModule.Empty<string, Malsys.Expressions.IValue>(),
				MapModule.Empty<string, Malsys.FunctionDefinition>(),
				0);

			IEnumerable<Symbol<IValue>> axiom = lsystem.Result.Symbols[0].Value.Evaluate();

			rewIt.Initialize(iterations, rewriter, symBuff, axiom);

			rewIt.Start();

			var result = symBuff.GetAndClear();
			var writer = new IndentStringWriter();
			var printer = new CanonicPrinter(writer);
			foreach (var sym in result) {
				printer.Print(sym);
				writer.Write(" ");
			}

			string actual = writer.GetResult().TrimEnd();
			if (excpected != actual) {
				var w = new IndentStringWriter();
				var p = new CanonicPrinter(w);
				foreach (var sym in axiom) {
					p.Print(sym);
					w.Write(" ");
				}
				Console.WriteLine("in: " + w.GetResult().TrimEnd());
				Console.WriteLine("out: " + actual);
				Console.WriteLine("excpected: " + excpected);
			}

			Assert.AreEqual(excpected, actual);
		}

	}
}

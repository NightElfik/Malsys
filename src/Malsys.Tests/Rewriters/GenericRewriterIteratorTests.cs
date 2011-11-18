using System;
using System.Collections.Generic;
using Malsys.Compilers;
using Malsys.Expressions;
using Malsys.IO;
using Malsys.Parsing;
using Malsys.Processing.Components;
using Malsys.Processing.Components.Interpreters;
using Malsys.Processing.Components.Rewriters;
using Malsys.SourceCode.Printers;
using Microsoft.FSharp.Text.Lexing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Malsys.Processing;
using Malsys.Evaluators;
using Microsoft.FSharp.Collections;
using Malsys.SemanticModel.Evaluated;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Compiled;

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
			var inAst = ParserUtils.ParseInputNoComents(lexBuff, msgs, "testInput");

			if (msgs.ErrorOcured) {
				Console.WriteLine("lsys: " + input);
				Assert.Fail("Failed to parse L-system. " + msgs.ToString());
			}

			var compiler = new InputCompiler(msgs);
			var inCompiled = compiler.CompileFromAst(inAst);

			if (msgs.ErrorOcured) {
				Assert.Fail("Failed to compile L-system." + msgs.ToString());
			}

			var exprEvaluator = new ExpressionEvaluator();
			var evaluator = new InputEvaluator(exprEvaluator);
			var inBlockEvaled = evaluator.Evaluate(inCompiled);

			if (inBlockEvaled.Lsystems.Count != 1) {
				Assert.Fail("L-system not defined.");
			}

			var lsysEvaluator = new LsystemEvaluator(exprEvaluator);
			var lsystem = lsysEvaluator.Evaluate(inBlockEvaled.Lsystems["l"], ImmutableList<IValue>.Empty,
				MapModule.Empty<string, IValue>(), MapModule.Empty<string, FunctionEvaledParams>());

			var context = new ProcessContext(lsystem, null, inBlockEvaled, exprEvaluator);

			var symBuff = new SymbolsBuffer();

			var rewriter = new SymbolRewriter() {
				OutputProcessor = rewIt,
				Context = context
			};

			IEnumerable<Symbol<IValue>> axiom = lsystem.SymbolsConstants["axiom"];

			rewIt.Rewriter = rewriter;
			rewIt.OutputProcessor = symBuff;
			rewIt.Axiom = axiom;
			rewIt.Iterations = iterations.ToConst();

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

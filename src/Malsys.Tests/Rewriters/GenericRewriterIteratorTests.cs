using System;
using Malsys.Compilers;
using Malsys.Evaluators;
using Malsys.IO;
using Malsys.Processing;
using Malsys.Processing.Components;
using Malsys.Processing.Components.Common;
using Malsys.Processing.Components.Rewriters;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Evaluated;
using Malsys.SourceCode.Printers;
using Microsoft.FSharp.Collections;
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

			var msgs = new MessageLogger();
			var inBlockEvaled = CompilerUtils.EvaluateLsystem(input);

			if (inBlockEvaled.Lsystems.Count != 1) {
				Assert.Fail("L-system not defined.");
			}

			var evaluator = new MalsysEvaluator();
			var lsysEvaluator = evaluator.Resolve<ILsystemEvaluator>();
			var lsystem = lsysEvaluator.Evaluate(inBlockEvaled.Lsystems["l"], ImmutableList<IValue>.Empty,
				MapModule.Empty<string, IValue>(), MapModule.Empty<string, FunctionEvaledParams>());

			var fm = new FilesManager("./");
			var context = new ProcessContext(lsystem, fm, inBlockEvaled, evaluator, msgs);

			var symBuff = new SymbolsMemoryBuffer();

			var rewriter = new SymbolRewriter();
			rewriter.Initialize(context);
			rewriter.OutputProcessor = rewIt;

			var axiom = lsystem.SymbolsConstants["axiom"];

			rewIt.Initialize(context);
			rewIt.Rewriter = rewriter;
			rewIt.OutputProcessor = symBuff;
			rewIt.Axiom = axiom;
			rewIt.Iterations = iterations.ToConst();

			rewIt.Start(false, new TimeSpan(0, 0, 1));

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

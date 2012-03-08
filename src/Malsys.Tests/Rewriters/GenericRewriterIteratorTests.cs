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
using Malsys.Processing.Output;

namespace Malsys.Tests.Rewriters {
	public static class GenericRewriterIteratorTests {

		public static void EmptyInputTests(IIterator rewIt) {
			doTest(rewIt, 0, "", "", "");
			doTest(rewIt, 1, "", "", "");
			doTest(rewIt, 8, "", "", "");
		}

		public static void ManyItersTests(IIterator rewIt) {
			doTest(rewIt, 1000,
				"rewrite x(x) to x(x + 1);",
				"x(0)",
				"x(1000)");
		}

		public static void FibTests(IIterator rewIt) {
			doTest(rewIt, 42,
				"rewrite a(a) {b(b)} to a(b); rewrite {a(a)} b(b) to b(a + b);",
				"a(0) b(1)",
				"a(267914296) b(433494437)");
		}


		private static void doTest(IIterator rewIt, int iterations, string rewriteRules,
				string inputSymbols, string excpected) {

			string input = "lsystem l {{ set symbols axiom = {0}; {1} }}".Fmt(inputSymbols, rewriteRules);

			var msgs = new MessageLogger();
			var inBlockEvaled = TestUtils.EvaluateLsystem(input);

			if (inBlockEvaled.Lsystems.Count != 1) {
				Assert.Fail("L-system not defined.");
			}

			var evaluator = new EvaluatorsContainer(TestUtils.ExpressionEvaluatorContext);
			var lsysEvaluator = evaluator.ResolveLsystemEvaluator();
			var lsystem = lsysEvaluator.Evaluate(inBlockEvaled.Lsystems["l"], ImmutableList<IValue>.Empty, TestUtils.ExpressionEvaluatorContext);

			var fm = new FileOutputProvider("./");
			var context = new ProcessContext(lsystem, fm, inBlockEvaled, inBlockEvaled.ExpressionEvaluatorContext, msgs);

			var symBuff = new SymbolsMemoryBuffer();

			var rewriter = new SymbolRewriter();
			rewriter.SymbolProvider = rewIt;
			rewriter.Initialize(context);

			rewIt.SymbolProvider = rewriter;
			rewIt.OutputProcessor = symBuff;
			rewIt.AxiomProvider = new SymbolProvider(lsystem.ComponentSymbolsAssigns["axiom"]);
			rewIt.Iterations = iterations.ToConst();
			rewIt.Initialize(context);

			rewIt.Start(false, TimeSpan.MaxValue);

			var result = symBuff.GetAndClear();
			var writer = new IndentStringWriter();
			var printer = new CanonicPrinter(writer);
			foreach (var sym in result) {
				printer.Print(sym);
				writer.Write(" ");
			}

			string actual = writer.GetResult().TrimEnd();
			if (excpected != actual) {
				Console.WriteLine("out: " + actual);
				Console.WriteLine("excepted: " + excpected);
			}

			Assert.AreEqual(excpected, actual);
		}

	}
}

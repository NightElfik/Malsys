using System;
using Malsys.Evaluators;
using Malsys.IO;
using Malsys.Processing;
using Malsys.Processing.Components.Common;
using Malsys.Processing.Components.RewriterIterators;
using Malsys.Processing.Components.Rewriters;
using Malsys.Processing.Output;
using Malsys.SemanticModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Malsys.SourceCode.Printers;

namespace Malsys.Tests.Rewriters {
	[TestClass]
	public class MemoryBufferedIteratorTests {

		[TestMethod]
		public void EmptyInputTests() {
			doTest(0, "", "", "");
			doTest(1, "", "", "");
			doTest(8, "", "", "");
		}

		[TestMethod]
		public void ManyItersTests() {
			doTest(1000,
				"rewrite x(x) to x(x + 1);",
				"x(0)",
				"x(1000)");
		}

		[TestMethod]
		public void FibTests() {
			doTest(42,
				"rewrite a(a) {b(b)} to a(b); rewrite {a(a)} b(b) to b(a + b);",
				"a(0) b(1)",
				"a(267914296) b(433494437)");
		}


		private static void doTest(int iterations, string rewriteRules,
				string inputSymbols, string excpected) {

			string input = "lsystem l {{ set symbols axiom = {0}; {1} }}".Fmt(inputSymbols, rewriteRules);

			var msgs = new MessageLogger();
			var inBlockEvaled = TestUtils.EvaluateInput(input);

			if (inBlockEvaled.Lsystems.Count != 1) {
				Assert.Fail("L-system not defined.");
			}

			var evaluator = new EvaluatorsContainer(TestUtils.ExpressionEvaluatorContext);
			var lsystem = TestUtils.EvaluateLsystem(inBlockEvaled.Lsystems["l"]);

			var fm = new FileOutputProvider("./");
			var context = new ProcessContext(lsystem, fm, inBlockEvaled, evaluator, inBlockEvaled.ExpressionEvaluatorContext,
				null, TimeSpan.MaxValue, null, msgs);

			var symBuff = new SymbolsMemoryBuffer();
			symBuff.Cleanup();

			var rewIt = new MemoryBufferedIterator();
			rewIt.Cleanup();
			var rewriter = new SymbolRewriter();
			rewriter.SymbolProvider = rewIt;
			rewriter.Initialize(context);

			rewIt.SymbolProvider = rewriter;
			rewIt.OutputProcessor = symBuff;
			rewIt.AxiomProvider = new SymbolProvider(lsystem.ComponentSymbolsAssigns["axiom"]);
			rewIt.Iterations = iterations.ToConst();

			rewIt.Initialize(context);

			rewIt.Start(false);

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

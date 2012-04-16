using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Malsys.Compilers;
using Malsys.Evaluators;
using Malsys.Processing;
using Malsys.Processing.Output;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Malsys.Tests.Processing {
	[TestClass]
	public class InheritanceTests {


		[TestMethod]
		public void EmptyTests() {
			doTest("");
		}

		[TestMethod]
		public void LsystemConstantInheritanceTests() {
			doTest("abstract lsystem Base { let a = 1; let b = 2; }"
				+ "lsystem Derived extends Base { let b = 3; set symbols axiom = A(a) B(b); }"
				+ "process all with SymbolPrinter;",
				"A(1) B(3)");
		}

		[TestMethod]
		public void LsystemFunctionInheritanceTests() {
			doTest("abstract lsystem Base { fun a(){ return 1; } fun b(){ return 2; } }"
				+ "lsystem Derived extends Base { fun b(){ return 3; } set symbols axiom = A(a()) B(b()); }"
				+ "process all with SymbolPrinter;",
				"A(1) B(3)");
		}

		[TestMethod]
		public void LsystemSymbolsInheritanceTests() {
			doTest("abstract lsystem Base { set symbols axiom = A; }"
				+ "lsystem Derived extends Base { set symbols axiom = B; }"
				+ "process all with SymbolPrinter;",
				"B");
			doTest("abstract lsystem Base { set symbols axiom = A; }"
				+ "lsystem Derived extends Base { }"
				+ "process all with SymbolPrinter;",
				"A");
		}

		[TestMethod]
		public void LsystemCompSetInheritanceTests() {
			doTest("abstract lsystem Base { set iterations = 1; }"
				+ "lsystem Derived extends Base { set iterations = 2; set symbols axiom = A; rewrite A to A(currentIteration); }"
				+ "process all with SymbolPrinter;",
				"A(2)");
			doTest("abstract lsystem Base { set iterations = 1; }"
				+ "lsystem Derived extends Base { set symbols axiom = A; rewrite A to A(currentIteration); }"
				+ "process all with SymbolPrinter;",
				"A(1)");
		}

		[TestMethod]
		public void LsystemRewriteRuleInheritanceTests() {
			doTest("abstract lsystem Base { rewrite A to B; rewrite X to Y; }"
				+ "lsystem Derived extends Base { rewrite A to C; set iterations = 1; set symbols axiom = A X; }"
				+ "process all with SymbolPrinter;",
				"C Y");
		}

		[TestMethod]
		public void LsystemInterpretationsInheritanceTests() {
			doTest("abstract lsystem Base { interpret A as X; interpret B as Y; }"
				+ "lsystem Derived extends Base { set debugInterpretation = true; interpret A as W; set symbols axiom = A B; }"
				+ "process all with ThreeJsRenderer;",
				"Begin processing",
				"A => W ERROR (undefined)",
				"B => Y ERROR (undefined)",
				"End processing");
		}

		[TestMethod]
		public void LsystemArgsInheritanceTests() {
			doTest("abstract lsystem Base(a, b = 2) { interpret A as A(a); interpret B as B(b); }"
				+ "lsystem Derived extends Base(1) { set debugInterpretation = true; set symbols axiom = A B; }"
				+ "process all with ThreeJsRenderer;",
				"Begin processing",
				"A => A(1) ERROR (undefined)",
				"B => B(2) ERROR (undefined)",
				"End processing");
			doTest("abstract lsystem Base(a, b = 2) { interpret A as A(a); interpret B as B(b); }"
				+ "lsystem Derived(c) extends Base(c) { set debugInterpretation = true; set symbols axiom = A B; }"
				+ "process all (1) with ThreeJsRenderer;",
				"Begin processing",
				"A => A(1) ERROR (undefined)",
				"B => B(2) ERROR (undefined)",
				"End processing");
		}



		private void doTest(string input, params string[] expectedOutputs) {

			var logger = new MessageLogger();

			var processManager = new ProcessManager(TestUtils.CompilersContainer, new EvaluatorsContainer(TestUtils.ExpressionEvaluatorContext), TestUtils.StdResolver);
			var inputEvalueated = processManager.CompileAndEvaluateInput(input, "testInput", logger);

			var actualOutputs = new List<string>();

			if (!logger.ErrorOccurred) {
				var outProvider = new InMemoryOutputProvider();
				processManager.ProcessInput(TestUtils.StdLib.JoinWith(inputEvalueated), outProvider, logger, TimeSpan.MaxValue);

				actualOutputs.AddRange(outProvider.GetOutputs().SelectMany(x => Encoding.UTF8.GetString(x.OutputData).Trim().SplitToLines()));

			}

			foreach (var msg in logger) {
				Console.WriteLine(msg.GetFullMessage());
			}

			actualOutputs.AddRange(logger.Select(msg => msg.Id));

			Console.WriteLine(string.Join(Environment.NewLine, actualOutputs));

			CollectionAssert.AreEquivalent(expectedOutputs, actualOutputs);

		}

	}
}

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
	public class InherenceTests {


		[TestMethod]
		public void EmptyTests() {
			doTest("");
		}

		[TestMethod]
		public void LsystemConstantInherenceTests() {
			doTest("abstract lsystem Base { let a = 1; let b = 2; }"
				+ "lsystem Derived extends Base { let b = 3; set symbols axiom = A(a) B(b); }"
				+ "process all with SymbolPrinter;",
				"A(1) B(3)");
		}

		[TestMethod]
		public void LsystemFunctionInherenceTests() {
			doTest("abstract lsystem Base { fun a(){ return 1; } fun b(){ return 2; } }"
				+ "lsystem Derived extends Base { fun b(){ return 3; } set symbols axiom = A(a()) B(b()); }"
				+ "process all with SymbolPrinter;",
				"A(1) B(3)");
		}

		[TestMethod]
		public void LsystemSymbolsInherenceTests() {
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
		public void LsystemCompSetInherenceTests() {
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
		public void LsystemRewriteRuleInherenceTests() {
			doTest("abstract lsystem Base { rewrite A to B; rewrite X to Y; }"
				+ "lsystem Derived extends Base { rewrite A to C; set iterations = 1; set symbols axiom = A X; }"
				+ "process all with SymbolPrinter;",
				"C Y");
		}

		[TestMethod]
		public void LsystemInterpretationsInherenceTests() {
			doTest("abstract lsystem Base { interpret A as X; interpret B as Y; }"
				+ "lsystem Derived extends Base { interpret A as W; set symbols axiom = A B; }"
				+ "process all with InterpretationDebugger;",
				"A => W",
				"B => Y");
		}

		[TestMethod]
		public void LsystemArgsInherenceTests() {
			doTest("abstract lsystem Base(a, b = 2) { interpret A as A(a); interpret B as B(b); }"
				+ "lsystem Derived extends Base(1) { set symbols axiom = A B; }"
				+ "process all with InterpretationDebugger;",
				"A => A(1)",
				"B => B(2)");
			doTest("abstract lsystem Base(a, b = 2) { interpret A as A(a); interpret B as B(b); }"
				+ "lsystem Derived(c) extends Base(c) { set symbols axiom = A B; }"
				+ "process all (1) with InterpretationDebugger;",
				"A => A(1)",
				"B => B(2)");
		}



		private void doTest(string input, params string[] expectedOutputs) {

			var logger = new MessageLogger();

			var processManager = new ProcessManager(new CompilersContainer(), new EvaluatorsContainer(TestUtils.ExpressionEvaluatorContext), TestUtils.StdResolver);
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

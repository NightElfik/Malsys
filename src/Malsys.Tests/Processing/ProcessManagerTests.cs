/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Malsys.Evaluators;
using Malsys.Processing;
using Malsys.Processing.Output;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Malsys.Tests.Process {
	[TestClass]
	public class ProcessManagerTests {

		[TestMethod]
		public void EmptyInputTests() {
			doTest("");
		}

		[TestMethod]
		public void GettablePropertiesInLsystemTests() {

			doTest(string.Join("\n", "lsystem l {",
					"set IValue = ConstantGet;",
					"}",
					"",
					"configuration Config {",
					"component Starter typeof StarterComponent;",
					"component Getter typeof GettablePropertiesComponent;",
					"component Setter typeof SettablePropertyAliasesComponent;",
					"}",
					"",
					"process l with Config;"),
				"StarterComponent",
				"GettablePropertiesComponent",
				"SettablePropertyAliasesComponent:8");

			doTest(string.Join("\n", "lsystem l {",
					 "set IValue = ValuesArrayGet;",
					 "}",
					 "",
					 "configuration Config {",
					 "component Starter typeof StarterComponent;",
					 "component Getter typeof GettablePropertiesComponent;",
					 "component Setter typeof SettablePropertyAliasesComponent;",
					 "}",
					 "",
					 "process l with Config;"),
				 "StarterComponent",
				 "GettablePropertiesComponent",
				 "SettablePropertyAliasesComponent:{1, 2, 3}");

			doTest(string.Join("\n", "lsystem l {",
					 "set IValue = IValueGet;",
					 "}",
					 "",
					 "configuration Config {",
					 "component Starter typeof StarterComponent;",
					 "component Getter typeof GettablePropertiesComponent;",
					 "component Setter typeof SettablePropertyAliasesComponent;",
					 "}",
					 "",
					 "process l with Config;"),
				 "StarterComponent",
				 "GettablePropertiesComponent",
				 "SettablePropertyAliasesComponent:42");

		}


		[TestMethod]
		public void LsystemStatsInProcessStatTests() {
			doTestStdLib(string.Join("\n", "lsystem l {",
					"set iterations = 5;",
					"set symbols axiom = W A;",
					"rewrite A to X A;",
					"}",
					"",
					"process l with SymbolPrinter",
					"set iterations = 2",
					"rewrite W to V;"),
				"V X X A");
			doTestStdLib(string.Join("\n", "lsystem l {",
					"set iterations = 2;",
					"set symbols axiom = A;",
					"rewrite A to X A;",
					"}",
					"",
					"process l with SymbolPrinter",
					"set symbols axiom = Y A",
					"rewrite A to Y A;"),
				"Y Y Y A");
		}


		private void doTest(string input, params string[] expectedOutputs) {

			var logger = new MessageLogger();

			var resolver = new ComponentResolver();
			Components.RegisterAllComponents(resolver);

			var processManager = new ProcessManager(TestUtils.CompilersContainer, new EvaluatorsContainer(new ExpressionEvaluatorContext()), resolver);
			var inputEvalueated = processManager.CompileAndEvaluateInput(input, "testInput", logger);

			var actualOutputs = new List<string>();

			if (!logger.ErrorOccurred) {
				var outProvider = new InMemoryOutputProvider();
				processManager.ProcessInput(inputEvalueated, outProvider, logger, TimeSpan.MaxValue);

				actualOutputs.AddRange(outProvider.GetOutputs().SelectMany(x => Encoding.UTF8.GetString(x.OutputData).Trim().SplitToLines()));

			}


			actualOutputs.AddRange(logger.Select(msg => msg.Id));

			Console.WriteLine(string.Join(Environment.NewLine, actualOutputs));

			CollectionAssert.AreEquivalent(expectedOutputs, actualOutputs);

		}

		private void doTestStdLib(string input, params string[] expectedOutputs) {

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

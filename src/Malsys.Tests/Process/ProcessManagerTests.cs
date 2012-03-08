using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Malsys.Compilers;
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


		private void doTest(string input, params string[] excpectedOutputs) {

			var logger = new MessageLogger();

			var resolver = new ComponentResolver();
			Components.RegisterAllComponents(resolver);

			var processManager = new ProcessManager(new CompilersContainer(), new EvaluatorsContainer(new ExpressionEvaluatorContext()), resolver);
			var inputEvalueated = processManager.CompileAndEvaluateInput(input, "testInput", logger);

			var actualOutputs = new List<string>();

			if (!logger.ErrorOccurred) {
				var outProvider = new InMemoryOutputProvider();
				processManager.ProcessInput(inputEvalueated, outProvider, logger, TimeSpan.MaxValue);

				actualOutputs.AddRange(outProvider.GetOutputs().SelectMany(x => Encoding.UTF8.GetString(x.OutputData).Trim().SplitToLines()));

			}


			actualOutputs.AddRange(logger.Select(msg => msg.Id));

			Console.WriteLine(string.Join(Environment.NewLine, actualOutputs));

			CollectionAssert.AreEquivalent(excpectedOutputs, actualOutputs);

		}

	}
}

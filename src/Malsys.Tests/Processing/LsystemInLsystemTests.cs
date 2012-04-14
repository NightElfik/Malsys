using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Malsys.Evaluators;
using Malsys.Media;
using Malsys.Processing;
using Malsys.Processing.Components.Renderers;
using Malsys.Processing.Output;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Malsys.Tests.Processing {
	[TestClass]
	public class LsystemInLsystemTests {

		private string init = DebugRenderer2D.InitName;
		private string move = DebugRenderer2D.MoveToName;
		private string draw = DebugRenderer2D.DrawToName;
		private string poly = DebugRenderer2D.DrawPolygonName;

		[TestMethod]
		public void BasicTests() {
			doTestStdLib(string.Join("\n",
					"abstract lsystem Inner {",
						"set symbols axiom = B;",
						"interpret B as MoveForward(1);",
					"}",
					"",
					"lsystem Main {",
						"set symbols axiom = A;",
						"interpret A as lsystem Inner;",
					"}",
					"",
					"process all with SvgRenderer use DebugRenderer2D as Renderer;"),
				new Data(init),
				new Data(move));
		}

		[TestMethod]
		public void InnerIteratorTests() {
			doTestStdLib(string.Join("\n",
					"abstract lsystem Inner {",
						"set iterations = 10;",
						"set symbols axiom = B(0);",
						"rewrite B(x) to B(x+1);",
						"interpret B(x) as MoveForward(x);",
					"}",
					"",
					"lsystem Main {",
						"set symbols axiom = A A;",
						"interpret A as lsystem Inner;",
					"}",
					"",
					"process all with SvgRenderer use DebugRenderer2D as Renderer;"),
				new Data(init, 0, 0),
				new Data(move, 10, 0),
				new Data(move, 20, 0));
		}

		[TestMethod]
		public void ArgumentsTests() {
			doTestStdLib(string.Join("\n",
					"abstract lsystem Inner(x, y = 1) {",
						"set iterations = x;",
						"set symbols axiom = B(y);",
						"rewrite B(x) to B(x+1);",
						"interpret B(x) as MoveForward(x);",
					"}",
					"",
					"lsystem Main {",
						"set symbols axiom = A(9) A(5, 5);",
						"interpret A as lsystem Inner;",
					"}",
					"",
					"process all with SvgRenderer use DebugRenderer2D as Renderer;"),
				new Data(init, 0, 0),
				new Data(move, 10, 0),
				new Data(move, 20, 0));
		}

		[TestMethod]
		public void LsystemInLsystemInLsystemTests() {
			doTestStdLib(string.Join("\n",
					"abstract lsystem InnerInner(x) {",
						"set symbols axiom = B;",
						"interpret B as MoveForward(x);",
					"}",
					"",
					"abstract lsystem Inner(x, y) {",
						"set symbols axiom = A(x) A(y);",
						"interpret A as lsystem InnerInner;",
					"}",
					"",
					"lsystem Main {",
						"set symbols axiom = X(10, 1) X(1, 10);",
						"interpret X as lsystem Inner;",
					"}",
					"",
					"process all with SvgRenderer use DebugRenderer2D as Renderer;"),
				new Data(init, 0, 0),
				new Data(move, 10, 0),
				new Data(move, 11, 0),
				new Data(move, 12, 0),
				new Data(move, 22, 0));
		}


		private void doTestStdLib(string input, params Data[] excpectedOutput) {

			var logger = new MessageLogger();

			var processManager = new ProcessManager(TestUtils.CompilersContainer, new EvaluatorsContainer(TestUtils.ExpressionEvaluatorContext), TestUtils.StdResolver);
			var inputEvalueated = processManager.CompileAndEvaluateInput(input, "testInput", logger);

			var actualOutputs = new List<string>();

			var outProvider = new InMemoryOutputProvider();
			processManager.ProcessInput(TestUtils.StdLib.JoinWith(inputEvalueated), outProvider, logger, TimeSpan.MaxValue);

			if (logger.ErrorOccurred) {
				Console.WriteLine(logger.AllMessagesToFullString());
				Assert.Fail();
			}

			var output = outProvider.GetOutputs().Single();
			var outStringLines = Encoding.UTF8.GetString(output.OutputData).Trim().SplitToLines();

			int i = 0;
			foreach (string outLine in outStringLines) {
				string[] strDataArr = outLine.Split('|');

				Assert.AreEqual(excpectedOutput[i].Name, strDataArr[0]);

				if (excpectedOutput[i].IsPointValid) {
					Assert.IsTrue(excpectedOutput[i].Point.IsEpsilonEqualTo(Point.Parse(strDataArr[1])));
				}
				i++;
			}

			if (logger.ErrorOccurred) {
				Console.WriteLine(logger.AllMessagesToFullString());
				Assert.Fail();
			}

			Assert.AreEqual(i, excpectedOutput.Length);

		}


		private struct Data {

			public string Name;
			public Point Point;

			public bool IsPointValid { get; private set; }


			public Data(string name)
				: this() {

				Name = name;
				Point.X = 0;
				Point.Y = 0;

				IsPointValid = false;
			}

			public Data(string name, double x, double y)
				: this() {

				Name = name;
				Point.X = x;
				Point.Y = y;

				IsPointValid = true;
			}

			public Data(string name, Point pt)
				: this() {

				Name = name;
				Point = pt;

				IsPointValid = true;
			}

		}

	}
}

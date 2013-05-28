// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using Malsys.Evaluators;
using Malsys.Media;
using Malsys.Processing;
using Malsys.Processing.Components.Interpreters;
using Malsys.Processing.Components.Renderers;
using Malsys.Processing.Output;
using Malsys.Reflection.Components;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Evaluated;
using Microsoft.FSharp.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Malsys.Tests.Interpreters {
	[TestClass]
	public class TurtleInterpreterTests {

		private static ImmutableList<OptionalParameterEvaled> emptyParams = ImmutableList<OptionalParameterEvaled>.Empty;
		private static ImmutableList<IExpression> emptyInstrParams = ImmutableList<IExpression>.Empty;

		private FSharpMap<string, SymbolInterpretationEvaled> symToInstr;

		private string init = DebugRenderer3D.InitName;
		private string move = DebugRenderer3D.MoveToName;
		private string draw = DebugRenderer3D.DrawToName;
		private string poly = DebugRenderer3D.DrawPolygonName;


		[TestInitialize]
		public void Initialize() {

			var intArr = new SymbolInterpretationEvaled[] {
				new SymbolInterpretationEvaled("F", emptyParams, "DrawForward", emptyInstrParams),
				new SymbolInterpretationEvaled("f", emptyParams, "MoveForward", emptyInstrParams),
				new SymbolInterpretationEvaled("+", emptyParams, "Yaw", emptyInstrParams),
				new SymbolInterpretationEvaled("^", emptyParams, "Pitch", emptyInstrParams),
				new SymbolInterpretationEvaled("/", emptyParams, "Roll", emptyInstrParams)
			};

			symToInstr = MapModule.Empty<string, SymbolInterpretationEvaled>();
			foreach (var item in intArr) {
				symToInstr = symToInstr.Add(item.Symbol, item);
			}
		}

		[TestMethod]
		public void EmptyInputTest() {
			doTest("", new Data[] { new Data(init, new Point3D(0, 0, 0), Quaternion.Identity) });
		}

		[TestMethod]
		public void BasicYawTest() {
			doTest(new Point3D(0, 0, 0), Quaternion.Identity,
				"F(1) +(90) F(1) +(90) F(1) +(90) F(1) +(90) F(1) +(90) F(1) +(90) F(1)",
				new Data[] {
					new Data(init, new Point3D(0, 0, 0), Quaternion.Identity),
					new Data(draw, new Point3D(1, 0, 0), new Quaternion(Math3D.UnitY, 0)),
					new Data(draw, new Point3D(1, 0, -1), new Quaternion(Math3D.UnitY, 90)),
					new Data(draw, new Point3D(0, 0, -1), new Quaternion(Math3D.UnitY, 180)),
					new Data(draw, new Point3D(0, 0, 0), new Quaternion(Math3D.UnitY, 270)),
					new Data(draw, new Point3D(1, 0, 0), new Quaternion(Math3D.UnitY, 0)),
					new Data(draw, new Point3D(1, 0, -1), new Quaternion(Math3D.UnitY, 90)),
					new Data(draw, new Point3D(0, 0, -1), new Quaternion(Math3D.UnitY, 180))
				});
		}

		[TestMethod]
		public void SquaresYawTest() {
			doTestSquare(new Quaternion(Math3D.UnitX, 90), "+", new Point3D(1, 0, 0), new Point3D(0, 1, 0));

			doTestSquare(new Quaternion(Math3D.UnitY, 90), "+", new Point3D(0, 0, -1), new Point3D(-1, 0, 0));

			doTestSquare(new Quaternion(Math3D.UnitZ, 90), "+", new Point3D(0, 1, 0), new Point3D(0, 0, -1));
		}

		[TestMethod]
		public void BasicPitchTest() {
			doTest("F(1) ^(90) F(1) ^(90) F(1) ^(90) F(1) ^(90) F(1) ^(90) F(1) ^(90) F(1)", new Data[] {
				new Data(init, new Point3D(0, 0, 0), Quaternion.Identity),
				new Data(draw, new Point3D(1, 0, 0), new Quaternion(Math3D.UnitZ, 0)),
				new Data(draw, new Point3D(1, 1, 0), new Quaternion(Math3D.UnitZ, 90)),
				new Data(draw, new Point3D(0, 1, 0), new Quaternion(Math3D.UnitZ, 180)),
				new Data(draw, new Point3D(0, 0, 0), new Quaternion(Math3D.UnitZ, 270)),
				new Data(draw, new Point3D(1, 0, 0), new Quaternion(Math3D.UnitZ, 0)),
				new Data(draw, new Point3D(1, 1, 0), new Quaternion(Math3D.UnitZ, 90)),
				new Data(draw, new Point3D(0, 1, 0), new Quaternion(Math3D.UnitZ, 180))
			});
		}

		[TestMethod]
		public void SquaresPitchTest() {
			doTestSquare(new Quaternion(Math3D.UnitX, 90), "^", new Point3D(1, 0, 0), new Point3D(0, 0, 1));

			doTestSquare(new Quaternion(Math3D.UnitY, 90), "^", new Point3D(0, 0, -1), new Point3D(0, 1, 0));

			doTestSquare(new Quaternion(Math3D.UnitZ, 90), "^", new Point3D(0, 1, 0), new Point3D(-1, 0, 0));
		}

		[TestMethod]
		public void RollTest() {
			doTest("F(1) /(90) F(1) /(90) F(1) /(90) F(1) /(90) F(1) /(90) F(1) /(90) F(1)", new Data[] {
				new Data(init, new Point3D(0, 0, 0), Quaternion.Identity),
				new Data(draw, new Point3D(1, 0, 0), new Quaternion(Math3D.UnitX, 0)),
				new Data(draw, new Point3D(2, 0, 0), new Quaternion(Math3D.UnitX, 90)),
				new Data(draw, new Point3D(3, 0, 0), new Quaternion(Math3D.UnitX, 180)),
				new Data(draw, new Point3D(4, 0, 0), new Quaternion(Math3D.UnitX, 270)),
				new Data(draw, new Point3D(5, 0, 0), new Quaternion(Math3D.UnitX, 0)),
				new Data(draw, new Point3D(6, 0, 0), new Quaternion(Math3D.UnitX, 90)),
				new Data(draw, new Point3D(7, 0, 0), new Quaternion(Math3D.UnitX, 180))
			});
		}

		[TestMethod]
		public void YawRollTest() {

			string symbolsTemplate = "{0}(1) /(90) +(90) {0}(1) /(-90) +(-90) {0}(1)";

			doTest(symbolsTemplate.Fmt("f"), new Data[] {
				new Data(init, 0, 0, 0),
				new Data(move, 1, 0, 0),
				new Data(move, 1, 1, 0),
				new Data(move, 1, 1, 1)
			});

			doTest(symbolsTemplate.Fmt("F"), new Data[] {
				new Data(init, 0, 0, 0),
				new Data(draw, 1, 0, 0),
				new Data(draw, 1, 1, 0),
				new Data(draw, 1, 1, 1)
			});
		}

		[TestMethod]
		public void YawPitchTest() {

			string symbolsTemplate = "{0}(1) ^(90) {0}(1) ^(-90) +(-90) {0}(1)";

			doTest(symbolsTemplate.Fmt("f"), new Data[] {
				new Data(init, 0, 0, 0),
				new Data(move, 1, 0, 0),
				new Data(move, 1, 1, 0),
				new Data(move, 1, 1, 1)
			});

			doTest(symbolsTemplate.Fmt("F"), new Data[] {
				new Data(init, 0, 0, 0),
				new Data(draw, 1, 0, 0),
				new Data(draw, 1, 1, 0),
				new Data(draw, 1, 1, 1)
			});
		}

		[TestMethod]
		public void PitchRollTest() {

			string symbolsTemplate = "{0}(1) ^(90) {0}(1) /(90) ^(90) {0}(1)";

			doTest(symbolsTemplate.Fmt("f"), new Data[] {
				new Data(init, 0, 0, 0),
				new Data(move, 1, 0, 0),
				new Data(move, 1, 1, 0),
				new Data(move, 1, 1, 1)
			});

			doTest(symbolsTemplate.Fmt("F"), new Data[] {
				new Data(init, 0, 0, 0),
				new Data(draw, 1, 0, 0),
				new Data(draw, 1, 1, 0),
				new Data(draw, 1, 1, 1)
			});
		}

		[TestMethod]
		public void PitchCircleTest() {
			doTestCircle("^", 3, Quaternion.Identity);
			doTestCircle("^", 17, Quaternion.Identity);

			doTestCircle("^", 31, new Quaternion(new Vector3D(1, 2, 3), 42));
			doTestCircle("^", 101, new Quaternion(new Vector3D(-2, 5, 0), -28.3));
		}

		[TestMethod]
		public void YawCircleTest() {
			doTestCircle("+", 3, Quaternion.Identity);
			doTestCircle("+", 17, Quaternion.Identity);

			doTestCircle("+", 31, new Quaternion(new Vector3D(1, 2, 3), 42));
			doTestCircle("+", 101, new Quaternion(new Vector3D(-2, 5, 0), -28.3));
		}




		private void doTestSquare(Quaternion startRotation, string rotationSymbol, Point3D firstStep, Point3D thirdStep) {

			Point3D origin = new Point3D(0, 0, 0);
			Point3D secondStep = Math3D.AddPoints(firstStep, thirdStep);

			string symbolsTemplate = "{1}(1) {0}(90) {1}(1) {0}(90) {1}(1) {0}(90) {1}(1) {0}(90) {1}(1) {0}(90) {1}(1) {0}(90) {1}(1)";

			doTest(origin, startRotation,
				symbolsTemplate.Fmt(rotationSymbol, "f"),
				new Data[] {
					new Data(init, origin),
					new Data(move, firstStep),
					new Data(move, secondStep),
					new Data(move, thirdStep),
					new Data(move, origin),
					new Data(move, firstStep),
					new Data(move, secondStep),
					new Data(move, thirdStep)
				});

			doTest(origin, startRotation,
				 symbolsTemplate.Fmt(rotationSymbol, "F"),
				 new Data[] {
					new Data(init, origin),
					new Data(draw, firstStep),
					new Data(draw, secondStep),
					new Data(draw, thirdStep),
					new Data(draw, origin),
					new Data(draw, firstStep),
					new Data(draw, secondStep),
					new Data(draw, thirdStep)
				});

		}

		private void doTestCircle(string rotationSymbol, int steps, Quaternion startRotation) {

			Point3D origin = new Point3D(0, 0, 0);
			double angleStep = 360.0 / steps;

			string symbolsMove = "f(1) {0}({1}) ".FmtInvariant(rotationSymbol, angleStep).Repeat(steps);

			doTest(origin, startRotation, symbolsMove,
				new Data[] { new Data(init, origin) }
					.Concat(Enumerable.Repeat(new Data(move), steps - 1))
					.Concat(new Data[] { new Data(move, origin) }).ToArray());

			string symbolsDraw = "F(1) {0}({1}) ".FmtInvariant(rotationSymbol, angleStep).Repeat(steps);

			doTest(origin, startRotation, symbolsDraw,
				new Data[] { new Data(init, origin) }
					.Concat(Enumerable.Repeat(new Data(draw), steps - 1))
					.Concat(new Data[] { new Data(draw, origin) }).ToArray());

		}

		private void doTest(string inputSymbols, Data[] excpectedOutput) {
			doTest(new Point3D(0, 0, 0), Quaternion.Identity, inputSymbols, excpectedOutput);
		}

		private void doTest(Quaternion startRotation, string inputSymbols, Data[] excpectedOutput) {
			doTest(new Point3D(0, 0, 0), startRotation, inputSymbols, excpectedOutput);
		}

		private void doTest(Point3D startPoint, Quaternion startRotation, string inputSymbols, Data[] excpectedOutput) {

			var testedInterpreter = new TurtleInterpreter();
			testedInterpreter.Reset();

			var symbols = TestUtils.CompileSymbols(inputSymbols);

			var lsystem = new LsystemEvaled("", false, null, TestUtils.ExpressionEvaluatorContext, null, null, symToInstr, null, null);

			var logger = new MessageLogger();
			var evaluator = new EvaluatorsContainer(TestUtils.ExpressionEvaluatorContext);
			var symbolEvaluator = evaluator.Resolve<ISymbolEvaluator>();
			var outProvider = new InMemoryOutputProvider();

			var intMeta = new ComponentMetadataDumper().GetMetadata(testedInterpreter.GetType(), logger);
			var component = new ConfigurationComponent("interpreter", testedInterpreter, intMeta);
			var componentsGraph = MapModule.Empty<string, ConfigurationComponent>().Add(component.Name, component);

			var context = new ProcessContext(lsystem, outProvider, null, evaluator,
				TestUtils.ExpressionEvaluatorContext, null, TimeSpan.MaxValue, componentsGraph);

			var caller = new InterpreterCaller();
			caller.Reset();
			caller.Initialize(context);

			var renderer = new DebugRenderer3D();
			renderer.Reset();
			renderer.Initialize(context);
			testedInterpreter.Renderer = renderer;

			testedInterpreter.Origin = new ValuesArray(new ImmutableList<Constant>(
				startPoint.X.ToConst(), startPoint.Y.ToConst(), startPoint.Z.ToConst()));
			testedInterpreter.RotationQuaternion = new ValuesArray(new ImmutableList<Constant>(
				startRotation.W.ToConst(), startRotation.X.ToConst(), startRotation.Y.ToConst(), startRotation.Z.ToConst()));

			testedInterpreter.Initialize(context);

			caller.BeginProcessing(false);

			foreach (var sym in symbols) {
				caller.ProcessSymbol(symbolEvaluator.Evaluate(sym, TestUtils.ExpressionEvaluatorContext));
			}

			caller.EndProcessing();

			var output = outProvider.GetOutputs().Single();
			var outStringLines = Encoding.UTF8.GetString(output.OutputData).Trim().SplitToLines();

			int i = 0;
			foreach (string outLine in outStringLines) {
				string[] strDataArr = outLine.Split('|');

				Assert.AreEqual(excpectedOutput[i].Name, strDataArr[0]);

				if (excpectedOutput[i].IsPointValid) {
					Assert.IsTrue(excpectedOutput[i].Point.IsAlmostEqualTo(Point3D.Parse(strDataArr[1])));
				}

				if (excpectedOutput[i].IsRotationValid) {
					Quaternion actualRotation = Quaternion.Parse(strDataArr[2]);
					actualRotation.Invert();
					Quaternion excpectedUnit = excpectedOutput[i].Rotation * actualRotation;
					Assert.IsTrue(excpectedUnit.Angle < 0.001 || excpectedUnit.Angle > 359.999);
				}
				i++;
			}

			Assert.AreEqual(i, excpectedOutput.Length);

		}


		private struct Data {

			public string Name;
			public Point3D Point;
			public Quaternion Rotation;

			public bool IsPointValid { get; private set; }
			public bool IsRotationValid { get; private set; }


			public Data(string name)
				: this() {

				Name = name;
				Point.X = 0;
				Point.Y = 0;
				Point.Z = 0;
				Rotation = Quaternion.Identity;

				IsPointValid = false;
				IsRotationValid = false;
			}

			public Data(string name, double x, double y, double z)
				: this() {

				Name = name;
				Point.X = x;
				Point.Y = y;
				Point.Z = z;
				Rotation = Quaternion.Identity;

				IsPointValid = true;
				IsRotationValid = false;
			}

			public Data(string name, Point3D pt)
				: this() {

				Name = name;
				Point = pt;
				Rotation = Quaternion.Identity;

				IsPointValid = true;
				IsRotationValid = false;
			}

			public Data(string name, Point3D pt, Quaternion rotation)
				: this() {

				Name = name;
				Point = pt;
				Rotation = rotation;

				IsPointValid = true;
				IsRotationValid = true;
			}

		}

	}
}

// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using Malsys.Evaluators;
using Malsys.IO;
using Malsys.Processing;
using Malsys.Processing.Components;
using Malsys.Processing.Components.Interpreters;
using Malsys.Processing.Output;
using Malsys.Reflection.Components;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Compiled.Expressions;
using Malsys.SemanticModel.Evaluated;
using Malsys.SourceCode.Printers;
using Microsoft.FSharp.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Malsys.Tests.Interpreters {
	[TestClass]
	public class InterpreterCallerTests {

		private SymbolInterpretationEvaled buildSymbolInterpretation(string symbol, List<OptionalParameterEvaled> parameters,
				string instructionName, List<IExpression> instructionParameters) {

			return new SymbolInterpretationEvaled(null) {
				Symbol = symbol,
				Parameters = parameters ?? new List<OptionalParameterEvaled>(),
				InstructionName = instructionName,
				InstructionParameters = instructionParameters ?? new List<IExpression>(),
				InstructionIsLsystemName = false,
			};
		}

		[TestMethod]
		public void EmptyInputTests() {
			doTest(new SymbolInterpretationEvaled[0], "", new string[0]);
		}

		[TestMethod]
		public void ExistingActionsTests() {

			var sym2Instr = new SymbolInterpretationEvaled[] {
				buildSymbolInterpretation("A", null, "ActionA", null),
				buildSymbolInterpretation("B", null, "ActionB", null),
				buildSymbolInterpretation("C", null, "ActionC", null)
			};

			doTest(sym2Instr,
				"A",
				new string[] { "ActionA" });

			doTest(sym2Instr,
				"A B A C",
				new string[] { "ActionA", "ActionB", "ActionA", "ActionC" });
		}

		[TestMethod]
		public void UnknownSymbolsTests() {

			var sym2Instr = new SymbolInterpretationEvaled[] {
				buildSymbolInterpretation("A", null, "ActionA", null)
			};

			doTest(sym2Instr,
				"A x A x y",
				new string[] { "ActionA", "ActionA" });
		}

		[TestMethod]
		[ExpectedException(typeof(InterpretationException))]
		public void UnknownActionsTests() {
			var sym2Instr = new SymbolInterpretationEvaled[] {
			    buildSymbolInterpretation("A", null, "ActionA", null),
			    buildSymbolInterpretation("C", null, "??", null)
			};

			doTest(sym2Instr,
				"A C A C C",
				new string[] { "ActionA", "ActionA" });
		}

		[TestMethod]
		public void ParametersFromSymbolTests() {

			var sym2Instr = new SymbolInterpretationEvaled[] {
				buildSymbolInterpretation("A", null, "ActionA", null)
			};

			doTest(sym2Instr,
				"A A(8) A(8, 8)",
				new string[] { "ActionA", "ActionA(8)", "ActionA(8, 8)" });

			doTest(sym2Instr,
				"A(5) A({1, 2}) A(1, 1, 1) A(1, {0, 0})",
				new string[] { "ActionA(5)", "ActionA({1, 2})", "ActionA(1, 1, 1)", "ActionA(1, {0, 0})" });
		}

		[TestMethod]
		public void ParametersTests() {

			var opParamsX = new List<OptionalParameterEvaled>() {
				new OptionalParameterEvaled(null) { Name = "x" },
			};
			var opParamsXY = new List<OptionalParameterEvaled>() {
				new OptionalParameterEvaled(null) { Name = "x" },
				new OptionalParameterEvaled(null) { Name = "y" },
			};

			var sym2Instr = new SymbolInterpretationEvaled[] {
				buildSymbolInterpretation("A", opParamsX, "ActionA", new List<IExpression>() { 5.ToConst() }),
				buildSymbolInterpretation("B", opParamsX, "ActionB", new List<IExpression>() { "x".ToVar() }),
				buildSymbolInterpretation("C", opParamsXY, "ActionC", new List<IExpression>() { "x".ToVar(), 5.ToConst(), "y".ToVar() }),
			};

			doTest(sym2Instr,
				"A(8) A(8, 8)",
				new string[] { "ActionA(5)", "ActionA(5)" });

			doTest(sym2Instr,
				"B(8) B(8, 8)",
				new string[] { "ActionB(8)", "ActionB(8)" });

			doTest(sym2Instr,
				"C(8, 9) C(8, 9, 10)",
				new string[] { "ActionC(8, 5, 9)", "ActionC(8, 5, 9)" });
		}

		[TestMethod]
		public void NativelyOptionalParametersTests() {

			var sym2Instr = new SymbolInterpretationEvaled[] {
				buildSymbolInterpretation("A", null, "ActionA", new List<IExpression>() { 5.ToConst() }),
				buildSymbolInterpretation("B", null, "ActionB", new List<IExpression>() { 5.ToConst(), 4.ToConst() }),
			};

			doTest(sym2Instr,
				"A A(8) A(8, 8)",
				new string[] { "ActionA(5)", "ActionA(8)", "ActionA(8, 8)" });

			doTest(sym2Instr,
				"B B(8) B(8, 8) B(8, 8, 8)",
				new string[] { "ActionB(5, 4)", "ActionB(8, 4)", "ActionB(8, 8)", "ActionB(8, 8, 8)" });
		}

		[TestMethod]
		public void OptionalParametersTests() {

			var opParamsX = new List<OptionalParameterEvaled>() {
				new OptionalParameterEvaled(null) { Name = "x", DefaultValue = 1.ToConst() },
			};
			var opParamsXY = new List<OptionalParameterEvaled>() {
				new OptionalParameterEvaled(null) { Name = "x", DefaultValue = 1.ToConst() },
				new OptionalParameterEvaled(null) { Name = "y", DefaultValue = 2.ToConst() },
			};

			var sym2Instr = new SymbolInterpretationEvaled[] {
				buildSymbolInterpretation("A", opParamsX, "ActionA", new List<IExpression>() { "x".ToVar() }),
				buildSymbolInterpretation("B", opParamsXY, "ActionB", new List<IExpression>() { "x".ToVar(), "y".ToVar() }),
			};

			doTest(sym2Instr,
				"A A(8) A(8, 8)",
				new string[] { "ActionA(1)", "ActionA(8)", "ActionA(8)" });

			doTest(sym2Instr,
				"B B(8) B(8, 8) B(8, 8, 8)",
				new string[] { "ActionB(1, 2)", "ActionB(8, 2)", "ActionB(8, 8)", "ActionB(8, 8)" });
		}


		private static void doTest(SymbolInterpretationEvaled[] symbolToInstr,
				string inputSymbols, string[] excpected) {

			var caller = new InterpreterCaller();
			caller.Reset();
			var symbols = TestUtils.CompileSymbols(inputSymbols);

			var evaluator = new EvaluatorsContainer(TestUtils.ExpressionEvaluatorContext);
			var symToInstr = MapModule.Empty<string, SymbolInterpretationEvaled>();
			foreach (var item in symbolToInstr) {
				symToInstr = symToInstr.Add(item.Symbol, item);
			}
			var lsystem = new LsystemEvaled("Test L-system") {
				ExpressionEvaluatorContext = TestUtils.ExpressionEvaluatorContext,
				SymbolsInterpretation = symToInstr,
			};
			var logger = new MessageLogger();

			var dummyInterpreter = new DummyInterpreter();
			dummyInterpreter.Reset();
			var intMeta = new ComponentMetadataDumper().GetMetadata(dummyInterpreter.GetType(), logger);
			var component = new ConfigurationComponent("interpreter", dummyInterpreter, intMeta);
			var componentsGraph = MapModule.Empty<string, ConfigurationComponent>().Add(component.Name, component);

			var context = new ProcessContext(lsystem, new InMemoryOutputProvider(), null, evaluator,
				TestUtils.ExpressionEvaluatorContext, null, TimeSpan.MaxValue, componentsGraph);

			caller.Initialize(context);

			var symbolEvaluator = evaluator.Resolve<ISymbolEvaluator>();

			foreach (var sym in symbols) {
				caller.ProcessSymbol(symbolEvaluator.Evaluate(sym, TestUtils.ExpressionEvaluatorContext));
			}

			var actual = dummyInterpreter.Actions.ToArray();

			if (excpected.Length != actual.Length || !excpected.SequenceEqual(actual)) {
				Console.WriteLine("in: " + inputSymbols);
				Console.Write("actual:");
				foreach (var a in actual) {
					Console.Write(" " + a);
				}

				Assert.Fail();
			}
		}


		private class DummyInterpreter : IInterpreter {

			public List<string> Actions = new List<string>();
			private IndentStringWriter writer;
			private CanonicPrinter printer;


			public DummyInterpreter() {
				writer = new IndentStringWriter();
				printer = new CanonicPrinter(writer);
			}

			public IRenderer Renderer {
				set { throw new NotImplementedException(); }
			}


			#region IComponent Members


			public IMessageLogger Logger { get; set; }

			public void Reset() { }

			public void Initialize(ProcessContext context) { }

			public void Cleanup() { }

			public void Dispose() { }


			public bool RequiresMeasure { get { return false; } }

			public void BeginProcessing(bool measuring) { }

			public void EndProcessing() { }

			#endregion IComponent Members


			[SymbolInterpretation]
			public void ActionA(ArgsStorage args) {
				Actions.Add("ActionA" + printParams(args));
			}

			[SymbolInterpretation]
			public void ActionB(ArgsStorage args) {
				Actions.Add("ActionB" + printParams(args));
			}

			[SymbolInterpretation]
			public void ActionC(ArgsStorage args) {
				Actions.Add("ActionC" + printParams(args));
			}

			private string printParams(ArgsStorage args) {
				if (args.IsEmpty) {
					return "";
				}

				writer.Write("(");
				printer.PrintSeparated(args, v => printer.Print(v));
				writer.Write(")");
				string result = writer.GetResultAndClear();

				return result;
			}
		}
	}
}

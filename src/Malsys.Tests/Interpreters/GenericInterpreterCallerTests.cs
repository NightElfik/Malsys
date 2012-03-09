using System;
using System.Collections.Generic;
using System.Linq;
using Malsys.Compilers;
using Malsys.Evaluators;
using Malsys.IO;
using Malsys.Parsing;
using Malsys.Processing;
using Malsys.Processing.Components;
using Malsys.Processing.Components.Interpreters;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Evaluated;
using Malsys.SourceCode.Printers;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Text.Lexing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Malsys.SemanticModel.Compiled.Expressions;
using Malsys.Processing.Output;

namespace Malsys.Tests.Interpreters {
	public static class GenericInterpreterCallerTests {

		private static ImmutableList<OptionalParameterEvaled> emptyParams = ImmutableList<OptionalParameterEvaled>.Empty;
		private static ImmutableList<IExpression> emptyInstrParams = ImmutableList<IExpression>.Empty;


		public static void EmptyInputTests(IInterpreterCaller caller) {

			doTest(caller, new SymbolInterpretationEvaled[0], "", new string[0]);
		}

		public static void ExistingActionsTests(IInterpreterCaller caller) {

			var sym2Instr = new SymbolInterpretationEvaled[] {
				new SymbolInterpretationEvaled("A", emptyParams, "ActionA", emptyInstrParams),
				new SymbolInterpretationEvaled("B", emptyParams, "ActionB", emptyInstrParams),
				new SymbolInterpretationEvaled("C", emptyParams, "ActionC", emptyInstrParams)
			};

			doTest(caller, sym2Instr,
				"A",
				new string[] { "ActionA" });

			doTest(caller, sym2Instr,
				"A B A C",
				new string[] { "ActionA", "ActionB", "ActionA", "ActionC" });
		}

		public static void UnknownSymbolsTests(IInterpreterCaller caller) {

			var sym2Instr = new SymbolInterpretationEvaled[] {
				new SymbolInterpretationEvaled("A", emptyParams, "ActionA", emptyInstrParams)
			};

			doTest(caller, sym2Instr,
				"A x A x y",
				new string[] { "ActionA", "ActionA" });
		}

		public static void UnknownActionsTests(IInterpreterCaller caller) {
			var sym2Instr = new SymbolInterpretationEvaled[] {
			    new SymbolInterpretationEvaled("A", emptyParams, "ActionA", emptyInstrParams),
			    new SymbolInterpretationEvaled("C", emptyParams, "??", emptyInstrParams)
			};

			doTest(caller, sym2Instr,
				"A C A C C",
				new string[] { "ActionA", "ActionA" });
		}

		public static void ParametersFromSymbolTests(IInterpreterCaller caller) {

			var sym2Instr = new SymbolInterpretationEvaled[] {
				new SymbolInterpretationEvaled("A", emptyParams, "ActionA", emptyInstrParams)
			};

			doTest(caller, sym2Instr,
				"A A(8) A(8, 8)",
				new string[] { "ActionA", "ActionA(8)", "ActionA(8, 8)" });

			doTest(caller, sym2Instr,
				"A(5) A({1, 2}) A(1, 1, 1) A(1, {0, 0})",
				new string[] { "ActionA(5)", "ActionA({1, 2})", "ActionA(1, 1, 1)", "ActionA(1, {0, 0})" });
		}

		public static void ParametersTests(IInterpreterCaller caller) {

			var opParamsX = new ImmutableList<OptionalParameterEvaled>(
				new OptionalParameterEvaled("x")
			);
			var opParamsXY = new ImmutableList<OptionalParameterEvaled>(
				new OptionalParameterEvaled("x"),
				new OptionalParameterEvaled("y")
			);

			var sym2Instr = new SymbolInterpretationEvaled[] {
				new SymbolInterpretationEvaled("A", opParamsX, "ActionA", new ImmutableList<IExpression>(5.ToConst())),
				new SymbolInterpretationEvaled("B", opParamsX, "ActionB", new ImmutableList<IExpression>("x".ToVar())),
				new SymbolInterpretationEvaled("C", opParamsXY, "ActionC", new ImmutableList<IExpression>("x".ToVar(), 5.ToConst(), "y".ToVar())),
			};

			doTest(caller, sym2Instr,
				"A(8) A(8, 8)",
				new string[] { "ActionA(5)", "ActionA(5)" });

			doTest(caller, sym2Instr,
				"B(8) B(8, 8)",
				new string[] { "ActionB(8)", "ActionB(8)" });

			doTest(caller, sym2Instr,
				"C(8, 9) C(8, 9, 10)",
				new string[] { "ActionC(8, 5, 9)", "ActionC(8, 5, 9)" });
		}

		public static void NativelyOptionalParametersTests(IInterpreterCaller caller) {

			var sym2Instr = new SymbolInterpretationEvaled[] {
				new SymbolInterpretationEvaled("A", emptyParams, "ActionA", new ImmutableList<IExpression>(5.ToConst())),
				new SymbolInterpretationEvaled("B", emptyParams, "ActionB", new ImmutableList<IExpression>(5.ToConst(), 4.ToConst())),
			};

			doTest(caller, sym2Instr,
				"A A(8) A(8, 8)",
				new string[] { "ActionA(5)", "ActionA(8)", "ActionA(8, 8)" });

			doTest(caller, sym2Instr,
				"B B(8) B(8, 8) B(8, 8, 8)",
				new string[] { "ActionB(5, 4)", "ActionB(8, 4)", "ActionB(8, 8)", "ActionB(8, 8, 8)" });
		}

		public static void OptionalParametersTests(IInterpreterCaller caller) {

			var opParamsX = new ImmutableList<OptionalParameterEvaled>(
				new OptionalParameterEvaled("x", 1.ToConst())
			);
			var opParamsXY = new ImmutableList<OptionalParameterEvaled>(
				new OptionalParameterEvaled("x", 1.ToConst()),
				new OptionalParameterEvaled("y", 2.ToConst())
			);

			var sym2Instr = new SymbolInterpretationEvaled[] {
				new SymbolInterpretationEvaled("A", opParamsX, "ActionA", new ImmutableList<IExpression>("x".ToVar())),
				new SymbolInterpretationEvaled("B", opParamsXY, "ActionB", new ImmutableList<IExpression>("x".ToVar(), "y".ToVar())),
			};

			doTest(caller, sym2Instr,
				"A A(8) A(8, 8)",
				new string[] { "ActionA(1)", "ActionA(8)", "ActionA(8)" });

			doTest(caller, sym2Instr,
				"B B(8) B(8, 8) B(8, 8, 8)",
				new string[] { "ActionB(1, 2)", "ActionB(8, 2)", "ActionB(8, 8)", "ActionB(8, 8)" });
		}


		private static void doTest(IInterpreterCaller caller, SymbolInterpretationEvaled[] symbolToInstr,
				string inputSymbols, string[] excpected) {

			var symbols = TestUtils.CompileSymbols(inputSymbols);

			var evaluator = new EvaluatorsContainer(TestUtils.ExpressionEvaluatorContext);
			var symToInstr = MapModule.Empty<string, SymbolInterpretationEvaled>();
			foreach (var item in symbolToInstr) {
				symToInstr = symToInstr.Add(item.Symbol, item);
			}
			var lsystem = new LsystemEvaled("", TestUtils.ExpressionEvaluatorContext, null, null, symToInstr, null, null);
			var logger = new MessageLogger();
			var context = new ProcessContext(lsystem, new InMemoryOutputProvider(), null, TestUtils.ExpressionEvaluatorContext, logger);

			var dummy = new DummyInterpreter();
			caller.Interpreter = dummy;
			caller.Initialize(context);

			var symbolEvaluator = evaluator.Resolve<ISymbolEvaluator>();

			foreach (var sym in symbols) {
				caller.ProcessSymbol(symbolEvaluator.Evaluate(sym, TestUtils.ExpressionEvaluatorContext));
			}

			var actual = dummy.Actions.ToArray();

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
			IndentStringWriter writer;
			CanonicPrinter printer;

			public DummyInterpreter() {
				writer = new IndentStringWriter();
				printer = new CanonicPrinter(writer);
			}


			#region IInterpreter Members

			public IRenderer Renderer {
				set { throw new NotImplementedException(); }
			}

			public bool IsRendererCompatible(IRenderer renderer) {
				throw new NotImplementedException();
			}

			public bool RequiresMeasure { get { return false; } }

			public void Initialize(ProcessContext context) {
				throw new NotImplementedException();
			}

			public void Cleanup() {
				throw new NotImplementedException();
			}

			public void BeginProcessing(bool measuring) {
			}

			public void EndProcessing() {
			}

			#endregion


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

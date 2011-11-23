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
using Malsys.SemanticModel.Evaluated;
using Malsys.SourceCode.Printers;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Text.Lexing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Malsys.Tests.Interpreters {
	public static class GenericInterpreterCallerTests {

		public static void EmptyInputTests(IInterpreterCaller caller) {

			var sym2Instr = new Dictionary<string, Symbol<IValue>>();

			doTest(caller, sym2Instr, "", new string[0]);
		}

		public static void ExistingActionsTests(IInterpreterCaller caller) {
			var sym2Instr = new Dictionary<string, Symbol<IValue>>(){
				{"A", new Symbol<IValue>("ActionA", ImmutableList<IValue>.Empty) },
				{"B", new Symbol<IValue>("ActionB", ImmutableList<IValue>.Empty) },
				{"C", new Symbol<IValue>("ActionC", ImmutableList<IValue>.Empty) }
			};

			doTest(caller, sym2Instr,
				"A",
				new string[] { "ActionA" });

			doTest(caller, sym2Instr,
				"A B A C",
				new string[] { "ActionA", "ActionB", "ActionA", "ActionC" });
		}

		public static void UnknownSymbolsTests(IInterpreterCaller caller) {
			var sym2Instr = new Dictionary<string, Symbol<IValue>>(){
				{"A", new Symbol<IValue>("ActionA", ImmutableList<IValue>.Empty) }
			};

			doTest(caller, sym2Instr,
				"A x A x y",
				new string[] { "ActionA", "ActionA" });
		}

		public static void UnknownActionsTests(IInterpreterCaller caller) {
			var sym2Instr = new Dictionary<string, Symbol<IValue>>(){
				{"A", new Symbol<IValue>("ActionA", ImmutableList<IValue>.Empty) },
				{"C", new Symbol<IValue>("??", ImmutableList<IValue>.Empty) }
			};

			doTest(caller, sym2Instr,
				"A C A C C",
				new string[] { "ActionA", "ActionA" });
		}

		public static void ParametersTests(IInterpreterCaller caller) {
			var sym2Instr = new Dictionary<string, Symbol<IValue>>(){
				{"A", new Symbol<IValue>("ActionA", ImmutableList<IValue>.Empty) },
				{"B", new Symbol<IValue>("ActionB", ImmutableList<IValue>.Empty) },
				{"C", new Symbol<IValue>("ActionC", ImmutableList<IValue>.Empty) }
			};

			doTest(caller, sym2Instr,
				"A(0)",
				new string[] { "ActionA(0)" });

			doTest(caller, sym2Instr,
				"A(5) B({1, 2}) A(1, 1, 1) C(1, {0, 0})",
				new string[] { "ActionA(5)", "ActionB({1, 2})", "ActionA(1, 1, 1)", "ActionC(1, {0, 0})" });
		}

		public static void OptionalParametersTests(IInterpreterCaller caller) {
			var sym2Instr = new Dictionary<string, Symbol<IValue>>(){
				{"A", new Symbol<IValue>("ActionA", new ImmutableList<IValue>(0.ToConst())) },
				{"B", new Symbol<IValue>("ActionB", new ImmutableList<IValue>(5.ToConst(), 6.ToConst(), 7.ToConst())) }
			};

			doTest(caller, sym2Instr,
				"A A(1) A(1, 2)",
				new string[] { "ActionA(0)", "ActionA(1)", "ActionA(1, 2)" });

			doTest(caller, sym2Instr,
				"B B(1) B(1, 2) B(1, 2, 3) B(1, 2, 3, 4)",
				new string[] { "ActionB(5, 6, 7)", "ActionB(1, 6, 7)",
					"ActionB(1, 2, 7)", "ActionB(1, 2, 3)", "ActionB(1, 2, 3, 4)" });
		}


		private static void doTest(IInterpreterCaller caller, Dictionary<string, Symbol<IValue>> symbolToInstr,
				string inputSymbols, string[] excpected) {

			var lexBuff = LexBuffer<char>.FromString(inputSymbols);
			var msgs = new MessagesCollection();
			var symbolsAst = ParserUtils.ParseSymbols(lexBuff, msgs, "testInput");

			if (msgs.ErrorOcured) {
				Console.WriteLine("in: " + inputSymbols);
				Assert.Fail("Failed to parse symbols. " + msgs.ToString());
			}

			var compiler = new LsystemCompiler(msgs, new InputCompiler(msgs));
			var symbols = compiler.CompileSymbolsList(symbolsAst);

			if (msgs.ErrorOcured) {
				Console.WriteLine("in: " + inputSymbols);
				Assert.Fail("Failed to compile symbols. " + msgs.ToString());
			}

			var exprEvaluator = new ExpressionEvaluator();
			var symToInstr = MapModule.Empty<string, Symbol<IValue>>();
			foreach (var item in symbolToInstr) {
				symToInstr = symToInstr.Add(item.Key, item.Value);
			}
			var lsystem = new LsystemEvaled("", null, null, null, symToInstr, null, null);
			var context = new ProcessContext(lsystem, null, null, exprEvaluator, msgs);

			var dummy = new DummyInterpreter();
			caller.Initialize(context);
			caller.Interpreter = dummy;

			foreach (var sym in symbols) {
				caller.ProcessSymbol(exprEvaluator.EvaluateSymbol(sym, null, null));
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

			#endregion

			#region IComponent Members

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
				printer.PrintValueSeparated(args);
				writer.Write(")");
				string result = writer.GetResultAndClear();

				return result;
			}
		}
	}
}

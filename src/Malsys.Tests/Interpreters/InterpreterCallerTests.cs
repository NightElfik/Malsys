using System;
using System.Collections.Generic;
using System.Linq;
using Malsys.Compilers;
using Malsys.Expressions;
using Malsys.Interpreters;
using Malsys.IO;
using Malsys.Parsing;
using Malsys.SourceCode.Printers;
using Microsoft.FSharp.Text.Lexing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Malsys.Renderers;

namespace Malsys.Tests.Interpreters {
	[TestClass]
	public class InterpreterCallerTests {

		[TestMethod]
		public void EmptyInputTests() {
			var sym2Instr = new Dictionary<string, string>();

			doTest(sym2Instr, "", new string[0]);
		}

		[TestMethod]
		public void ExistingActionsTests() {
			var sym2Instr = new Dictionary<string, string>(){
				{"A", "ActionA"},
				{"B", "ActionB"},
				{"C", "ActionC"}
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
			var sym2Instr = new Dictionary<string, string>(){
				{"A", "ActionA"}
			};

			doTest(sym2Instr,
				"A x A x y",
				new string[] { "ActionA", "ActionA" });
		}

		[TestMethod]
		public void UnknownActionsTests() {
			var sym2Instr = new Dictionary<string, string>(){
				{"A", "ActionA"},
				{"C", "??"}
			};

			doTest(sym2Instr,
				"A C A C C",
				new string[] { "ActionA", "ActionA" });
		}

		[TestMethod]
		public void ParametersTests() {
			var sym2Instr = new Dictionary<string, string>(){
				{"A", "ActionA"},
				{"B", "ActionB"},
				{"C", "ActionC"}
			};

			doTest(sym2Instr,
				"A(0)",
				new string[] { "ActionA(0)" });

			doTest(sym2Instr,
				"A(5) B({1, 2}) A(1, 1, 1) C(1, {0, 0})",
				new string[] { "ActionA(5)", "ActionB({1, 2})", "ActionA(1, 1, 1)", "ActionC(1, {0, 0})" });
		}


		private void doTest(Dictionary<string, string> symbolToInstr, string inputSymbols, string[] excpected) {

			var lexBuff = LexBuffer<char>.FromString(inputSymbols);
			var msgs = new MessagesCollection();
			var symbolsAst = ParserUtils.ParseSymbols(lexBuff, msgs, "testInput");

			if (msgs.ErrorOcured) {
				Console.WriteLine("in: " + inputSymbols);
				Assert.Fail("Failed to parse symbols. " + msgs.ToString());
			}

			var compiler = new LsystemCompiler(new InputCompiler(msgs));
			var symbols = compiler.CompileListFailSafe(symbolsAst);

			var dummy = new DummyInterpreter();
			var caller = new InterpreterCaller() {
				Interpreter = dummy,
				SymbolsInterpretation = symbolToInstr
			};

			foreach (var sym in symbols) {
				caller.ProcessSymbol(sym.Evaluate());
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

			public void BeginInterpreting() {
			}

			public void EndInterpreting() {
			}

			#endregion


			[SymbolInterpretation]
			public void ActionA(ImmutableList<IValue> prms) {
				Actions.Add("ActionA" + printParams(prms));
			}

			[SymbolInterpretation]
			public void ActionB(ImmutableList<IValue> prms) {
				Actions.Add("ActionB" + printParams(prms));
			}

			[SymbolInterpretation]
			public void ActionC(ImmutableList<IValue> prms) {
				Actions.Add("ActionC" + printParams(prms));
			}

			private string printParams(ImmutableList<IValue> prms) {
				if (prms.IsEmpty) {
					return "";
				}

				writer.Write("(");
				printer.PrintValueSeparated(prms);
				writer.Write(")");
				string result = writer.GetResultAndClear();

				return result;
			}
		}

	}
}

using System.IO;
using System.Linq;
using Malsys.Evaluators;
using Malsys.IO;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;
using Malsys.SourceCode.Printers;
using InterpretActionParams = System.Tuple<System.Action<Malsys.Evaluators.ArgsStorage>, int>;

namespace Malsys.Processing.Components.Interpreters {
	/// <summary>
	/// Prints conversion from processed symbols to interpretation method name. If any interpreters are in configuration
	/// it also calls appropriate interpretation methods thus this component can replace InterpreterCaller without
	/// loosing functionality. This is handy if you are not sure how symbols are interpreted and what arguments were
	/// sent to interpretation method.
	/// </summary>
	/// <name>Interpreter calls debugger</name>
	/// <group>Interpreters</group>
	public class InterpreterCallerDebugger : InterpreterCaller {

		private IndentTextWriter itWriter;
		private CanonicPrinter debugPrinter;
		private IOutputProvider outProvider;


		public override void Initialize(ProcessContext ctxt) {
			outProvider = ctxt.OutputProvider;
			base.Initialize(ctxt);
		}

		override public void BeginProcessing(bool measuring) {

			itWriter = new IndentTextWriter(new StreamWriter(
				outProvider.GetOutputStream<InterpreterCaller>("interpret calls debugger", MimeType.Text.Plain)));
			debugPrinter = new CanonicPrinter(itWriter);

			base.BeginProcessing(measuring);
		}

		override public void EndProcessing() {

			itWriter.Dispose();

			base.EndProcessing();
		}



		override public void ProcessSymbol(Symbol<IValue> symbol) {

			debugPrinter.Print(symbol);
			debugPrinter.Write(" => ");

			SymbolInterpretationEvaled instr;
			if (!symbolToInstr.TryGetValue(symbol.Name, out instr)) {
				debugPrinter.NewLine();
				return;
			}

			if (instr.Parameters.IsEmpty) {
				args.AddArgs(symbol.Arguments, exprEvalCtxt.EvaluateList(instr.InstructionParameters));
			}
			else {
				var eec = exprEvalCtxt;  // create a working copy of constants
				try {
					mapSybolArgs(symbol, instr, ref eec);
				}
				catch (EvalException ex) {
					debugPrinter.WriteLine("ERROR ({0})".Fmt(ex.GetFullMessage()));
					return;
				}
				args.AddArgs(eec.EvaluateList(instr.InstructionParameters));
			}

			debugPrinter.Write(instr.InstructionName);
			if (args.ArgsCount > 0) {
				debugPrinter.Write("(");
				debugPrinter.Print(args);
				debugPrinter.Write(")");
			}

			if (interpreters.Count == 0) {
				debugPrinter.NewLine();
				return;
			}

			InterpretActionParams iActionParams;
			if (!instrToDel.TryGetValue(instr.InstructionName, out iActionParams)) {
				debugPrinter.WriteLine("ERROR (undefined)");
				return;
			}

			if (args.ArgsCount < iActionParams.Item2) {
				debugPrinter.WriteLine("ERROR (not enough arguments)");
				return;
			}

			debugPrinter.NewLine();

			iActionParams.Item1(args);
		}

	}
}

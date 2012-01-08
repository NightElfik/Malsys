using System.IO;
using Malsys.Evaluators;
using Malsys.IO;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;
using Malsys.SourceCode.Printers;
using InterpretActionParams = System.Tuple<System.Action<Malsys.Evaluators.ArgsStorage>, int>;

namespace Malsys.Processing.Components.Interpreters {
	[Component("Interpreter caller debugger", "Debug")]
	public class InterpreterCallerDebugger : InterpreterCaller {

		private IndentTextWriter itWriter;
		private CanonicPrinter debugPrinter;
		private IOutputProvider outProvider;

		[UserConnectable(IsOptional = true)]
		override public IInterpreter Interpreter {
			set {
				interpreter = value;
			}
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
				args.AddArgs(symbol.Arguments, exprEvaluator.EvaluateList(instr.InstructionParameters));
			}
			else {
				var consts = globalConsts;  // create a working copy of constants
				try {
					mapSybolArgs(symbol, instr, ref consts);
				}
				catch (EvalException ex) {
					debugPrinter.WriteLine("ERROR ({0})".Fmt(ex.Message));
					return;
				}
				args.AddArgs(exprEvaluator.EvaluateList(instr.InstructionParameters, consts, globalFuns));
			}

			debugPrinter.Write(instr.InstructionName);
			if (args.ArgsCount > 0) {
				debugPrinter.Write("(");
				debugPrinter.Print(args);
				debugPrinter.Write(")");
			}

			if (interpreter == null) {
				debugPrinter.NewLine();
				return;
			}

			InterpretActionParams iActionParams;
			if (!instrToDel.TryGetValue(instr.InstructionName.ToLowerInvariant(), out iActionParams)) {
				debugPrinter.WriteLine("ERROR (undefined)");
				return;
			}

			if (args.ArgsCount < iActionParams.Item2) {
				debugPrinter.WriteLine("ERROR (not enough arguments)");
				return;
			}

			debugPrinter.NewLine();

			iActionParams.Item1.Invoke(args);
		}

		override public void Initialize(ProcessContext ctxt) {

			outProvider = ctxt.OutputProvider;

			base.Initialize(ctxt);
		}


		override public void BeginProcessing(bool measuring) {

			itWriter = new IndentTextWriter(new StreamWriter(
				outProvider.GetOutputStream<InterpreterCaller>("interpret calls debugger", MimeType.Text_Plain)));
			debugPrinter = new CanonicPrinter(itWriter);

			if (interpreter != null) {
				base.BeginProcessing(measuring);
			}
		}

		override public void EndProcessing() {

			itWriter.Dispose();

			if (interpreter != null) {
				base.EndProcessing();
			}
		}
	}
}

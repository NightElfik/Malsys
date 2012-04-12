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
	/// Prints conversion from processed symbols to interpretation method name (ignores connected interpreter).
	/// This is handy if you are not sure how symbols are interpreted
	/// and what arguments were send to interpretation method.
	/// </summary>
	/// <name>Interpreter calls debugger</name>
	/// <group>Interpreters</group>
	public class InterpreterCallerDebugger : InterpreterCaller {

		private IndentTextWriter itWriter;
		private CanonicPrinter debugPrinter;
		private IOutputProvider outProvider;

		/// <summary>
		/// Connectable interpreter property to allow replacing any interpreter in any configuration with this debugger.
		/// This component is ignored.
		/// </summary>
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
				args.AddArgs(symbol.Arguments, exprEvalCtxt.EvaluateList(instr.InstructionParameters));
			}
			else {
				var eec = exprEvalCtxt;  // create a working copy of constants
				try {
					mapSybolArgs(symbol, instr, ref eec);
				}
				catch (EvalException ex) {
					debugPrinter.WriteLine("ERROR ({0})".Fmt(ex.Message));
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
			exprEvalCtxt = ctxt.ExpressionEvaluatorContext;
			symbolToInstr = ctxt.Lsystem.SymbolsInterpretation.ToDictionary(x => x.Key, x => x.Value);
		}


		override public void BeginProcessing(bool measuring) {

			itWriter = new IndentTextWriter(new StreamWriter(
				outProvider.GetOutputStream<InterpreterCaller>("interpret calls debugger", MimeType.Text.Plain)));
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

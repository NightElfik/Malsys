using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Malsys.Evaluators;
using Malsys.Processing.Components.Common;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;
using InterpretActionParams = System.Tuple<System.Action<Malsys.Evaluators.ArgsStorage>, int>;
using Malsys.IO;
using Malsys.SourceCode.Printers;
using System.IO;


namespace Malsys.Processing.Components.Interpreters {
	/// <summary>
	/// Process symbols by calling interpretation methods on connected interpreter.
	/// For conversion are used defined interpretation rules in current L-system.
	/// </summary>
	/// <name>Interpreter caller</name>
	/// <group>Interpreters</group>
	public class InterpreterCaller : IInterpreterCaller {

		private List<IInterpreter> interpreters = new List<IInterpreter>();

		private IExpressionEvaluatorContext exprEvalCtxt;

		private Dictionary<string, SymbolInterpretationEvaled> symbolToInstr;

		private Dictionary<string, InterpretActionParams> instrToDel = new Dictionary<string, InterpretActionParams>();

		private ArgsStorage args = new ArgsStorage();

		private bool debug;
		private IndentTextWriter itWriter;
		private CanonicPrinter debugPrinter;


		public IMessageLogger Logger { get; set; }

		/// <summary>
		/// True if print debug information about interpretation converting.
		/// </summary>
		/// <expected>true or false</expected>
		/// <default>false</default>
		[AccessName("debugInterpretation")]
		[UserSettable]
		public Constant DebugInterpretation { get; set; }


		/// <summary>
		/// Specialized component to allow interpret L-system symbol as another L-system.
		/// </summary>
		[UserConnectable(IsOptional = true)]
		public ILsystemInLsystemProcessor LsystemInLsystemProcessor { get; set; }


		public bool RequiresMeasure { get { return false; } }


		public void Initialize(ProcessContext ctxt) {

			debug = DebugInterpretation.IsTrue;
			if (debug) {
				itWriter = new IndentTextWriter(new StreamWriter(
					ctxt.OutputProvider.GetOutputStream<InterpreterCaller>("interpreter calls", MimeType.Text.Plain)));
				debugPrinter = new CanonicPrinter(itWriter);
			}

			exprEvalCtxt = ctxt.ExpressionEvaluatorContext;
			symbolToInstr = ctxt.Lsystem.SymbolsInterpretation.ToDictionary(x => x.Key, x => x.Value);

			var interpretersMeta = ctxt.ComponentGraph
				.Where(x => typeof(IInterpreter).IsAssignableFrom(x.Value.ComponentType))
				.Select(x => x.Value)
				.ToList();

			createInstrToDelCahce(interpretersMeta);

			interpreters = interpretersMeta.Select(x => (IInterpreter)x.Component).ToList();


		}

		public void Cleanup() {
			if (debug) {
				if (itWriter != null) {
					itWriter.Dispose();
					itWriter = null;
				}
				debugPrinter = null;
			}

			interpreters = null;
			DebugInterpretation = Constant.False;
			instrToDel.Clear();
		}


		public void BeginProcessing(bool measuring) {

			if (debug) {
				debugPrinter.WriteLine("Begin processing" + (measuring ? " (measuring)" : ""));
			}
			else {
				foreach (var i in interpreters) {
					i.BeginProcessing(measuring);
				}
			}
		}

		public void EndProcessing() {

			if (debug) {
				debugPrinter.WriteLine("End processing");
			}
			else {
				foreach (var i in interpreters) {
					i.EndProcessing();
				}
			}
		}


		public void ProcessSymbol(Symbol<IValue> symbol) {

			if (debug) {
				debugPrinter.Print(symbol);
				debugPrinter.Write(" => ");
			}

			SymbolInterpretationEvaled instr;
			if (!symbolToInstr.TryGetValue(symbol.Name, out instr)) {
				if (debug) {
					debugPrinter.NewLine();
				}
				return;  // symbol with missing interpret method is ignored
			}

			if (instr.Parameters.IsEmpty) {
				// use instruction argument as default values if no explicit arguments are defined
				args.AddArgs(symbol.Arguments, exprEvalCtxt.EvaluateList(instr.InstructionParameters));
			}
			else {
				var eec = exprEvalCtxt;  // create a working copy of constants
				mapSybolArgs(symbol, instr, ref eec);
				args.AddArgs(eec.EvaluateList(instr.InstructionParameters));
			}


			if (debug) {
				debugPrinter.Write(instr.InstructionName);
				if (args.ArgsCount > 0) {
					debugPrinter.Write("(");
					debugPrinter.Print(args);
					debugPrinter.Write(")");
				}
			}

			if (instr.InstructionIsLsystemName) {
				if (LsystemInLsystemProcessor == null) {
					throw new InterpretationException("Failed to interpret symbol `{0}` as L-system `{1}`. Component of type `{2}` is not connected."
						.Fmt(symbol.Name, instr.InstructionName, typeof(ILsystemInLsystemProcessor).FullName));
				}
				if (debug) {
					debugPrinter.NewLine();
				}
				LsystemInLsystemProcessor.ProcessLsystem(instr.InstructionName, instr.LsystemConfigName, args.ToArray());
				return;
			}


			InterpretActionParams iActionParams;
			if (!instrToDel.TryGetValue(instr.InstructionName, out iActionParams)) {
				if (debug) {
					debugPrinter.WriteLine(" ERROR (undefined)");
					return;
				}
				else {
					throw new InterpretationException("Unknown interpreter action `{0}` of symbol `{1}`."
						.Fmt(instr.InstructionName, symbol.Name));
				}
			}

			if (args.ArgsCount < iActionParams.Item2) {
				if (debug) {
					debugPrinter.WriteLine(" ERROR (not enough arguments)");
					return;
				}
				else {
					throw new InterpretationException("Not enough arguments supplied for interpretation of "
						+ "symbol `{0}` which needs {1} arguments for invoking `{2}` but only {3} were given."
						.Fmt(symbol.Name, iActionParams.Item2, instr.InstructionName, args.ArgsCount));
				}
			}

			if (debug) {
				debugPrinter.NewLine();
			}
			else {
				iActionParams.Item1(args);
			}
		}



		private void mapSybolArgs(Symbol<IValue> symbol, SymbolInterpretationEvaled instr, ref IExpressionEvaluatorContext eec) {

			var prms = instr.Parameters;

			for (int i = 0; i < prms.Length; i++) {
				IValue val;
				if (i < symbol.Arguments.Length) {
					val = symbol.Arguments[i];
				}
				else {
					if (prms[i].IsOptional) {
						val = prms[i].DefaultValue;
					}
					else {
						throw new EvalException("Not enough custom arguments supplied by symbol `{0}` to interpretation of `{1}`."
							.Fmt(symbol.Name, instr.InstructionName));
					}
				}

				eec = eec.AddVariable(prms[i].Name, val);
			}
		}

		private void createInstrToDelCahce(IEnumerable<ConfigurationComponent> interpretersMeta) {

			foreach (var interpretMeta in interpretersMeta) {
				var interpret = (IInterpreter)interpretMeta.Component;

				foreach (var intMethod in interpretMeta.Metadata.InterpretationMethods) {

					var del = createInterpretAction(intMethod.MethodInfo, interpret);
					var intAction = new InterpretActionParams(del, intMethod.MandatoryParamsCount);
					foreach (var name in intMethod.Names) {
						instrToDel.Add(name, intAction);
					}
				}
			}
		}

		private Action<ArgsStorage> createInterpretAction(MethodInfo mi, IInterpreter interpreter) {
			var instance = Expression.Constant(interpreter, interpreter.GetType());
			var argument = Expression.Parameter(typeof(ArgsStorage), "arguments");
			var call = Expression.Call(instance, mi, argument);
			return Expression.Lambda<Action<ArgsStorage>>(call, argument).Compile();
		}

	}
}

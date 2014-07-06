// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Malsys.Evaluators;
using Malsys.IO;
using Malsys.Processing.Components.Common;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;
using Malsys.SourceCode.Printers;
using InterpretActionParams = System.Tuple<System.Action<Malsys.Evaluators.ArgsStorage>, int>;


namespace Malsys.Processing.Components.Interpreters {
	/// <summary>
	/// Process symbols by calling interpretation methods on connected interpreter.
	/// For conversion are used defined interpretation rules in current L-system.
	/// </summary>
	/// <name>Interpreter caller</name>
	/// <group>Interpreters</group>
	/// TODO: improve dictionaries to be able to handle more interpreters with same method names.
	public class InterpreterCaller : IInterpreterCaller {

		private List<IInterpreter> interpreters = new List<IInterpreter>();

		private IExpressionEvaluatorContext exprEvalCtxt;

		private Dictionary<string, SymbolInterpretationEvaled> symbolToInstr;

		private Dictionary<string, InterpretActionParams> instrToDel = new Dictionary<string, InterpretActionParams>();

		private ArgsStorage args = new ArgsStorage();

		private bool debug;
		private bool interpDebug;
		private IndentTextWriter itWriter;
		private CanonicPrinter debugPrinter;


		/// <summary>
		/// True if print debug information about interpretation converting.
		/// </summary>
		/// <expected>true or false</expected>
		/// <default>false</default>
		[AccessName("debugInterpretation")]
		[UserSettable]
		public Constant DebugInterpretation { get; set; }

		/// <summary>
		/// If true, interpreter will interpret symbols while debugging.
		/// </summary>
		/// <expected>true or false</expected>
		/// <default>false</default>
		[AccessName("interpretWhileDebug")]
		[UserSettable]
		public Constant InterpretWhileDebug { get; set; }


		/// <summary>
		/// Specialized component to allow interpret L-system symbol as another L-system.
		/// </summary>
		[UserConnectable(IsOptional = true)]
		public ILsystemInLsystemProcessor LsystemInLsystemProcessor { get; set; }


		public IMessageLogger Logger { get; set; }


		[UserConnectable(IsOptional = true, AllowMultiple = true)]
		public IInterpreter ExplicitInterpreters {
			set {
				if (!explicitInterpreters.Contains(value)) {
					explicitInterpreters.Add(value);
				}
			}
		}
		private List<IInterpreter> explicitInterpreters = new List<IInterpreter>();


		public void Reset() {
			exprEvalCtxt = null;
			interpreters = null;
			DebugInterpretation = Constant.False;
			InterpretWhileDebug = Constant.False;
			explicitInterpreters.Clear();
			instrToDel.Clear();
			symbolToInstr = null;
		}

		public void Initialize(ProcessContext ctxt) {

			debug = DebugInterpretation.IsTrue;
			interpDebug = InterpretWhileDebug.IsTrue;
			if (debug) {
				itWriter = new IndentTextWriter(new StreamWriter(
					ctxt.OutputProvider.GetOutputStream<InterpreterCaller>("interpreter calls", MimeType.Text.Plain)));
				debugPrinter = new CanonicPrinter(itWriter);
			}

			exprEvalCtxt = ctxt.ExpressionEvaluatorContext;
			symbolToInstr = ctxt.Lsystem.SymbolsInterpretation.ToDictionary(x => x.Key, x => x.Value);

			List<ConfigurationComponent> interpretersMeta;
			if (explicitInterpreters.Count > 0) {
				interpretersMeta = explicitInterpreters
					.Select(x => ctxt.FindComponent(x))
					.Where(x => x.HasValue)
					.Select(x => x.Value.Value)
					.ToList();
			}
			else {
				interpretersMeta = ctxt.ComponentGraph
					.Where(x => typeof(IInterpreter).IsAssignableFrom(x.Value.ComponentType))
					.Select(x => x.Value)
					.ToList();
			}

			createInstrToDelCahce(interpretersMeta);

			interpreters = interpretersMeta.Select(x => (IInterpreter)x.Component).ToList();

			if (LsystemInLsystemProcessor != null) {
				LsystemInLsystemProcessor.SetInterpreters(interpreters);
			}

		}

		public void Cleanup() {
			if (debug) {
				if (itWriter != null) {
					itWriter.Dispose();
					itWriter = null;
				}
				debugPrinter = null;
			}

			instrToDel.Clear();

			if (symbolToInstr != null) {
				symbolToInstr.Clear();
				symbolToInstr = null;
			}
		}

		public void Dispose() { }



		public bool RequiresMeasure { get { return false; } }


		public void BeginProcessing(bool measuring) {

			if (debug) {
				debugPrinter.WriteLine("Begin processing" + (measuring ? " (measuring)" : ""));
			}

			if (!debug || interpDebug) {
				foreach (var i in interpreters) {
					i.BeginProcessing(measuring);
				}
			}
		}

		public void EndProcessing() {

			if (debug) {
				debugPrinter.WriteLine("End processing");
			}

			if (!debug || interpDebug) {
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
				return;  // Symbol with missing interpret method is ignored.
			}

			if (instr.Parameters.Count == 0) {
				// Use instruction argument as default values if no explicit arguments are defined.
				args.AddArgs(symbol.Arguments, exprEvalCtxt.EvaluateList(instr.InstructionParameters));
			}
			else {
				var eec = exprEvalCtxt;  // Create a working copy of constants.
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
					Logger.LogMessage(Message.LilpNotConnected, symbol.Name, instr.InstructionName, typeof(ILsystemInLsystemProcessor).FullName);
				}
				else {
					if (debug) {
						debugPrinter.NewLine();
					}
					LsystemInLsystemProcessor.ProcessLsystem(instr.InstructionName, instr.LsystemConfigName, args.ToArray());
				}
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

			if (!debug || interpDebug) {
				iActionParams.Item1(args);
			}
		}



		private void mapSybolArgs(Symbol<IValue> symbol, SymbolInterpretationEvaled instr, ref IExpressionEvaluatorContext eec) {

			var prms = instr.Parameters;

			for (int i = 0; i < prms.Count; i++) {
				IValue val;
				if (i < symbol.Arguments.Count) {
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
			//var instance = Expression.Constant(interpreter, interpreter.GetType());
			//var argument = Expression.Parameter(typeof(ArgsStorage), "arguments");
			//var call = Expression.Call(instance, mi, argument);
			//return Expression.Lambda<Action<ArgsStorage>>(call, argument).Compile();
			// tremendous speedup:
			return arg => mi.Invoke(interpreter, new object[] { arg });
		}


		public enum Message {

			[Message(MessageType.Warning, "Failed to interpret Symbol `{0}` as L-system `{1}`, responsible component is not connected (type {2}).")]
			LilpNotConnected,

		}

	}
}

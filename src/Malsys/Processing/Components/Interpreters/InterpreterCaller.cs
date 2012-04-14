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


namespace Malsys.Processing.Components.Interpreters {
	/// <summary>
	/// Process symbols by calling interpretation methods on connected interpreter.
	/// For conversion are used defined interpretation rules in current L-system.
	/// </summary>
	/// <name>Interpreter caller</name>
	/// <group>Interpreters</group>
	public class InterpreterCaller : IInterpreterCaller {

		protected List<IInterpreter> interpreters = new List<IInterpreter>();

		protected IExpressionEvaluatorContext exprEvalCtxt;

		protected Dictionary<string, SymbolInterpretationEvaled> symbolToInstr;

		protected Dictionary<string, InterpretActionParams> instrToDel = new Dictionary<string, InterpretActionParams>();

		protected ArgsStorage args = new ArgsStorage();


		public IMessageLogger Logger { get; set; }


		/// <summary>
		/// Specialized component to allow interpret L-system symbol as another L-system.
		/// </summary>
		[UserConnectable(IsOptional = true)]
		public virtual ILsystemInLsystemProcessor LsystemInLsystemProcessor { get; set; }


		public bool RequiresMeasure { get { return false; } }


		public virtual void Initialize(ProcessContext ctxt) {

			exprEvalCtxt = ctxt.ExpressionEvaluatorContext;
			symbolToInstr = ctxt.Lsystem.SymbolsInterpretation.ToDictionary(x => x.Key, x => x.Value);

			var interpretersMeta = ctxt.ComponentGraph
				.Where(x => typeof(IInterpreter).IsAssignableFrom(x.Value.ComponentType))
				.Select(x => x.Value)
				.ToList();

			createInstrToDelCahce(interpretersMeta);

			interpreters = interpretersMeta.Select(x => (IInterpreter)x.Component).ToList();

		}

		public virtual void Cleanup() {
			interpreters = null;
			instrToDel.Clear();
		}


		public virtual void BeginProcessing(bool measuring) {
			foreach (var i in interpreters) {
				i.BeginProcessing(measuring);
			}
		}

		public virtual void EndProcessing() {
			foreach (var i in interpreters) {
				i.EndProcessing();
			}
		}


		public virtual void ProcessSymbol(Symbol<IValue> symbol) {

			SymbolInterpretationEvaled instr;
			if (!symbolToInstr.TryGetValue(symbol.Name, out instr)) {
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

			if (instr.InstructionIsLsystemName) {
				if (LsystemInLsystemProcessor == null) {
					throw new InterpretationException("Failed to interpret symbol `{0}` as L-system `{1}`. Component of type `{2}` is not connected."
						.Fmt(symbol.Name, instr.InstructionName, typeof(ILsystemInLsystemProcessor).FullName));
				}
				LsystemInLsystemProcessor.ProcessLsystem(instr.InstructionName, instr.LsystemConfigName, args.ToArray());
				return;
			}


			InterpretActionParams iActionParams;
			if (!instrToDel.TryGetValue(instr.InstructionName, out iActionParams)) {
				throw new InterpretationException("Unknown interpreter action `{0}` of symbol `{1}`."
					.Fmt(instr.InstructionName, symbol.Name));
			}

			if (args.ArgsCount < iActionParams.Item2) {
				throw new InterpretationException("Not enough arguments supplied for interpretation of "
					+ "symbol `{0}` which needs {1} arguments for invoking `{2}` but only {3} were given."
					.Fmt(symbol.Name, iActionParams.Item2, instr.InstructionName, args.ArgsCount));
			}

			iActionParams.Item1(args);
		}



		protected void mapSybolArgs(Symbol<IValue> symbol, SymbolInterpretationEvaled instr, ref IExpressionEvaluatorContext eec) {

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

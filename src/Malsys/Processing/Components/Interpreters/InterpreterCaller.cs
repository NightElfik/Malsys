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

		protected IInterpreter interpreter;
		protected ILsystemInLsystemProcessor lsystemInLsystemProcessor;
		protected IExpressionEvaluatorContext exprEvalCtxt;

		protected Dictionary<string, SymbolInterpretationEvaled> symbolToInstr;

		protected Dictionary<string, InterpretActionParams> instrToDel;

		protected ArgsStorage args = new ArgsStorage();



		#region IInterpreterCaller Members

		/// <summary>
		/// Interpreter on which will be interpretation methods called.
		/// </summary>
		[UserConnectable]
		public virtual IInterpreter Interpreter {
			set {
				interpreter = value;
			}
		}

		/// <summary>
		/// Specialized component to allow interpret L-system symbol as another L-system.
		/// </summary>
		[UserConnectable(IsOptional=true)]
		public virtual ILsystemInLsystemProcessor LsystemInLsystemProcessor {
			set {
				lsystemInLsystemProcessor = value;
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
				if (lsystemInLsystemProcessor == null) {
					throw new InterpretationException("Failed to interpret symbol `{0}` as L-system `{1}`. Component of type `{2}` is not connected."
						.Fmt(symbol.Name, instr.InstructionName, typeof(ILsystemInLsystemProcessor).FullName));
				}
				lsystemInLsystemProcessor.ProcessLsystem(instr.InstructionName, instr.LsystemConfigName, args.ToArray());
				return;
			}


			InterpretActionParams iActionParams;
			if (!instrToDel.TryGetValue(instr.InstructionName, out iActionParams)) {
				throw new InterpretationException("Unknown interpreter action `{0}` of symbol `{1}` of interpreter `{2}`."
					.Fmt(instr.InstructionName, symbol.Name, interpreter.GetType().FullName));
			}

			if (args.ArgsCount < iActionParams.Item2) {
				throw new InterpretationException("Not enough arguments supplied for interpretation of "
					+ "symbol `{0}` which needs {1} arguments for invoking `{2}` but only {3} were given."
					.Fmt(symbol.Name, iActionParams.Item2, instr.InstructionName, args.ArgsCount));
			}

			iActionParams.Item1(args);
		}

		public bool RequiresMeasure { get { return false; } }

		public virtual void Initialize(ProcessContext ctxt) {

			exprEvalCtxt = ctxt.ExpressionEvaluatorContext;
			symbolToInstr = ctxt.Lsystem.SymbolsInterpretation.ToDictionary(x => x.Key, x => x.Value);

			var component = ctxt.FindComponent(interpreter);
			if (component == null) {
				throw new ComponentException("Interpreter metadata not found.");
			}

			createInstrToDelCahce(component.Value.Value);
		}

		public virtual void Cleanup() { }

		public virtual void BeginProcessing(bool measuring) {
			interpreter.BeginProcessing(measuring);
		}

		public virtual void EndProcessing() {
			interpreter.EndProcessing();
		}

		#endregion


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

		private void createInstrToDelCahce(ConfigurationComponent component) {

			instrToDel = new Dictionary<string, InterpretActionParams>();

			foreach (var intMethod in component.Metadata.InterpretationMethods) {

				var del = createInterpretAction(intMethod.MethodInfo);
				var intAction = new InterpretActionParams(del, intMethod.MandatoryParamsCount);
				foreach (var name in intMethod.Names) {
					instrToDel.Add(name, intAction);
				}
			}
		}

		private Action<ArgsStorage> createInterpretAction(MethodInfo mi) {
			var instance = Expression.Constant(interpreter, interpreter.GetType());
			var argument = Expression.Parameter(typeof(ArgsStorage), "arguments");
			var call = Expression.Call(instance, mi, argument);
			return Expression.Lambda<Action<ArgsStorage>>(call, argument).Compile();
		}

	}
}

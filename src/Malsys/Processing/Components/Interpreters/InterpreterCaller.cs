using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Malsys.Evaluators;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;
using Microsoft.FSharp.Core;
using InterpretAction = System.Action<Malsys.Evaluators.ArgsStorage>;
using SymIntMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Symbol<Malsys.SemanticModel.Evaluated.IValue>>;


namespace Malsys.Processing.Components.Interpreters {
	public class InterpreterCaller : IInterpreterCaller {

		private IInterpreter interpreter;

		private SymIntMap symbolToInstr;

		private Dictionary<string, InterpretAction> instrToDel;

		private ArgsStorage args = new ArgsStorage();



		#region IInterpreterCaller Members

		[UserConnectable]
		public IInterpreter Interpreter {
			set {
				interpreter = value;
				createInstrToDelCahce();
			}
		}

		public void ProcessSymbol(Symbol<IValue> symbol) {

			var maybeInstr = symbolToInstr.TryFind(symbol.Name);
			if (OptionModule.IsSome(maybeInstr)) {
				var instr = maybeInstr.Value;

				InterpretAction iAction;
				if (instrToDel.TryGetValue(instr.Name.ToLowerInvariant(), out iAction)) {
					args.AddArgs(symbol.Arguments, instr.Arguments);
					iAction.Invoke(args);
				}
				else {
					// TODO: handle symbol with unknown action
				}
			}
			else {
				// TODO: resolve missing interpret method for symbol
			}
		}

		public bool RequiresMeasure { get { return false; } }

		public void Initialize(ProcessContext ctxt) {

			symbolToInstr = ctxt.Lsystem.SymbolsInterpretation;

			createInstrToDelCahce();
		}

		public void Cleanup() { }

		public void BeginProcessing(bool measuring) {
			interpreter.BeginProcessing(measuring);
		}

		public void EndProcessing() {
			interpreter.EndProcessing();
		}

		#endregion


		private void createInstrToDelCahce() {

			instrToDel = new Dictionary<string, InterpretAction>();

			foreach (var methodInfo in interpreter.GetType().GetMethods()) {
				var attrs = methodInfo.GetCustomAttributes(typeof(SymbolInterpretationAttribute), false);
				if (attrs.Length != 1) {
					continue;
				}

				var prms = methodInfo.GetParameters();
				if (prms.Length != 1 && !prms[0].ParameterType.Equals(typeof(ArgsStorage))) {
					continue;
				}

				var del = createInterpretAction(methodInfo);
				instrToDel.Add(methodInfo.Name.ToLowerInvariant(), del);
			}
		}

		private InterpretAction createInterpretAction(MethodInfo mi) {
			var instance = Expression.Constant(interpreter, interpreter.GetType());
			var argument = Expression.Parameter(typeof(ArgsStorage), "arguments");
			var call = Expression.Call(instance, mi, argument);
			return Expression.Lambda<InterpretAction>(call, argument).Compile();
		}

	}
}

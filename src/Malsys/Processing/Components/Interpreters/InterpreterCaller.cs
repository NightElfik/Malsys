using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reflection;
using Malsys.Evaluators;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;
using InterpretAction = System.Action<Malsys.Evaluators.ArgsStorage>;
using SymIntMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Symbol<Malsys.SemanticModel.Evaluated.IValue>>;


namespace Malsys.Processing.Components.Interpreters {
	public class InterpreterCaller : IInterpreterCaller {

		private IInterpreter interpret;

		private SymIntMap symbolToInstr;

		private Dictionary<string, InterpretAction> instrToDel;

		private ArgsStorage args = new ArgsStorage();


		public InterpreterCaller() {
			interpret = EmptyInterpret.Instance;
			symbolToInstr = MapModule.Empty<string, Symbol<IValue>>();

			createInstrToDelCahce();
		}

		[ContractInvariantMethod]
		private void objectInvariant() {

			Contract.Invariant(interpret != null);
			Contract.Invariant(symbolToInstr != null);
			Contract.Invariant(instrToDel != null);
			Contract.Invariant(args != null);
		}


		#region IInterpreterCaller Members

		public IInterpreter Interpreter {
			set {
				interpret = value;
				createInstrToDelCahce();
			}
		}

		#endregion

		#region ISymbolProcessor Members

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

		#endregion

		#region IComponent Members

		public ProcessContext Context {
			set { symbolToInstr = value.Lsystem.SymbolsInterpretation; }
		}

		public void BeginProcessing(bool measuring) {
			interpret.BeginProcessing(measuring);
		}

		public void EndProcessing() {
			interpret.EndProcessing();
		}

		#endregion


		private void createInstrToDelCahce() {

			instrToDel = new Dictionary<string, InterpretAction>();

			foreach (var methodInfo in interpret.GetType().GetMethods()) {
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
			var instance = Expression.Constant(interpret, interpret.GetType());
			var argument = Expression.Parameter(typeof(ArgsStorage), "args");
			var call = Expression.Call(instance, mi, argument);
			return Expression.Lambda<InterpretAction>(call, argument).Compile();
		}

	}
}

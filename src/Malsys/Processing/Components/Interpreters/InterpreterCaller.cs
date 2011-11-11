using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reflection;
using Malsys.Expressions;
using InterpretAction = System.Action<Malsys.ImmutableList<Malsys.Expressions.IValue>>;

namespace Malsys.Processing.Components.Interpreters {
	public class InterpreterCaller : IInterpreterCaller {

		private IInterpreter interpret;

		private Dictionary<string, string> symbolToInstr;

		private Dictionary<string, InterpretAction> symbolToIntActionCache;


		public InterpreterCaller() {
			interpret = EmptyInterpret.Instance;
			symbolToInstr = new Dictionary<string, string>();

			createIntActionsCahce();
		}

		[ContractInvariantMethod]
		private void objectInvariant() {

			Contract.Invariant(interpret != null);
			Contract.Invariant(symbolToInstr != null);

			Contract.Invariant(symbolToIntActionCache != null);
		}


		#region IInterpreterCaller Members

		public IInterpreter Interpreter {
			set {
				interpret = value;
				createIntActionsCahce();
			}
		}

		public ProcessContext Context {
			set {
				//symbolToInstr = value;
				//createIntActionsCahce();
			}
		}

		#endregion

		#region ISymbolProcessor Members

		public void BeginProcessing() {
			interpret.BeginInterpreting();
		}

		public void ProcessSymbol(Symbol<IValue> symbol) {

			InterpretAction iAction;
			if (symbolToIntActionCache.TryGetValue(symbol.Name, out iAction)) {
				iAction.Invoke(symbol.Arguments);
			}
			else {
				// TODO: handle symbol with unknown action
			}
		}

		public void EndProcessing() {
			interpret.EndInterpreting();
		}

		#endregion


		private void createIntActionsCahce() {

			var instrToDel = new Dictionary<string, InterpretAction>();

			foreach (var methodInfo in interpret.GetType().GetMethods()) {
				var attrs = methodInfo.GetCustomAttributes(typeof(SymbolInterpretationAttribute), false);
				if (attrs.Length != 1) {
					continue;
				}

				var prms = methodInfo.GetParameters();
				if (prms.Length != 1 && !prms[0].ParameterType.Equals(typeof(ImmutableList<IValue>))) {
					continue;
				}

				var del = createInterpretAction(methodInfo);
				instrToDel.Add(methodInfo.Name.ToLowerInvariant(), del);
			}

			var symbolToInterpterCall = new Dictionary<string, InterpretAction>();

			foreach (var sym2instr in symbolToInstr) {
				InterpretAction action;
				if (instrToDel.TryGetValue(sym2instr.Value.ToLowerInvariant(), out action)) {
					symbolToInterpterCall.Add(sym2instr.Key, action);
				}
				else {
					// TODO: resolve missing interpret method for symbol
				}

			}

			symbolToIntActionCache = symbolToInterpterCall;
		}

		private InterpretAction createInterpretAction(MethodInfo mi) {
			var instance = Expression.Constant(interpret, interpret.GetType());
			var argument = Expression.Parameter(typeof(ImmutableList<IValue>), "prms");
			var call = Expression.Call(instance, mi, argument);
			return Expression.Lambda<InterpretAction>(call, argument).Compile();
		}


	}
}

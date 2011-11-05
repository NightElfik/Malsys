using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Malsys.Expressions;
using Malsys.Rewriters;
using InterpretAction = System.Action<Malsys.ImmutableList<Malsys.Expressions.IValue>>;

namespace Malsys.Interpreters {
	public class InterpreterCaller : ISymbolProcessor {

		private IInterpreter interpret;
		private Dictionary<string, InterpretAction> symbolToIntActionCache;


		public InterpreterCaller(IInterpreter ipret, Dictionary<string, string> symbolToInstr) {
			interpret = ipret;
			symbolToIntActionCache = createIntActionsCahce(interpret, symbolToInstr);
		}


		#region ISymbolProcessor Members

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

		}

		#endregion



		private Dictionary<string, InterpretAction> createIntActionsCahce(
				IInterpreter interpret, Dictionary<string, string> symbolToInstr) {

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

			return symbolToInterpterCall;
		}

		private InterpretAction createInterpretAction(MethodInfo mi) {
			var instance = Expression.Constant(interpret, interpret.GetType());
			var argument = Expression.Parameter(typeof(ImmutableList<IValue>), "prms");
			var call = Expression.Call(instance, mi, argument);
			return Expression.Lambda<InterpretAction>(call, argument).Compile();
		}
	}
}

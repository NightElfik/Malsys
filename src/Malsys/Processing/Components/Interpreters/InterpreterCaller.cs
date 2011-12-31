using System.Collections.Generic;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Malsys.Evaluators;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;
using Microsoft.FSharp.Core;
using InterpretAction = System.Action<Malsys.Evaluators.ArgsStorage>;
using InterpretActionParams = System.Tuple<System.Action<Malsys.Evaluators.ArgsStorage>, int>;
using SymIntMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Symbol<Malsys.SemanticModel.Evaluated.IValue>>;


namespace Malsys.Processing.Components.Interpreters {
	[Component("Interpreter caller", ComponentGroupNames.Interpreters)]
	public class InterpreterCaller : IInterpreterCaller {

		private IInterpreter interpreter;
		private IMessageLogger logger;

		private Dictionary<string, Symbol<IValue>> symbolToInstr;

		private Dictionary<string, InterpretActionParams> instrToDel;

		private ArgsStorage args = new ArgsStorage();



		#region IInterpreterCaller Members

		[UserConnectable]
		public IInterpreter Interpreter {
			set {
				interpreter = value;
			}
		}

		public void ProcessSymbol(Symbol<IValue> symbol) {

			Symbol<IValue> instr;
			if (!symbolToInstr.TryGetValue(symbol.Name, out instr)) {
				// TODO: handle missing interpret method for symbol
				return;
			}

			InterpretActionParams iActionParams;
			if (!instrToDel.TryGetValue(instr.Name.ToLowerInvariant(), out iActionParams)) {
				throw new InterpretationException("Unknown interpreter action `{0}` of symbol `{1}` of interpreter `{2}`."
					.Fmt(instr.Name, symbol.Name, interpreter.GetType().FullName));
			}

			args.AddArgs(symbol.Arguments, instr.Arguments);
			if (args.ArgsCount < iActionParams.Item2) {
				throw new InterpretationException("Not enough arguments supplied. "
					+ "Symbol `{0}` needs {1} arguments for invoking `{2}` but only {3} were given."
					.Fmt(symbol.Name, iActionParams.Item2, instr.Name, args.ArgsCount));
			}

			iActionParams.Item1.Invoke(args);
		}

		public bool RequiresMeasure { get { return false; } }

		public void Initialize(ProcessContext ctxt) {

			logger = ctxt.Logger;
			symbolToInstr = ctxt.Lsystem.SymbolsInterpretation.ToDictionary(x => x.Key, x => x.Value);

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

			instrToDel = new Dictionary<string, InterpretActionParams>();

			foreach (var methodInfo in interpreter.GetType().GetMethods()) {

				var attrs = methodInfo.GetCustomAttributes(typeof(SymbolInterpretationAttribute), true);
				if (attrs.Length != 1) {
					continue;
				}

				var attr = (SymbolInterpretationAttribute)attrs[0];

				var prms = methodInfo.GetParameters();
				if (prms.Length != 1 || !prms[0].ParameterType.Equals(typeof(ArgsStorage))) {
					throw new ComponentInitializationException("Interpreter method marked by `{0}` have invalid parameters."
						.Fmt(typeof(SymbolInterpretationAttribute).Name));
				}

				var del = createInterpretAction(methodInfo);
				instrToDel.Add(methodInfo.Name.ToLowerInvariant(), new InterpretActionParams(del, attr.RequiredParametersCount));
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

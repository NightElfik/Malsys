using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Malsys.Evaluators;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;
using ConstsMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Evaluated.IValue>;
using FunsMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Compiled.FunctionEvaledParams>;
using InterpretAction = System.Action<Malsys.Evaluators.ArgsStorage>;
using InterpretActionParams = System.Tuple<System.Action<Malsys.Evaluators.ArgsStorage>, int>;


namespace Malsys.Processing.Components.Interpreters {
	[Component("Interpreter caller", ComponentGroupNames.Interpreters)]
	public class InterpreterCaller : IInterpreterCaller {

		protected IInterpreter interpreter;
		protected IExpressionEvaluator exprEvaluator;
		protected ConstsMap globalConsts;
		protected FunsMap globalFuns;

		protected Dictionary<string, SymbolInterpretationEvaled> symbolToInstr;

		protected Dictionary<string, InterpretActionParams> instrToDel;

		protected ArgsStorage args = new ArgsStorage();



		#region IInterpreterCaller Members

		[UserConnectable]
		public virtual IInterpreter Interpreter {
			set {
				interpreter = value;
			}
		}

		public virtual void ProcessSymbol(Symbol<IValue> symbol) {

			SymbolInterpretationEvaled instr;
			if (!symbolToInstr.TryGetValue(symbol.Name, out instr)) {
				return;  // symbol with missing interpret method is ignored
			}

			if (instr.Parameters.IsEmpty) {
				// use instruction argument as default values if no explicit arguments are defined
				args.AddArgs(symbol.Arguments, exprEvaluator.EvaluateList(instr.InstructionParameters, globalConsts, globalFuns));
			}
			else {
				var consts = globalConsts;  // create a working copy of constants
				mapSybolArgs(symbol, instr, ref consts);
				args.AddArgs(exprEvaluator.EvaluateList(instr.InstructionParameters, consts, globalFuns));
			}


			InterpretActionParams iActionParams;
			if (!instrToDel.TryGetValue(instr.InstructionName.ToLowerInvariant(), out iActionParams)) {
				throw new InterpretationException("Unknown interpreter action `{0}` of symbol `{1}` of interpreter `{2}`."
					.Fmt(instr.InstructionName, symbol.Name, interpreter.GetType().FullName));
			}

			if (args.ArgsCount < iActionParams.Item2) {
				throw new InterpretationException("Not enough arguments supplied for interpretation of "
					+ "symbol `{0}` which needs {1} arguments for invoking `{2}` but only {3} were given."
					.Fmt(symbol.Name, iActionParams.Item2, instr.InstructionName, args.ArgsCount));
			}

			iActionParams.Item1.Invoke(args);
		}

		public bool RequiresMeasure { get { return false; } }

		public virtual void Initialize(ProcessContext ctxt) {

			exprEvaluator = ctxt.Evaluator.ResolveExpressionEvaluator();
			globalConsts = ctxt.Lsystem.Constants;
			globalFuns = ctxt.Lsystem.Functions;

			symbolToInstr = ctxt.Lsystem.SymbolsInterpretation.ToDictionary(x => x.Key, x => x.Value);

			createInstrToDelCahce();
		}

		public virtual void Cleanup() { }

		public virtual void BeginProcessing(bool measuring) {

			interpreter.BeginProcessing(measuring);
		}

		public virtual void EndProcessing() {
			interpreter.EndProcessing();
		}

		#endregion


		protected void mapSybolArgs(Symbol<IValue> symbol, SymbolInterpretationEvaled instr, ref ConstsMap consts) {

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

				consts = consts.Add(prms[i].Name, val);
			}
		}

		private void createInstrToDelCahce() {

			instrToDel = new Dictionary<string, InterpretActionParams>();

			if (interpreter == null) {
				return;
			}

			foreach (var methodInfo in interpreter.GetType().GetMethods()) {

				var attrs = methodInfo.GetCustomAttributes(typeof(SymbolInterpretationAttribute), true);
				if (attrs.Length != 1) {
					continue;
				}

				var attr = (SymbolInterpretationAttribute)attrs[0];

				var prms = methodInfo.GetParameters();
				if (prms.Length != 1 || prms[0].ParameterType != typeof(ArgsStorage)) {
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

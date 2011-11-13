using System.Collections.Generic;
using Malsys.Evaluators;
using Malsys.Parsing;
using Malsys.SemanticModel.Compiled;
using Microsoft.FSharp.Text.Lexing;
using FunMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Evaluated.FunctionEvaledParams>;
using VarMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Evaluated.IValue>;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Compilers {
	public class InputCompiler {

		private MessagesCollection msgs;

		private LsystemCompiler lsysCompiler;
		private ExpressionCompiler exprCompiler;
		private BindingCompilerVisitor bindCompiler;
		private ExpressionEvaluator exprEvaluator;
		private InputCompilerVisitor inVisitor;

		public MessagesCollection Messages { get { return msgs; } }
		public LsystemCompiler LsystemCompiler { get { return lsysCompiler; } }
		public ExpressionCompiler ExpressionCompiler { get { return exprCompiler; } }


		public InputCompiler(MessagesCollection msgsColl) {
			msgs = msgsColl;

			exprCompiler = new ExpressionCompiler(msgs);
			lsysCompiler = new LsystemCompiler(this);
			bindCompiler = new BindingCompilerVisitor(this);
			exprEvaluator = new ExpressionEvaluator();
			inVisitor = new InputCompilerVisitor(this);
		}




		public CompilerResult<InputBlock> CompileFromString(string strInput, string sourceName) {

			var lexBuff = LexBuffer<char>.FromString(strInput);
			msgs.DefaultSourceName = sourceName;
			var comments = new List<Ast.Comment>();

			var parsedInput = ParserUtils.ParseLsystemStatements(comments, lexBuff, msgs, sourceName);

			return CompileFromAst(parsedInput);
		}

		public CompilerResult<InputBlock> CompileFromAst(Ast.InputBlock parsedInput) {

			var compiledInput = new List<IInputStatement>(parsedInput.Length);

			for (int i = 0; i < parsedInput.Length; i++) {
				var stat = inVisitor.TryCompile(parsedInput[i]);
				if (stat) {
					compiledInput.Add((IInputStatement)stat);
				}
			}

			var inEval = new InputEvaluator(new BindingsEvaluator(exprEvaluator));
			try {
				return inEval.Evaluate(compiledInput);
			}
			catch (EvalException ex) {
				msgs.AddError("Failed to evaluate input. " + ex.GetWholeMessage(), Position.Unknown);
				return CompilerResult<InputBlock>.Error;
			}
		}



		public ImmutableList<OptionalParameter> CompileParameters(ImmutableList<Ast.OptionalParameter> parameters) {

			int parametersCount = parameters.Count;
			bool wasOptional = false;
			var result = new OptionalParameter[parametersCount];

			for (int i = 0; i < parametersCount; i++) {

				result[i] = new OptionalParameter(parameters[i].NameId.Name, exprCompiler.CompileExpression(parameters[i].OptionalValue));

				if (result[i].IsOptional) {
					wasOptional = true;
				}
				else if (wasOptional) {
					msgs.AddError("Mandatory parameters have to be before all optional parameters, but mandatory parameter `{0}` is after optional."
							.Fmt(parameters[i].NameId.Name),
						parameters[i].Position);
				}
			}

			// check wether parameters names are unique
			foreach (var indices in result.GetEqualValuesIndices((l, r) => { return l.Name.Equals(r.Name); })) {
				msgs.AddError("{0}. and {1}. parameter have same name `{2}`.".Fmt(indices.Item1 + 1, indices.Item2 + 1, result[indices.Item1].Name),
					parameters[indices.Item1].Position, parameters[indices.Item2].Position);
			}

			return new ImmutableList<OptionalParameter>(result, true);
		}

		public CompilerResult<Binding> CompileBinding(Ast.Binding binding, BindingType allowTypes) {

			return bindCompiler.TryCompile(binding, allowTypes);
		}

		public ImmutableList<Binding> CompileBindingsList(ImmutableList<Ast.Binding> bindingList, BindingType allowTypes) {

			var compiledBinds = new List<Binding>(bindingList.Length);

			for (int i = 0; i < bindingList.Length; i++) {
				var bind = bindCompiler.TryCompile(bindingList[i], allowTypes);
				if (bind) {
					compiledBinds.Add(bind);
				}
			}

			return new ImmutableList<Binding>(compiledBinds);
		}

	}
}

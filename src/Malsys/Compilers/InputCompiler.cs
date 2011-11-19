using System.Collections.Generic;
using Malsys.Evaluators;
using Malsys.Parsing;
using Malsys.SemanticModel.Compiled;
using Microsoft.FSharp.Text.Lexing;

namespace Malsys.Compilers {
	public class InputCompiler {

		private MessagesCollection msgs;

		private LsystemCompiler lsysCompiler;
		private ExpressionCompiler exprCompiler;
		private FunctionCompilerVisitor funCompiler;
		private ExpressionEvaluator exprEvaluator;
		private InputCompilerVisitor inVisitor;

		public MessagesCollection Messages { get { return msgs; } }
		public LsystemCompiler LsystemCompiler { get { return lsysCompiler; } }
		public ExpressionCompiler ExpressionCompiler { get { return exprCompiler; } }


		public InputCompiler(MessagesCollection msgsColl) {
			msgs = msgsColl;

			exprCompiler = new ExpressionCompiler(msgs);
			lsysCompiler = new LsystemCompiler(this);
			funCompiler = new FunctionCompilerVisitor(this);
			exprEvaluator = new ExpressionEvaluator();
			inVisitor = new InputCompilerVisitor(this);
		}




		public ImmutableList<IInputStatement> CompileFromString(string strInput, string sourceName) {

			var lexBuff = LexBuffer<char>.FromString(strInput);
			msgs.DefaultSourceName = sourceName;
			var comments = new List<Ast.Comment>();

			var parsedInput = ParserUtils.ParseInput(comments, lexBuff, msgs, sourceName);

			return CompileFromAst(parsedInput);
		}

		public ImmutableList<IInputStatement> CompileFromAst(Ast.InputBlock parsedInput) {

			var statements = new List<IInputStatement>(parsedInput.Length);

			for (int i = 0; i < parsedInput.Length; i++) {
				var stat = inVisitor.TryCompile(parsedInput[i]);
				if (stat) {
					statements.Add(stat.Result);
				}
			}

			return new ImmutableList<IInputStatement>(statements);
		}



		public ImmutableList<OptionalParameter> CompileParameters(ImmutableList<Ast.OptionalParameter> parameters) {

			int parametersCount = parameters.Count;
			bool wasOptional = false;
			var result = new OptionalParameter[parametersCount];

			for (int i = 0; i < parametersCount; i++) {

				result[i] = new OptionalParameter(parameters[i].NameId.Name, exprCompiler.CompileExpression(parameters[i].DefaultValue));

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

		public ConstantDefinition CompileConstDef(Ast.ConstantDefinition constDefAst) {
			return new ConstantDefinition(constDefAst.NameId.Name, exprCompiler.CompileExpression(constDefAst.ValueExpr), constDefAst);
		}

		public Function CompileFunctionDef(Ast.FunctionDefinition funDefAst) {

			var compiledStats = new IFunctionStatement[funDefAst.Statements.Length];

			for (int i = 0; i < funDefAst.Statements.Length; i++) {
				compiledStats[i] = funCompiler.Compile(funDefAst.Statements[i]);
			}

			var prms = CompileParameters(funDefAst.Parameters);
			var stats = new ImmutableList<IFunctionStatement>(compiledStats, true);
			return new Function(funDefAst.NameId.Name, prms, stats, funDefAst);
		}

	}
}

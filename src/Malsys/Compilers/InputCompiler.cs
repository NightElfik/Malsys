using System.Collections.Generic;
using Malsys.Evaluators;
using Malsys.Parsing;
using Malsys.SemanticModel.Compiled;
using Microsoft.FSharp.Text.Lexing;

namespace Malsys.Compilers {
	public class InputCompiler : MessagesLogger<InputCompilerMessageType> {

		private LsystemCompiler lsysCompiler;
		private ExpressionCompiler exprCompiler;
		private FunctionCompilerVisitor funCompiler;
		private ExpressionEvaluator exprEvaluator;
		private InputCompilerVisitor inVisitor;

		public LsystemCompiler LsystemCompiler { get { return lsysCompiler; } }
		public ExpressionCompiler ExpressionCompiler { get { return exprCompiler; } }


		public InputCompiler(MessagesCollection msgs) : base(msgs) {

			inVisitor = new InputCompilerVisitor(this);

			lsysCompiler = new LsystemCompiler(msgs, this);
			exprCompiler = new ExpressionCompiler(msgs);

			funCompiler = new FunctionCompilerVisitor(this);
			exprEvaluator = new ExpressionEvaluator();
		}




		public ImmutableList<IInputStatement> CompileFromString(string strInput, string sourceName) {

			var lexBuff = LexBuffer<char>.FromString(strInput);
			messages.DefaultSourceName = sourceName;
			var comments = new List<Ast.Comment>();

			var parsedInput = ParserUtils.ParseInput(comments, lexBuff, messages, sourceName);

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
					logMessage(InputCompilerMessageType.RequiredParamAfterOptional, parameters[i].Position, parameters[i].NameId.Name);
				}
			}

			// check wether parameters names are unique
			foreach (var indices in result.GetEqualValuesIndices((l, r) => { return l.Name.Equals(r.Name); })) {
				logMessage(InputCompilerMessageType.ParamNameNotUnique, parameters[indices.Item1].Position, result[indices.Item1].Name, parameters[indices.Item2].Position);
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


		public override string GetMessageTypeId(InputCompilerMessageType msgType) {
			return ((int)msgType).ToString();
		}

		protected override string resolveMessage(InputCompilerMessageType msgType, out MessageType type, params object[] args) {

			type = MessageType.Error;

			switch (msgType) {
				case InputCompilerMessageType.ParamNameNotUnique:
					return "Parameter name `{0}` is not unique. See also {1}.".Fmt(args);
				case InputCompilerMessageType.RequiredParamAfterOptional:
					return "Optional parameters must appear after all required parameters, but required parameter `{0}` is after optional.".Fmt(args);
				default:
					return "Unknown error.";
			}
		}


		class InputCompilerVisitor : Ast.IInputVisitor {


			private InputCompiler inCompiler;


			private CompilerResult<IInputStatement> result;


			public InputCompilerVisitor(InputCompiler inComp) {
				inCompiler = inComp;
			}


			public CompilerResult<IInputStatement> TryCompile(Ast.IInputStatement astStatement) {

				astStatement.Accept(this);
				return result;
			}



			#region IInputVisitor Members

			public void Visit(Ast.ConstantDefinition constDef) {
				result = inCompiler.CompileConstDef(constDef);
			}

			public void Visit(Ast.EmptyStatement emptyStat) {
				result = CompilerResult<IInputStatement>.Error;
			}

			public void Visit(Ast.FunctionDefinition funDef) {
				result = inCompiler.CompileFunctionDef(funDef);
			}

			public void Visit(Ast.LsystemDefinition lsysDef) {
				result = inCompiler.LsystemCompiler.CompileLsystem(lsysDef);
			}

			#endregion
		}
	}

	public enum InputCompilerMessageType {
		Unknown,
		RequiredParamAfterOptional,
		ParamNameNotUnique,
	}
}

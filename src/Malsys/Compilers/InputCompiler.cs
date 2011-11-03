using System.Collections.Generic;
using System.Diagnostics;
using Malsys.Expressions;
using Malsys.Parsing;
using Microsoft.FSharp.Text.Lexing;

namespace Malsys.Compilers {
	public class InputCompiler {

		private MessagesCollection msgs;

		private LsystemCompiler lsysCompiler;
		private ExpressionCompiler exprCompiler;


		public MessagesCollection Messages { get { return msgs; } }
		public LsystemCompiler LsystemCompiler { get { return lsysCompiler; } }
		public ExpressionCompiler ExpressionCompiler { get { return exprCompiler; } }


		public InputCompiler(MessagesCollection msgsColl) {
			msgs = msgsColl;

			lsysCompiler = new LsystemCompiler(this);
			exprCompiler = new ExpressionCompiler(msgs);
		}




		public CompilerResult<InputBlock> CompileFromString(string strInput, string sourceName) {

			var lexBuff = LexBuffer<char>.FromString(strInput);
			msgs.DefaultSourceName = sourceName;
			var comments = new List<Ast.Comment>();

			var parsedInput = ParserUtils.ParseLsystemStatements(comments, lexBuff, msgs, sourceName);

			return CompileFromAst(parsedInput);
		}

		public CompilerResult<InputBlock> CompileFromAst(Ast.InputBlock parsedInput) {

			var lsysDefs = new List<LsystemDefinition>();
			var varDefs = new List<VariableDefinition<IExpression>>();
			var funDefs = new List<FunctionDefinition>();

			foreach (var statement in parsedInput) {

				if (statement is Malsys.Ast.Lsystem) {
					var lsysResult = lsysCompiler.Compile((Ast.Lsystem)statement);
					if (lsysResult) {
						lsysDefs.Add(lsysResult);
					}
				}

				else if (statement is Ast.VariableDefinition) {
					var vd = CompileFailSafe((Ast.VariableDefinition)statement);
					varDefs.Add(vd);
				}

				else if (statement is Ast.FunctionDefinition) {
					var fd = CompileFailSafe((Ast.FunctionDefinition)statement);
					funDefs.Add(fd);
				}

				else if (statement is Ast.EmptyStatement) {
					msgs.AddMessage("Empty statement found.", CompilerMessageType.Notice, statement.Position);
				}

				else {
					Debug.Fail("Unhandled type `{0}` of {1} while compiling input block `{2}`.".Fmt(
						statement.GetType().Name, typeof(Ast.IInputStatement).Name, msgs.DefaultSourceName));

					msgs.AddError("Internal L-system input compiler error.", statement.Position);
				}
			}


			if (msgs.ErrorOcured) {
				msgs.AddError("Some error ocured during compilation of `{0}`.".Fmt(msgs.DefaultSourceName), Position.Unknown);
				Debug.Assert(msgs.ErrorOcured, "Compiler's result error state is false, but error ocured.");
				return CompilerResult<InputBlock>.Error;
			}

			var lsysImm = new ImmutableList<LsystemDefinition>(lsysDefs);
			var varsImm = new ImmutableList<VariableDefinition<IExpression>>(varDefs);
			var funsImm = new ImmutableList<FunctionDefinition>(funDefs);

			return new InputBlock(lsysImm, varsImm, funsImm);
		}





		public FunctionDefinition CompileFailSafe(Ast.FunctionDefinition funDef) {

			var prms = CompileParametersFailSafe(funDef.Parameters);
			var varDefs = CompileFailSafe(funDef.LocalVarDefs);
			var retExpr = exprCompiler.CompileExpression(funDef.ReturnExpression);

			return new FunctionDefinition(funDef.NameId.Name, prms, varDefs, retExpr);
		}

		public ImmutableList<OptionalParameter> CompileParametersFailSafe(ImmutableList<Ast.OptionalParameter> parameters) {

			int parametersCount = parameters.Count;
			bool wasOptional = false;
			var result = new OptionalParameter[parametersCount];

			for (int i = 0; i < parametersCount; i++) {

				var prm = parameters[i];
				var compiledParam = CompileParameterFailSafe(prm);
				result[i] = compiledParam;

				if (compiledParam.IsOptional) {
					wasOptional = true;
				}
				else {
					if (wasOptional) {
						msgs.AddError("Mandatory parameters have to be before all optional parameters, but mandatory parameter `{0}` is after optional.".Fmt(prm.NameId.Name),
							prm.Position);
					}
				}
			}

			// check wether parameters names are unique
			foreach (var indices in result.GetEqualValuesIndices((l, r) => { return l.Name.Equals(r.Name); })) {
				msgs.AddError("{0}. and {1}. parameter have same name `{2}`.".Fmt(indices.Item1 + 1, indices.Item2 + 1, result[indices.Item1].Name),
					parameters[indices.Item1].Position, parameters[indices.Item2].Position);
			}

			return new ImmutableList<OptionalParameter>(result, true);
		}

		public OptionalParameter CompileParameterFailSafe(Ast.OptionalParameter parameter) {

			if (parameter.IsOptional) {
				var value = exprCompiler.CompileExpression(parameter.OptionalValue);
				IValue evalValue;

				try {
					evalValue = ExpressionEvaluator.Evaluate(value);
				}
				catch (EvalException ex) {
					msgs.AddError("Faled to evaluate default value of parameter `{0}`. {1}".Fmt(parameter.NameId.Name, ex.GetWholeMessage()),
						parameter.OptionalValue.Position);
					evalValue = Constant.NaN;
				}

				return new OptionalParameter(parameter.NameId.Name, evalValue);
			}

			else {
				return new OptionalParameter(parameter.NameId.Name);
			}
		}




		public VariableDefinition<IExpression> CompileFailSafe(Ast.VariableDefinition varDef) {
			return new VariableDefinition<IExpression>(varDef.NameId.Name, exprCompiler.CompileExpression(varDef.Expression));
		}

		public VariableDefinition<SymbolsList<IExpression>> CompileFailSafe(Ast.SymbolsDefinition symDef) {
			return new VariableDefinition<SymbolsList<IExpression>>(symDef.NameId.Name, lsysCompiler.CompileListFailSafe(symDef.Symbols));
		}

		public ImmutableList<VariableDefinition<IExpression>> CompileFailSafe(ImmutableList<Ast.VariableDefinition> varDefs) {

			var rsltList = new VariableDefinition<IExpression>[varDefs.Length];

			for (int i = 0; i < varDefs.Length; i++) {
				rsltList[i] = CompileFailSafe(varDefs[i]);
			}

			return new ImmutableList<VariableDefinition<IExpression>>(rsltList, true);
		}
	}
}

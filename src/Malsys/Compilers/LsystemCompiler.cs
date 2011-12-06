using System.Collections.Generic;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Compiled;

namespace Malsys.Compilers {
	internal class LsystemCompiler : ILsystemCompiler, Ast.ILsystemVisitor {

		private MessageLogger msgs;
		private IConstantDefinitionCompiler constDefCompiler;
		private IFunctionDefinitionCompiler funDefCompiler;
		private IExpressionCompiler exprCompiler;
		private IParametersCompiler paramsCompiler;
		private IRewriteRuleCompiler rrCompiler;
		private ISymbolCompiler symbolCompiler;

		private CompilerResult<ILsystemStatement> visitResult;


		public LsystemCompiler(MessageLogger messageLogger, IConstantDefinitionCompiler constantDefCompiler, IFunctionDefinitionCompiler functionDefCompiler,
				IExpressionCompiler expressionCompiler, ParametersCompiler parametersCompiler, ISymbolCompiler symbolCompiler) {

			msgs = messageLogger;
			constDefCompiler = constantDefCompiler;
			funDefCompiler = functionDefCompiler;
			exprCompiler = expressionCompiler;
			paramsCompiler = parametersCompiler;
			this.symbolCompiler = symbolCompiler;
		}


		public Lsystem Compile(Ast.LsystemDefinition lsysDef) {

			var prms = paramsCompiler.CompileList(lsysDef.Parameters);
			var stats = compileLsystemStatements(lsysDef.Statements);

			return new Lsystem(lsysDef.NameId.Name, prms, stats, lsysDef);
		}


		private ImmutableList<ILsystemStatement> compileLsystemStatements(ImmutableList<Ast.ILsystemStatement> statements) {

			var compStats = new List<ILsystemStatement>(statements.Count);

			foreach (var stat in statements) {
				stat.Accept(this);
				if (visitResult) {
					compStats.Add(visitResult.Result);
				}
			}

			return new ImmutableList<ILsystemStatement>(compStats);
		}

		private SymbolsConstDefinition compileSymbolConstant(Ast.SymbolsConstDefinition symbolConstAst) {

			var symbols = symbolCompiler.CompileList<Ast.LsystemSymbol, Symbol<IExpression>>(symbolConstAst.SymbolsList);
			return new SymbolsConstDefinition(symbolConstAst.NameId.Name, symbols);
		}

		private SymbolsInterpretation compileSymbolsInterpretation(Ast.SymbolsInterpretDef symbolsInterpretAst) {

			var symbols = symbolCompiler.CompileList<Ast.LsystemSymbol, Symbol<VoidStruct>>(symbolsInterpretAst.Symbols);
			var defVals = exprCompiler.CompileList(symbolsInterpretAst.DefaultParameters);
			return new SymbolsInterpretation(symbolsInterpretAst.Instruction.Name, defVals, symbols);
		}


		#region ILsystemVisitor Members

		public void Visit(Ast.ConstantDefinition constDef) {
			visitResult = constDefCompiler.Compile(constDef);
		}

		public void Visit(Ast.EmptyStatement emptyStat) {
			visitResult = CompilerResult<ILsystemStatement>.Error;
		}

		public void Visit(Ast.FunctionDefinition funDef) {
			visitResult = funDefCompiler.Compile(funDef);
		}

		public void Visit(Ast.RewriteRule rewriteRule) {
			visitResult = rrCompiler.Compile(rewriteRule);
		}

		public void Visit(Ast.SymbolsInterpretDef symbolInterpretDef) {
			visitResult = compileSymbolsInterpretation(symbolInterpretDef);
		}

		public void Visit(Ast.SymbolsConstDefinition symbolsDef) {
			visitResult = compileSymbolConstant(symbolsDef);
		}

		public void Visit(Ast.ProcessStatement processDef) {
			throw new System.NotImplementedException();
		}

		#endregion

	}



}

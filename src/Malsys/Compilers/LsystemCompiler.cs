using System.Linq;
using System.Collections.Generic;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Compiled;

namespace Malsys.Compilers {
	internal class LsystemCompiler : ILsystemCompiler, Ast.ILsystemVisitor {

		private IConstantDefinitionCompiler constDefCompiler;
		private IFunctionDefinitionCompiler funDefCompiler;
		private IExpressionCompiler exprCompiler;
		private IParametersCompiler paramsCompiler;
		private IRewriteRuleCompiler rrCompiler;
		private ISymbolCompiler symbolCompiler;
		private IProcessStatementsCompiler processStatsCompiler;

		private IMessageLogger logger;
		private CompilerResult<ILsystemStatement> visitResult;


		public LsystemCompiler(IConstantDefinitionCompiler constantDefCompiler, IFunctionDefinitionCompiler functionDefCompiler,
				IExpressionCompiler expressionCompiler, IParametersCompiler parametersCompiler, ISymbolCompiler symbolCompiler,
				IRewriteRuleCompiler rewriteRuleCompiler, IProcessStatementsCompiler processStatementsCompiler) {

			constDefCompiler = constantDefCompiler;
			funDefCompiler = functionDefCompiler;
			exprCompiler = expressionCompiler;
			paramsCompiler = parametersCompiler;
			rrCompiler = rewriteRuleCompiler;
			this.symbolCompiler = symbolCompiler;
			processStatsCompiler = processStatementsCompiler;
		}


		public Lsystem Compile(Ast.LsystemDefinition lsysDef, IMessageLogger logger) {

			var prms = paramsCompiler.CompileList(lsysDef.Parameters, logger);
			var stats = compileLsystemStatements(lsysDef.Statements, logger);

			return new Lsystem(lsysDef.NameId.Name, prms, stats, lsysDef);
		}


		private ImmutableList<ILsystemStatement> compileLsystemStatements(ImmutableList<Ast.ILsystemStatement> statements, IMessageLogger logger) {

			var compStats = new List<ILsystemStatement>(statements.Count);
			this.logger = logger;

			foreach (var stat in statements) {
				stat.Accept(this);
				if (visitResult) {
					compStats.Add(visitResult.Result);
				}
			}

			logger = null;
			return new ImmutableList<ILsystemStatement>(compStats);
		}

		private SymbolsConstDefinition compileSymbolConstant(Ast.SymbolsConstDefinition symbolConstAst) {

			var symbols = symbolCompiler.CompileList<Ast.LsystemSymbol, Symbol<IExpression>>(symbolConstAst.SymbolsList, logger);
			return new SymbolsConstDefinition(symbolConstAst.NameId.Name, symbols);
		}

		private SymbolsInterpretation compileSymbolsInterpretation(Ast.SymbolsInterpretDef symbolsInterpretAst) {

			var symbols = symbolsInterpretAst.Symbols.Select(x => new Symbol<VoidStruct>(x.Name)).ToImmutableList();
			var prms = paramsCompiler.CompileList(symbolsInterpretAst.Parameters, logger);
			var defVals = exprCompiler.CompileList(symbolsInterpretAst.InstructionParameters, logger);
			return new SymbolsInterpretation(symbols, prms, symbolsInterpretAst.Instruction.Name, defVals, symbolsInterpretAst);
		}


		#region ILsystemVisitor Members

		public void Visit(Ast.ConstantDefinition constDef) {
			visitResult = constDefCompiler.Compile(constDef, logger);
		}

		public void Visit(Ast.EmptyStatement emptyStat) {
			visitResult = CompilerResult<ILsystemStatement>.Error;
		}

		public void Visit(Ast.FunctionDefinition funDef) {
			visitResult = funDefCompiler.Compile(funDef, logger);
		}

		public void Visit(Ast.RewriteRule rewriteRule) {
			visitResult = rrCompiler.Compile(rewriteRule, logger);
		}

		public void Visit(Ast.SymbolsInterpretDef symbolInterpretDef) {
			visitResult = compileSymbolsInterpretation(symbolInterpretDef);
		}

		public void Visit(Ast.SymbolsConstDefinition symbolsDef) {
			visitResult = compileSymbolConstant(symbolsDef);
		}

		public void Visit(Ast.ProcessStatement processStat) {
			visitResult = processStatsCompiler.Compile(processStat, logger);
		}

		#endregion

	}



}

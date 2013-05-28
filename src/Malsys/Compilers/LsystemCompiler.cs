// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Compiled;

namespace Malsys.Compilers {
	/// <remarks>
	/// All public members are thread safe if supplied compilers are thread safe.
	/// </remarks>
	public class LsystemCompiler : ILsystemCompiler {

		protected readonly IConstantDefinitionCompiler constDefCompiler;
		protected readonly IFunctionDefinitionCompiler funDefCompiler;
		protected readonly IExpressionCompiler exprCompiler;
		protected readonly IParametersCompiler paramsCompiler;
		protected readonly IRewriteRuleCompiler rrCompiler;
		protected readonly ISymbolCompiler symbolCompiler;


		public LsystemCompiler(IConstantDefinitionCompiler constantDefCompiler, IFunctionDefinitionCompiler functionDefCompiler,
				IExpressionCompiler expressionCompiler, IParametersCompiler parametersCompiler, ISymbolCompiler symbolCompiler,
				IRewriteRuleCompiler rewriteRuleCompiler) {

			constDefCompiler = constantDefCompiler;
			funDefCompiler = functionDefCompiler;
			exprCompiler = expressionCompiler;
			paramsCompiler = parametersCompiler;
			rrCompiler = rewriteRuleCompiler;
			this.symbolCompiler = symbolCompiler;
		}


		public Lsystem Compile(Ast.LsystemDefinition lsysDef, IMessageLogger logger) {

			var prms = paramsCompiler.CompileList(lsysDef.Parameters, logger);
			var baseLsys = compileBaseLsystems(lsysDef.BaseLsystems, logger);
			var stats = CompileList(lsysDef.Statements, logger);

			return new Lsystem(lsysDef.NameId.Name, lsysDef.IsAbstract, prms, baseLsys, stats, lsysDef);
		}

		public ILsystemStatement Compile(Ast.ILsystemStatement statement, IMessageLogger logger) {
			switch (statement.StatementType) {

				case Ast.LsystemStatementType.EmptyStatement:
					return null;

				case Ast.LsystemStatementType.ConstantDefinition:
					return constDefCompiler.Compile((Ast.ConstantDefinition)statement, logger);

				case Ast.LsystemStatementType.SymbolsConstDefinition:
					var symbolConstAst = (Ast.SymbolsConstDefinition)statement;
					var symbolsConst = symbolCompiler.CompileList<Ast.LsystemSymbol, Symbol<IExpression>>(symbolConstAst.SymbolsList, logger);
					return new SymbolsConstDefinition(symbolConstAst.NameId.Name, symbolsConst, symbolConstAst);

				case Ast.LsystemStatementType.SymbolsInterpretDef:
					var symIntDefAst = ((Ast.SymbolsInterpretDef)statement);
					var symbolsInterpret = symIntDefAst.Symbols.Select(x => new Symbol<VoidStruct>(x.Name)).ToImmutableList();
					var prms = paramsCompiler.CompileList(symIntDefAst.Parameters, logger);
					var defVals = exprCompiler.CompileList(symIntDefAst.InstructionParameters, logger);
					return new SymbolsInterpretation(symbolsInterpret, prms, symIntDefAst.Instruction.Name,
						defVals, symIntDefAst.InstructionIsLsystemName, symIntDefAst.LsystemConfigName.Name, symIntDefAst);

				case Ast.LsystemStatementType.FunctionDefinition:
					return funDefCompiler.Compile((Ast.FunctionDefinition)statement, logger);

				case Ast.LsystemStatementType.RewriteRule:
					return rrCompiler.Compile((Ast.RewriteRule)statement, logger);

				default:
					Debug.Fail("Unknown L-system statement type `{0}`.".Fmt(statement.StatementType.ToString()));
					return null;

			}
		}

		public ImmutableList<ILsystemStatement> CompileList(IEnumerable<Ast.ILsystemStatement> statementsList, IMessageLogger logger) {

			var compStats = new List<ILsystemStatement>();

			foreach (var stat in statementsList) {
				var cStat = Compile(stat, logger);
				if (cStat != null) {
					compStats.Add(cStat);
				}
			}

			return new ImmutableList<ILsystemStatement>(compStats);
		}


		protected ImmutableList<BaseLsystem> compileBaseLsystems(ImmutableList<Ast.BaseLsystem> baseLsys, IMessageLogger logger) {

			int length = baseLsys.Length;
			BaseLsystem[] result = new BaseLsystem[length];

			for (int i = 0; i < length; i++) {
				Ast.BaseLsystem astNode = baseLsys[i];
				result[i] = new BaseLsystem(astNode.NameId.Name, exprCompiler.CompileList(astNode.Arguments, logger), astNode);
			}

			return new ImmutableList<BaseLsystem>(result, true);
		}

	}



}

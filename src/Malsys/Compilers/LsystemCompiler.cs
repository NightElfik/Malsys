using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Compiled;

namespace Malsys.Compilers {
	/// <remarks>
	/// All public members are thread safe if supplied compilers are thread safe.
	/// </remarks>
	internal class LsystemCompiler : ILsystemCompiler {

		private readonly IConstantDefinitionCompiler constDefCompiler;
		private readonly IFunctionDefinitionCompiler funDefCompiler;
		private readonly IExpressionCompiler exprCompiler;
		private readonly IParametersCompiler paramsCompiler;
		private readonly IRewriteRuleCompiler rrCompiler;
		private readonly ISymbolCompiler symbolCompiler;


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
			var stats = compileLsystemStatements(lsysDef.Statements, logger);

			return new Lsystem(lsysDef.NameId.Name, lsysDef.IsAbstract, prms, baseLsys, stats, lsysDef);
		}


		private ImmutableList<BaseLsystem> compileBaseLsystems(ImmutableList<Ast.BaseLsystem> baseLsys, IMessageLogger logger) {

			int length = baseLsys.Length;
			BaseLsystem[] result = new BaseLsystem[length];

			for (int i = 0; i < length; i++) {
				Ast.BaseLsystem astNode = baseLsys[i];
				result[i] = new BaseLsystem(astNode.NameId.Name, exprCompiler.CompileList(astNode.Arguments, logger), astNode);
			}

			return new ImmutableList<BaseLsystem>(result, true);
		}


		private ImmutableList<ILsystemStatement> compileLsystemStatements(ImmutableList<Ast.ILsystemStatement> statements, IMessageLogger logger) {

			var compStats = new List<ILsystemStatement>(statements.Count);

			foreach (var stat in statements) {

				switch (stat.StatementType) {

					case Ast.LsystemStatementType.EmptyStatement:
						break;

					case Ast.LsystemStatementType.ConstantDefinition:
						compStats.Add(constDefCompiler.Compile((Ast.ConstantDefinition)stat, logger));
						break;

					case Ast.LsystemStatementType.SymbolsConstDefinition:
						var symbolConstAst = (Ast.SymbolsConstDefinition)stat;
						var symbolsConst = symbolCompiler.CompileList<Ast.LsystemSymbol, Symbol<IExpression>>(symbolConstAst.SymbolsList, logger);
						compStats.Add(new SymbolsConstDefinition(symbolConstAst.NameId.Name, symbolsConst));
						break;

					case Ast.LsystemStatementType.SymbolsInterpretDef:
						var symIntDefAst = ((Ast.SymbolsInterpretDef)stat);
						var symbolsInterpret = symIntDefAst.Symbols.Select(x => new Symbol<VoidStruct>(x.Name)).ToImmutableList();
						var prms = paramsCompiler.CompileList(symIntDefAst.Parameters, logger);
						var defVals = exprCompiler.CompileList(symIntDefAst.InstructionParameters, logger);
						compStats.Add(new SymbolsInterpretation(symbolsInterpret, prms, symIntDefAst.Instruction.Name,
							defVals, symIntDefAst.InstructionIsLsystemName, symIntDefAst.LsystemConfigName.Name, symIntDefAst));
						break;

					case Ast.LsystemStatementType.FunctionDefinition:
						compStats.Add(funDefCompiler.Compile((Ast.FunctionDefinition)stat, logger));
						break;

					case Ast.LsystemStatementType.RewriteRule:
						compStats.Add(rrCompiler.Compile((Ast.RewriteRule)stat, logger));
						break;

					default:
						Debug.Fail("Unknown L-system statement type `{0}`.".Fmt(stat.StatementType.ToString()));
						break;
				}
			}

			return new ImmutableList<ILsystemStatement>(compStats);
		}

	}



}

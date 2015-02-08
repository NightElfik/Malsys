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
			return new Lsystem(lsysDef) {
				Name = lsysDef.NameId.Name,
				IsAbstract = lsysDef.IsAbstract,
				Parameters = paramsCompiler.CompileList(lsysDef.Parameters, logger),
				BaseLsystems = compileBaseLsystems(lsysDef.BaseLsystems, logger),
				Statements = CompileList(lsysDef.Statements, logger),
			};
		}

		public ILsystemStatement Compile(Ast.ILsystemStatement statement, IMessageLogger logger) {
			switch (statement.StatementType) {

				case Ast.LsystemStatementType.EmptyStatement:
					return null;

				case Ast.LsystemStatementType.ConstantDefinition:
					return constDefCompiler.Compile((Ast.ConstantDefinition)statement, logger);

				case Ast.LsystemStatementType.SymbolsConstDefinition:
					var symbolConstAst = (Ast.SymbolsConstDefinition)statement;
					return new SymbolsConstDefinition(symbolConstAst) {
						Name = symbolConstAst.NameId.Name,
						Symbols = symbolCompiler.CompileList<Ast.LsystemSymbol, Symbol<IExpression>>(symbolConstAst.SymbolsList, logger)
					};

				case Ast.LsystemStatementType.SymbolsInterpretDef:
					var symIntDefAst = ((Ast.SymbolsInterpretDef)statement);
					return new SymbolsInterpretation(symIntDefAst) {
						Symbols = symIntDefAst.Symbols.Select(x => new Symbol<VoidStruct>(x) { Name = x.Name }).ToList(),
						Parameters = paramsCompiler.CompileList(symIntDefAst.Parameters, logger),
						InstructionName = symIntDefAst.Instruction.Name,
						InstructionParameters = exprCompiler.CompileList(symIntDefAst.InstructionParameters, logger),
						InstructionIsLsystemName = symIntDefAst.InstructionIsLsystemName,
						LsystemConfigName = symIntDefAst.LsystemConfigName.Name
					};

				case Ast.LsystemStatementType.FunctionDefinition:
					return funDefCompiler.Compile((Ast.FunctionDefinition)statement, logger);

				case Ast.LsystemStatementType.RewriteRule:
					return rrCompiler.Compile((Ast.RewriteRule)statement, logger);

				default:
					Debug.Fail("Unknown L-system statement type `{0}`.".Fmt(statement.StatementType.ToString()));
					return null;

			}
		}

		public List<ILsystemStatement> CompileList(IEnumerable<Ast.ILsystemStatement> statementsList, IMessageLogger logger) {
			return statementsList
				.Select(stat => Compile(stat, logger))
				.Where(cStat => cStat != null)
				.ToList();
		}


		protected List<BaseLsystem> compileBaseLsystems(IEnumerable<Ast.BaseLsystem> baseLsys, IMessageLogger logger) {
			return baseLsys
				.Select(astNode => new BaseLsystem(astNode) {
					Name = astNode.NameId.Name,
					Arguments = exprCompiler.CompileList(astNode.Arguments, logger),
				})
				.ToList();
		}

	}



}

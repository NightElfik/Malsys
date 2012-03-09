using System.Diagnostics;
using Malsys.SemanticModel.Compiled;

namespace Malsys.Compilers {
	/// <remarks>
	/// All public members are thread safe if supplied compilers are thread safe.
	/// </remarks>
	internal class FunctionDefCompiler : IFunctionDefinitionCompiler {

		private readonly IConstantDefinitionCompiler constDefCompiler;
		private readonly IExpressionCompiler exprCompiler;
		private readonly IParametersCompiler paramsCompiler;


		public FunctionDefCompiler(IConstantDefinitionCompiler constantDefCompiler, IExpressionCompiler expressionCompiler, IParametersCompiler parametersCompiler) {
			constDefCompiler = constantDefCompiler;
			exprCompiler = expressionCompiler;
			paramsCompiler = parametersCompiler;
		}


		public Function Compile(Ast.FunctionDefinition funDefAst, IMessageLogger logger) {

			var compiledStats = new IFunctionStatement[funDefAst.Statements.Length];

			for (int i = 0; i < funDefAst.Statements.Length; i++) {

				var stat = funDefAst.Statements[i];

				switch (stat.StatementType) {

					case Ast.FunctionStatementType.ConstantDefinition:
						compiledStats[i] = constDefCompiler.Compile((Ast.ConstantDefinition)stat, logger);
						break;

					case Ast.FunctionStatementType.Expression:
						compiledStats[i] = new FunctionReturnExpr(exprCompiler.Compile((Ast.Expression)stat, logger));
						break;

					default:
						Debug.Fail("Unknown function statement type `{0}`.".Fmt(stat.StatementType.ToString()));
						break;

				}
			}

			var prms = paramsCompiler.CompileList(funDefAst.Parameters, logger);
			var stats = new ImmutableList<IFunctionStatement>(compiledStats, true);
			return new Function(funDefAst.NameId.Name, prms, stats, funDefAst);
		}

	}
}

using System.Diagnostics;
using System.Linq;
using Malsys.SemanticModel.Compiled;

namespace Malsys.Compilers {
	/// <remarks>
	/// All public members are thread safe if supplied compilers are thread safe.
	/// </remarks>
	public class FunctionDefCompiler : IFunctionDefinitionCompiler {

		protected readonly IConstantDefinitionCompiler constDefCompiler;
		protected readonly IExpressionCompiler exprCompiler;
		protected readonly IParametersCompiler paramsCompiler;


		public FunctionDefCompiler(IConstantDefinitionCompiler constantDefCompiler, IExpressionCompiler expressionCompiler, IParametersCompiler parametersCompiler) {
			constDefCompiler = constantDefCompiler;
			exprCompiler = expressionCompiler;
			paramsCompiler = parametersCompiler;
		}


		public Function Compile(Ast.FunctionDefinition funDefAst, IMessageLogger logger) {

			var stats = funDefAst.Statements.Select(stat => {
				switch (stat.StatementType) {
					case Ast.FunctionStatementType.ConstantDefinition:
						return constDefCompiler.Compile((Ast.ConstantDefinition)stat, logger) as IFunctionStatement;
					case Ast.FunctionStatementType.Expression:
						return new FunctionReturnExpr(stat) {
							ReturnValue = exprCompiler.Compile((Ast.Expression)stat, logger),
						} as IFunctionStatement;
					default:
						Debug.Fail("Unknown function statement type `{0}`.".Fmt(stat.StatementType.ToString()));
						return null as IFunctionStatement;
				}
			}).ToList();

			return new Function(funDefAst) {
				Name = funDefAst.NameId.Name,
				Parameters = paramsCompiler.CompileList(funDefAst.Parameters, logger),
				Statements = stats,
			};
		}

	}
}

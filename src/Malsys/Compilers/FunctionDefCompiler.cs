using Malsys.SemanticModel.Compiled;

namespace Malsys.Compilers {
	internal class FunctionDefCompiler : IFunctionDefinitionCompiler, Ast.IFunctionVisitor {

		private IConstantDefinitionCompiler constDefCompiler;
		private IExpressionCompiler exprCompiler;
		private IParametersCompiler paramsCompiler;

		private IFunctionStatement visitResult;


		public FunctionDefCompiler(IConstantDefinitionCompiler constantDefCompiler, IExpressionCompiler expressionCompiler, IParametersCompiler parametersCompiler) {

			constDefCompiler = constantDefCompiler;
			exprCompiler = expressionCompiler;
			paramsCompiler = parametersCompiler;
		}


		public Function Compile(Ast.FunctionDefinition funDefAst) {

			var compiledStats = new IFunctionStatement[funDefAst.Statements.Length];

			for (int i = 0; i < funDefAst.Statements.Length; i++) {
				funDefAst.Statements[i].Accept(this);
				compiledStats[i] = visitResult;
			}

			var prms = paramsCompiler.CompileList(funDefAst.Parameters);
			var stats = new ImmutableList<IFunctionStatement>(compiledStats, true);
			return new Function(funDefAst.NameId.Name, prms, stats, funDefAst);
		}



		public void Visit(Ast.ConstantDefinition constDef) {
			visitResult = constDefCompiler.Compile(constDef);
		}

		public void Visit(Ast.Expression expr) {
			visitResult = new FunctionReturnExpr(exprCompiler.Compile(expr));
		}

	}
}

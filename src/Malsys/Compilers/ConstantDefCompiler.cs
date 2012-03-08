using Malsys.SemanticModel.Compiled;

namespace Malsys.Compilers {
	internal class ConstantDefCompiler : IConstantDefinitionCompiler {

		private IExpressionCompiler exprCompiler;


		public ConstantDefCompiler(IExpressionCompiler expressionCompiler) {
			exprCompiler = expressionCompiler;
		}


		public ConstantDefinition Compile(Ast.ConstantDefinition constDefAst, IMessageLogger logger) {
			var expr = exprCompiler.Compile(constDefAst.ValueExpr, logger);
			return new ConstantDefinition(constDefAst.NameId.Name, expr, constDefAst.IsComponentAssign, constDefAst);
		}

	}
}

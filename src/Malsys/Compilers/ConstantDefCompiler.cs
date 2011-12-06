using Malsys.SemanticModel.Compiled;

namespace Malsys.Compilers {
	internal class ConstantDefCompiler : IConstantDefinitionCompiler {

		private ExpressionCompiler exprCompiler;


		public ConstantDefCompiler(ExpressionCompiler expressionCompiler) {
			exprCompiler = expressionCompiler;
		}


		public ConstantDefinition Compile(Ast.ConstantDefinition constDefAst) {
			var expr = exprCompiler.Compile(constDefAst.ValueExpr);
			return new ConstantDefinition(constDefAst.NameId.Name, expr, constDefAst);
		}

	}
}

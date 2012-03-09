using Malsys.SemanticModel.Compiled;

namespace Malsys.Compilers {
	/// <remarks>
	/// All public members are thread safe if supplied compilers are thread safe.
	/// </remarks>
	internal class ConstantDefCompiler : IConstantDefinitionCompiler {

		private readonly IExpressionCompiler exprCompiler;


		public ConstantDefCompiler(IExpressionCompiler expressionCompiler) {
			exprCompiler = expressionCompiler;
		}


		public ConstantDefinition Compile(Ast.ConstantDefinition constDefAst, IMessageLogger logger) {
			var expr = exprCompiler.Compile(constDefAst.ValueExpr, logger);
			return new ConstantDefinition(constDefAst.NameId.Name, expr, constDefAst.IsComponentAssign, constDefAst);
		}

	}
}

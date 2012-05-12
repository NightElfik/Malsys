/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using Malsys.SemanticModel.Compiled;

namespace Malsys.Compilers {
	/// <remarks>
	/// All public members are thread safe if supplied compilers are thread safe.
	/// </remarks>
	public class ConstantDefCompiler : IConstantDefinitionCompiler {

		protected readonly IExpressionCompiler exprCompiler;


		public ConstantDefCompiler(IExpressionCompiler expressionCompiler) {
			exprCompiler = expressionCompiler;
		}


		public ConstantDefinition Compile(Ast.ConstantDefinition constDefAst, IMessageLogger logger) {
			var expr = exprCompiler.Compile(constDefAst.ValueExpr, logger);
			return new ConstantDefinition(constDefAst.NameId.Name, expr, constDefAst.IsComponentAssign, constDefAst);
		}

	}
}

using Malsys.SemanticModel.Compiled;

namespace Malsys.Compilers {
	class FunctionCompilerVisitor : Ast.IFunctionVisitor {

		InputCompiler inCompiler;

		IFunctionStatement result;


		public FunctionCompilerVisitor(InputCompiler inComp) {
			inCompiler = inComp;
		}

		public IFunctionStatement Compile(Ast.IFunctionStatement funStatAst) {
			funStatAst.Accept(this);
			return result;
		}


		#region IFunctionVisitor Members

		public void Visit(Ast.ConstantDefinition constDef) {
			result = inCompiler.CompileConstDef(constDef);
		}

		public void Visit(Ast.Expression expr) {
			result = new FunctionReturnExpr(inCompiler.ExpressionCompiler.CompileExpression(expr));
		}

		#endregion

	}
}

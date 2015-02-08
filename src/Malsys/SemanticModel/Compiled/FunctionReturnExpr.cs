
namespace Malsys.SemanticModel.Compiled {
	class FunctionReturnExpr : IFunctionStatement {

		public IExpression ReturnValue;

		public readonly Ast.IFunctionStatement AstNode;


		public FunctionReturnExpr(Ast.IFunctionStatement astNode) {
			AstNode = astNode;
		}

		
		public FunctionStatementType StatementType {
			get { return FunctionStatementType.ReturnExpression; }
		}

	}
}

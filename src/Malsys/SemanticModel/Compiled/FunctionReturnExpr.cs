// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.

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

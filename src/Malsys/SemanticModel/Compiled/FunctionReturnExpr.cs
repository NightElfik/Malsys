// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.

namespace Malsys.SemanticModel.Compiled {
	class FunctionReturnExpr : IFunctionStatement {

		public readonly IExpression ReturnValue;


		public FunctionReturnExpr(IExpression retVal) {
			ReturnValue = retVal;
		}


		#region IFunctionStatement Members

		public FunctionStatementType StatementType {
			get { return FunctionStatementType.ReturnExpression; }
		}

		#endregion
	}
}
